using Microsoft.Extensions.Options;
using Microsoft.Rest;
using FlazorTemplate.Configuration;
using FlazorTemplate.Providers;

namespace FlazorTemplate.Services.Data
{
    public class ApiDataAccessService : IApiDataAccessService
    {
        //public IPernixApi Client { get; set; }

        /// <summary>
        /// Default CORE constructor
        /// </summary>
        /// <param name="config">Dependency-injected options</param>
        public ApiDataAccessService(IOptions<AppSettings> settings, HttpClient httpClient, TokenProvider tokenProvider)
        {
            var baseUrl = settings.Value.ApiUrl ?? throw new NullReferenceException(nameof(settings.Value.ApiUrl));

            var bearerToken = tokenProvider.AccessToken;

            //Client = new PernixApi(new Uri(baseUrl),
            //    new TokenCredentials(bearerToken, "Bearer"),
            //    httpClient);
        }        
        
        public void Dispose()
        {
            //Client?.Dispose();
        }
    }
}
