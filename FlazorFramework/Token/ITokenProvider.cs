using System.Threading;

namespace Framework.Flazor.Token
{
    /// <summary>
    /// Provides a mechanism for ensuring only one activity is being performed at any given time, by cancelling all previous related activities.
    /// </summary>
    /// <see href="https://corgit.corvid.it/development/wdmt/wdmt.ui/-/issues/213"/>
    /// <see href="https://gitter.im/mrpmorris/fluxor?at=61017a1858232d5ab4ecbc9f"/>
    public interface ITokenProvider
    {
        /// <summary>
        /// Gets a cancellation token associated with <paramref name="tokenId"/>, and cancels all previous tokens issued for that tokenId.
        /// </summary>
        /// <param name="tokenId">An ID to associate the cancellation token with.</param>
        /// <returns>A cancellation token.</returns>
        /// <remarks>
        /// If <c>tokenId</c> is not provided, implementations should provide a sensible value (e.g. using [CallerFilePath]).
        /// </remarks>
        CancellationToken GetNewTokenAndCancelPreviousTokens(string? tokenId = null);
    }
}
