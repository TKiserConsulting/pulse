namespace Webfarm.Sdk.Web.Api.Extensions
{
    using System;
    using System.Collections.Generic;
    using DryIoc;
    using Microsoft.Extensions.Logging;
    using Serilog;
    using Serilog.Extensions.Logging;
    using Webfarm.Sdk.Web.Api.Exceptions;

    public static class RegistrationExtensions
    {
        public static void RegisterLogging(this IRegistrator container)
        {
            IEnumerable<ILoggerProvider> loggerProviders = new ILoggerProvider[]
            {
                new SerilogLoggerProvider(Log.Logger)
            };
            container.RegisterDelegate<ILoggerFactory>(r => new LoggerFactory(loggerProviders), Reuse.Singleton);

            container.Register(typeof(ILogger<>), typeof(Logger<>), Reuse.Singleton);
            container.Register(
                Made.Of<Microsoft.Extensions.Logging.ILogger>(() =>
                    new Logger<ErrorHandlingService>(Arg.Of<ILoggerFactory>())),
                Reuse.Transient,
                Setup.With(condition: r => r.Parent.ImplementationType == null));
            container.Register(
                Made.Of(() => Arg.Of<ILoggerFactory>().CreateLogger(Arg.Index<Type>(0)), r => r.Parent.ImplementationType),
                Reuse.Transient,
                Setup.With(condition: r => r.Parent.ImplementationType != null));

            container.Register(
                Made.Of(() => Log.Logger),
                Reuse.Transient,
                Setup.With(condition: r => r.Parent.ImplementationType == null));
            container.Register(
                Made.Of(() => Log.ForContext(Arg.Index<Type>(0)), r => r.Parent.ImplementationType),
                Reuse.Transient,
                Setup.With(condition: r => r.Parent.ImplementationType != null));
        }
    }
}
