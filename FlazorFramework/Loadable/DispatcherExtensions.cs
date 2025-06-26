using System;
using System.Threading;
using System.Threading.Tasks;
using Fluxor;

namespace Framework.Flazor.Loadable
{
    /// <summary>
    /// Extension methods on <see cref="IDispatcher"/>.
    /// </summary>
    public static class DispatcherExtensions
    {
        /// <summary>
        /// Loads data using the standard Loadable pattern.
        /// </summary>
        /// <typeparam name="T">The type of the data</typeparam>
        /// <param name="dispatcher">The dispatcher</param>
        /// <param name="dataAccessor">A function to retrieve the data with.</param>
        /// <param name="token">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>The data returned from <paramref name="dataAccessor"/></returns>
        public static async Task<T> LoadData<T>(this IDispatcher dispatcher, Func<Task<T>> dataAccessor, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                dispatcher.Dispatch(Load.Loading<T>());

                var data = await dataAccessor();

                token.ThrowIfCancellationRequested();
                dispatcher.Dispatch(Load.Success(data));

                return data;
            }
            catch (OperationCanceledException e) when (e.CancellationToken == token)
            {
                // Operation was cancelled. Do nothing.
                return await Task.FromCanceled<T>(token);
            }
            catch (Exception e)
            {
                dispatcher.Dispatch(Load.Error<T>(e));
                throw;
            }
        }
    }
}
