namespace Webfarm.Sdk.AutoRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using DryIoc;
    using JetBrains.Annotations;

    internal class AutoRegistrationBuilder : IAutoRegistration
    {
        private readonly IRegistrator container;
        private readonly List<Assembly> includedAssemblies = new List<Assembly>();
        private readonly List<Predicate<Assembly>> excludedAssemblyFilters = new List<Predicate<Assembly>>();
        private readonly List<Predicate<Type>> excludedTypeFilters = new List<Predicate<Type>>();
        private readonly List<RegistrationEntry> registrationEntries = new List<RegistrationEntry>();

        public AutoRegistrationBuilder(IRegistrator container)
        {
            this.container = container;
        }

        [NotNull]
        public IAutoRegistration Include(
            [NotNull] Predicate<Type> typeFilter,
            [NotNull] Action<Type> registrator)
        {
            if (typeFilter == null)
            {
                throw new ArgumentNullException(nameof(typeFilter));
            }

            if (registrator == null)
            {
                throw new ArgumentNullException(nameof(registrator));
            }

            this.registrationEntries.Add(new RegistrationEntry(typeFilter, registrator));
            return this;
        }

        [NotNull]
        public IAutoRegistration IncludeAssembly([NotNull] Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            this.includedAssemblies.Add(assembly);

            return this;
        }

        [NotNull]
        public IAutoRegistration ExcludeAssemblies([NotNull] Predicate<Assembly> filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            this.excludedAssemblyFilters.Add(filter);
            return this;
        }

        [NotNull]
        public IAutoRegistration Exclude([NotNull] Predicate<Type> filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            this.excludedTypeFilters.Add(filter);
            return this;
        }

        [NotNull]
        public IAutoRegistration Include(
            [NotNull] Predicate<Type> typeFilter,
            [NotNull] IRegistrationOptions registrationOptions)
        {
            if (typeFilter == null)
            {
                throw new ArgumentNullException(nameof(typeFilter));
            }

            if (registrationOptions == null)
            {
                throw new ArgumentNullException(nameof(registrationOptions));
            }

            var registrationEntry = new RegistrationEntry(
                typeFilter,
                t =>
                {
                    registrationOptions.Type = t;

                    var serviceType = registrationOptions.Implementation;
                    foreach (var contractType in registrationOptions.Interfaces)
                    {
                        this.container
                            .Register(
                                new ReflectionFactory(serviceType, registrationOptions.Reuse),
                                contractType,
                                registrationOptions.Name,
                                null,
                                true);
                    }
                });
            this.registrationEntries.Add(registrationEntry);
            return this;
        }

        public void ApplyAutoRegistration()
        {
            if (this.registrationEntries.Any())
            {
                var assemblies = this.includedAssemblies.Count > 0
                    ? this.includedAssemblies.ToArray()
                    : AppDomain.CurrentDomain.GetAssemblies();

                foreach (var type in assemblies
                    .Where(a => !this.excludedAssemblyFilters.Any(f => f(a)))
                    .SelectMany(a => a.GetTypes())
                    .Where(t => !this.excludedTypeFilters.Any(f => f(t))))
                {
                    foreach (var entry in this.registrationEntries)
                    {
                        entry.RegisterIfSatisfiesFilter(type);
                    }
                }
            }
        }

        private class RegistrationEntry
        {
            private readonly Predicate<Type> typeFilter;
            private readonly Action<Type> registrator;

            public RegistrationEntry(
                Predicate<Type> typeFilter,
                Action<Type> registrator)
            {
                this.typeFilter = typeFilter;
                this.registrator = registrator;
            }

            public void RegisterIfSatisfiesFilter(Type type)
            {
                if (this.typeFilter(type))
                {
                    this.registrator(type);
                }
            }
        }
    }
}
