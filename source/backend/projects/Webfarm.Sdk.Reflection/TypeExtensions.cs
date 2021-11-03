namespace Webfarm.Sdk.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using JetBrains.Annotations;

    public static class TypeExtensions
    {
        [NotNull]
        public static MethodInfo GetGenericMethod(
            [NotNull] this Type t,
            string name,
            Type[] genericArgTypes,
            Type[] argTypes,
            Type returnType)
        {
            var method = (
                from m in t.GetMethods(BindingFlags.Public | BindingFlags.Static)
                let args = m.GetParameters().Select(pi => pi.ParameterType.IsGenericType ? pi.ParameterType.GetGenericTypeDefinition() : pi.ParameterType).ToArray()
                where
                    m.Name == name
                    && m.GetGenericArguments().Length == genericArgTypes.Length
                    && (returnType == null ||
                        (m.ReturnType.IsGenericType ? m.ReturnType.GetGenericTypeDefinition() : m.ReturnType) ==
                        returnType)
                    && args.SequenceEqual(argTypes)
                select m)
                .Single()
                .MakeGenericMethod(genericArgTypes);

            return method;
        }

        [CanBeNull]
        public static Type FindOpenGeneric([NotNull] this Type type, Type openGeneric)
        {
            Contract.Assert(type != null);

            return type.GetInterfaces()
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == openGeneric);
        }

        public static bool IsSubclassOfRawGeneric(this Type toCheck, Type generic)
        {
            return toCheck.FindRelatedGeneric(generic) != null;
        }

        [NotNull]
        public static IEnumerable<Type> FindRelatedGenericArguments(this Type toCheck, Type generic)
        {
            var relatedGeneric = toCheck.FindRelatedGeneric(generic);
            return relatedGeneric?.GetGenericArguments() ?? Array.Empty<Type>();
        }

        [NotNull]
        public static IEnumerable<Type> GetImplementedInterfacesFixed([NotNull] this Type type)
        {
            var typeInfo = type.GetTypeInfo();

            return GetImplementedInterfacesFixed(typeInfo);
        }

        [CanBeNull]
        private static Type FindRelatedGeneric(this Type toCheck, Type generic)
        {
            Type result = null;
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    result = toCheck;
                    break;
                }

                toCheck = toCheck.BaseType;
            }

            return result;
        }

        [NotNull]
        private static IEnumerable<Type> GetImplementedInterfacesFixed([NotNull] this TypeInfo typeInfo)
        {
            IEnumerable<Type> result;
            if (typeInfo.IsGenericTypeDefinition)
            {
                result = typeInfo.ImplementedInterfaces
                    .Select(t =>
                        t.GetTypeInfo().Assembly.GetType(t.Namespace + "." + t.Name))
                    .Where(t => t != null);
            }
            else
            {
                result = typeInfo.ImplementedInterfaces;
            }

            return result;
        }
    }
}
