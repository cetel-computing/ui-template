using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Framework.Flazor.Autoregister;
using Fluxor.DependencyInjection;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace Flazor.Extensions
{
    public static class ServicesAutoregisterExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="assemblyToScan">The assembly to scan </param>
        /// <returns><see cref="FluxorOptions"/> for chaining</returns>
        // IMPORTANT: It is critical that this method is not inlined, as it calls <see cref="Assembly.GetCallingAssembly"/>.
        [MethodImpl(NoInlining)]
        public static FluxorOptions AutoregisterFeatures(this FluxorOptions options, params Assembly[] assembliesToScan)
        {
            if (!assembliesToScan.Any())
            {
                assembliesToScan = new[] { Assembly.GetCallingAssembly() };
            }

            var typeProvider = new TypeProvider(assembliesToScan);

            var assembly = typeProvider.CreateAssembly();

            options.ScanAssemblies(assembly, assembliesToScan);

            return options;
        }
    }
}
