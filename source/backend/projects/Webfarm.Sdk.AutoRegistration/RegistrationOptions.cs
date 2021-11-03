namespace Webfarm.Sdk.AutoRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DryIoc;
    using JetBrains.Annotations;
    using Webfarm.Sdk.Reflection;

    public class RegistrationOptions : IFluentRegistration
    {
        private Type type;
        private Func<Type, IEnumerable<Type>> interfacesToRegisterAsResolver = t => new List<Type>(t.GetImplementedInterfacesFixed());
        private Func<Type, string> nameToRegisterWithResolver = t => null;
        private Func<Type, Type> serviceToRegisterWithResolver = t => t;

        public string Name
        {
            get => this.nameToRegisterWithResolver(this.type);
            set => this.nameToRegisterWithResolver = t => value;
        }

        public IEnumerable<Type> Interfaces
        {
            get => this.interfacesToRegisterAsResolver(this.type);
            set => this.interfacesToRegisterAsResolver = t => value;
        }

        public Type Type
        {
            get => this.type;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                this.type = value;
            }
        }

        public Type Implementation => this.serviceToRegisterWithResolver(this.type);

        public IReuse Reuse { get; set; } = DryIoc.Reuse.Transient;

        [NotNull]
        public IFluentRegistration WithName([NotNull] string name)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            return this;
        }

        [NotNull]
        public IFluentRegistration WithReuse([NotNull] IReuse reuse)
        {
            this.Reuse = reuse ?? throw new ArgumentNullException(nameof(reuse));
            return this;
        }

        [NotNull]
        public IFluentRegistration WithName([NotNull] Func<Type, string> nameResolver)
        {
            this.nameToRegisterWithResolver = nameResolver ?? throw new ArgumentNullException(nameof(nameResolver));
            return this;
        }

        [NotNull]
        public IFluentRegistration WithTypeName()
        {
            this.nameToRegisterWithResolver = t => t.Name;
            return this;
        }

        [NotNull]
        public IFluentRegistration WithPartName(string name)
        {
            this.nameToRegisterWithResolver = t =>
                                              {
                                                  var typeName = t.Name;
                                                  return typeName.EndsWith(name, StringComparison.OrdinalIgnoreCase)
                                                      ? typeName.Remove(typeName.Length - name.Length)
                                                      : typeName;
                                              };
            return this;
        }

        [NotNull]
        public IFluentRegistration As<TContact>()
            where TContact : class
        {
            this.interfacesToRegisterAsResolver = t => new List<Type> { typeof(TContact) };
            return this;
        }

        [NotNull]
        public IFluentRegistration As([NotNull] Func<Type, Type> typeResolver)
        {
            if (typeResolver == null)
            {
                throw new ArgumentNullException(nameof(typeResolver));
            }

            this.interfacesToRegisterAsResolver = t => new List<Type> { typeResolver(t) };
            return this;
        }

        [NotNull]
        public IFluentRegistration As([NotNull] Func<Type, Type[]> typesResolver)
        {
            if (typesResolver == null)
            {
                throw new ArgumentNullException(nameof(typesResolver));
            }

            this.interfacesToRegisterAsResolver = t => new List<Type>(typesResolver(t));
            return this;
        }

        [NotNull]
        public IFluentRegistration AsFirstInterfaceOfType()
        {
            this.interfacesToRegisterAsResolver = t => new List<Type> { t.GetImplementedInterfacesFixed().First() };
            return this;
        }

        [NotNull]
        public IFluentRegistration AsSingleInterfaceOfType()
        {
            this.interfacesToRegisterAsResolver = t => new List<Type> { t.GetImplementedInterfacesFixed().Single() };
            return this;
        }

        [NotNull]
        public IFluentRegistration AsAllInterfacesOfType()
        {
            this.interfacesToRegisterAsResolver = t => new List<Type>(t.GetImplementedInterfacesFixed());
            return this;
        }

        [NotNull]
        public IFluentRegistration For(Func<Type, Type> typeResolver)
        {
            this.serviceToRegisterWithResolver = typeResolver;

            return this;
        }
    }
}
