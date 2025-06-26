using System;
using System.Threading;
using System.Threading.Tasks;
using Fluxor;
using Microsoft.Extensions.Logging;

namespace Framework.Flazor
{
    /// <summary>
    /// Base class for a Fluxor effect that incorporates error-handling and cancellation.
    /// </summary>
    /// <typeparam name="TAction">The type of action this effect handles.</typeparam>
    public abstract class BaseEffect<TAction> : Effect<TAction>
    {
        public BaseEffect(ILogger logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the message to be displayed to the user in the event of an error.
        /// </summary>
        /// <remarks>
        /// If this property returns <c>null</c> the user will not get notified.
        /// </remarks>
        protected abstract string? ErrorNotificationMessage { get; }

        /// <summary>
        /// Returns a cancellation token that is used to check if the job has cancelled.
        /// </summary>
        /// <remarks>
        /// If not overridden, this will return a <c>default</c> token.
        /// </remarks>
        /// <returns>A cancellation token</returns>
        protected virtual CancellationToken GetCancellationToken() => default;

        public override async Task HandleAsync(TAction action, IDispatcher dispatcher)
        {
            var token = GetCancellationToken();
            try
            {
                await HandleAction(action, dispatcher, token);
            }
            catch (OperationCanceledException e) when (e.CancellationToken == token)
            {
                // Do nothing - operation was cancelled.
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Failed to handle action {Action}", action?.GetType()?.Name);
                if (ErrorNotificationMessage is not null)
                {
                    dispatcher.DispatchError(ErrorNotificationMessage);
                }
            }
        }

        /// <summary>
        /// Handles the raised action.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="dispatcher"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected abstract Task HandleAction(TAction action, IDispatcher dispatcher, CancellationToken cancellationToken = default);
    }
}
