using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Framework.Flazor.Token
{
    /// <summary>
    /// Implements an in-memory token provider.
    /// </summary>
    public class TokenProvider : ITokenProvider
    {
        private readonly ConcurrentDictionary<string, CancellationTokenSource> _lockObjects = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokenId">
        /// A token identifier. It defaults to the caller file-path, which will typically provide a
        /// sufficiently unique ID. User's can supply their own if they need greater control however.
        /// </param>
        /// <returns></returns>
        public CancellationToken GetNewTokenAndCancelPreviousTokens([CallerFilePath] string? tokenId = null)
        {
            tokenId ??= string.Empty;
            var tokenSource = _lockObjects.AddOrUpdate(tokenId, CreateTokenSource, ReplaceTokenSource);
            return tokenSource.Token;
        }

        private CancellationTokenSource ReplaceTokenSource(
            string _, CancellationTokenSource existing)
        {
            existing.Cancel();
            return new();
        }

        private CancellationTokenSource CreateTokenSource(string _) => new();
    }
}
