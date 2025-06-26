using System.Reflection;

namespace Framework.Flazor.Autoregister
{
    /// <summary>
    /// Provides access to the states to auto-register.
    /// </summary>
    internal interface ITypeProvider
    {
        /// <summary>
        /// Creates an assembly containing the auto-registered Features.
        /// </summary>
        /// <returns></returns>
        Assembly CreateAssembly();
    }
}
