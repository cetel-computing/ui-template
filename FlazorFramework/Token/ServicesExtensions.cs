using Framework.Flazor.Token;
using Fluxor.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Flazor.Extensions
{
    public static class ServicesTokenProvider
    {
        /// <summary>
        /// Adds an in-memory token provider.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="assemblyToScan">The assembly to scan </param>
        /// <returns><see cref="FluxorOptions"/> for chaining</returns>
        public static FluxorOptions AddInMemoryTokenProvider(this FluxorOptions options)
        {
            options.Services.AddScoped<ITokenProvider, TokenProvider>();
            return options;
        }
    }
}
