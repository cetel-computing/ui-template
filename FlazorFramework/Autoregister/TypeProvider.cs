using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Framework.Flazor.Autoregister
{
    internal class TypeProvider : ITypeProvider
    {
        private readonly IEnumerable<Assembly> _assembliesToScan;

        public TypeProvider(IEnumerable<Assembly> assembliesToScan)
        {
            _assembliesToScan = assembliesToScan ?? throw new ArgumentNullException(nameof(assembliesToScan));
        }

        public Assembly CreateAssembly()
        {
            var assemblyName = new AssemblyName("DynamicFlazorFeatureAssembly");

            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                assemblyName,
                AssemblyBuilderAccess.Run);

            if (assemblyName.Name is null)
            {
                throw new Exception("AssemblyName is null");
            }

            var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);

            foreach (var (name, stateType) in GetStateTypesToAutoregister())
            {
                var baseType = typeof(Feature<>).MakeGenericType(stateType);
                var typeBuilder = moduleBuilder
                    .DefineType(
                        stateType.Name + "_Feature",
                        TypeAttributes.Public |
                        TypeAttributes.Class |
                        TypeAttributes.Sealed,
                        baseType);

                if (!string.IsNullOrWhiteSpace(name))
                {
                    CallBaseConstructor(baseType, typeBuilder, name);
                }

                try
                {
                    typeBuilder.CreateType();
                }
                catch (Exception e)
                {
                    throw new Exception($"Failed to create type for state '{stateType.Name}'", e);
                }
            }

            var generatedTypes = assemblyBuilder.DefinedTypes.ToArray();

            return generatedTypes.FirstOrDefault()?.Assembly ?? throw new Exception("No types defined in assembly");
        }

        private static void CallBaseConstructor(Type baseType, TypeBuilder typeBuilder, string name)
        {
            // Developer has supplied a name, so we must respect it.
            // See https://softwareproduction.eu/2014/07/21/reflection-emit-and-type-inheritance-calling-base-type-constructors/
            var ctorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, null);

            var ilGenerator = ctorBuilder.GetILGenerator();

            var baseConstructor = baseType.GetConstructor(
                bindingAttr: BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance,
                binder: null,
                types: new[] { typeof(string) },
                modifiers: null) ??
                throw new Exception($"No constructor found for {baseType}");

            ilGenerator.Emit(OpCodes.Ldarg_0);                // push "this" onto stack.
            ilGenerator.Emit(OpCodes.Ldstr, name);            // push 'name' onto stack
            ilGenerator.Emit(OpCodes.Call, baseConstructor);  // call base constructor

            ilGenerator.Emit(OpCodes.Nop);                    // C# compiler add 2 NOPS, so
            ilGenerator.Emit(OpCodes.Nop);                    // we'll add them, too.

            ilGenerator.Emit(OpCodes.Ret);                    // Return
        }

        private IEnumerable<(string name, Type type)> GetStateTypesToAutoregister()
        {
            var stateTypes = _assembliesToScan
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsPublic)
                .Where(HasFeatureAttribute)
                .Select(type => (type.GetCustomAttribute<FeatureAttribute>()?.FeatureName ?? string.Empty, type));

            return stateTypes;
        }

        private static bool HasFeatureAttribute(Type type)
        {
            return type.GetCustomAttribute<FeatureAttribute>() != null;
        }
    }
}
