// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable ParameterOnlyUsedForPreconditionCheck.Global
namespace Webfarm.Sdk.AutoRegistration
{
    using System;
    using System.Linq;
    using System.Reflection;
    using JetBrains.Annotations;

    #pragma warning disable CA1716 // Identifiers should not match keywords
    public static class If
    #pragma warning restore CA1716 // Identifiers should not match keywords
    {
        public static bool DecoratedWith<TAttr>([NotNull] this Type type)
            where TAttr : Attribute
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.GetTypeInfo().GetCustomAttributes(false).Any(a => a.GetType() == typeof(TAttr));
        }

        public static bool Implements<TContract>([NotNull] this Type type)
            where TContract : class
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.GetTypeInfo().ImplementedInterfaces.Any(i => i == typeof(TContract));
        }

        public static bool EndsWith([NotNull] this Type type, [NotNull] string name)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.GetTypeInfo().Name.EndsWith(name, StringComparison.Ordinal);
        }

        [NotNull]
        public static Predicate<Type> ImplementsOpenGeneric([NotNull] Type contract)
        {
            return t => ImplementsOpenGeneric(t, contract);
        }

        public static bool ImplementsOpenGeneric([NotNull] this Type type, [NotNull] Type contract)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            var contractTypeInfo = contract.GetTypeInfo();

            if (!contractTypeInfo.IsInterface)
            {
                throw new ArgumentException("Provided contract has to be an interface", nameof(contract));
            }

            if (!contractTypeInfo.IsGenericTypeDefinition)
            {
                throw new ArgumentException("Provided contract has to be an open generic", nameof(contract));
            }

            return type.GetTypeInfo().ImplementedInterfaces.Any(i => i.GetTypeInfo().IsGenericType && (i.GetGenericTypeDefinition() == contract));
        }

        public static bool ImplementsITypeName([NotNull] this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.GetTypeInfo().ImplementedInterfaces.Any(i =>
                i.Name.StartsWith("I", StringComparison.OrdinalIgnoreCase)
                && i.Name.Remove(0, 1) == type.Name);
        }

        public static bool ImplementsSingleInterface([NotNull] this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.GetTypeInfo().ImplementedInterfaces.Count() == 1;
        }

        public static bool Any([NotNull] this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return true;
        }

        public static bool Is<T>([NotNull] this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type == typeof(T);
        }

        public static bool IsAssignableFrom<T>([NotNull] this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo());
        }

        public static bool AnyAssembly([NotNull] this Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            return true;
        }

        public static bool ContainsType<T>([NotNull] this Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            return typeof(T).GetTypeInfo().Assembly == assembly;
        }

        [NotNull]
        public static Predicate<Type> NotEndsWith(string name)
        {
            return t => !t.EndsWith(name);
        }

        [NotNull]
        public static Predicate<Type> EndsWith(string name)
        {
            return t => t.EndsWith(name);
        }
    }
}
