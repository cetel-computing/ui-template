using System.Reflection;
using System.Runtime.CompilerServices;
using Blazored.Toast;
using Framework.Flazor;
using Fluxor.DependencyInjection;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace Flazor.Extensions
{
    public static class ServicesErrorHandlingExtensions
    {
        /// <summary>
        /// Adds support for handling <see cref="Error"/> actions by presenting them to the user.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="assemblyToScan">The assembly to scan </param>
        /// <returns><see cref="FluxorOptions"/> for chaining</returns>
        // IMPORTANT: It is critical that this method is not inlined, as it calls <see cref="Assembly.GetExecutingAssembly"/>.
        [MethodImpl(NoInlining)]
        public static FluxorOptions AddErrorHandling(this FluxorOptions options, params Assembly[] assembliesToScan)
        {
            options.Services.AddBlazoredToast();

            options.ScanAssemblies(Assembly.GetExecutingAssembly());

            return options;
        }
    }
}
