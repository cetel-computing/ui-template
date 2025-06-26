using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;

namespace FlazorTemplate
{
    /// <summary>
    /// Implementation of an <see cref="ITicketStore" /> to permit us to store the user's authentication ticket server-side, rather than client-side.
    /// </summary>
    /// <see href="https://corgit.corvid.it/development/customerportal/-/issues/79"/>
    /// <see href="https://stackoverflow.com/a/40631965/15393"/>
    public class MemoryCacheTicketStore : ITicketStore
    {
        private const string KeyPrefix = "AuthSessionStore-";
        private readonly IMemoryCache _cache;

        public MemoryCacheTicketStore(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var key = KeyPrefix + Guid.NewGuid();
            await RenewAsync(key, ticket);
            return key;
        }

        public Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            // https://github.com/aspnet/Caching/issues/221
            // Set to "NeverRemove" to prevent undesired evictions from gen2 GC
            var options = new MemoryCacheEntryOptions
            {
                Priority = CacheItemPriority.NeverRemove
            };

            var expiresUtc = ticket.Properties.ExpiresUtc;

            if (expiresUtc.HasValue)
            {
                options.SetAbsoluteExpiration(expiresUtc.Value);
            }

            options.SetSlidingExpiration(TimeSpan.FromMinutes(60));

            _cache.Set(key, ticket, options);

            return Task.CompletedTask;
        }

        public Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            _cache.TryGetValue(key, out AuthenticationTicket ticket);
            return Task.FromResult(ticket);
        }

        public Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            return Task.CompletedTask;
        }
    }
}
