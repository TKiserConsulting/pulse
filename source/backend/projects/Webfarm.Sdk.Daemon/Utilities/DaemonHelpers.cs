namespace Webfarm.Sdk.Daemon.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using DryIoc;
    using DryIoc.MefAttributedModel;
    using DryIoc.Microsoft.DependencyInjection;
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Serilog;

    public static class DaemonHelpers
    {
        [NotNull]
        public static Task<int> RunAsConsoleDaemonAsync<TCompositionRoot, TWebHostStartup>(
            [NotNull] string[] args,
            Action<IHostBuilder> configure = null,
            CancellationToken cancellationToken = default)
            where TCompositionRoot : class
        {
            return RunAsConsoleDaemonAsync<TCompositionRoot>(args, typeof(TWebHostStartup), configure, cancellationToken);
        }

        [NotNull]
        public static async Task<int> RunAsConsoleDaemonAsync<TCompositionRoot>(
            [NotNull] string[] args,
            [CanBeNull] Type webHostStartupType = null,
            Action<IHostBuilder> configure = null,
            CancellationToken cancellationToken = default)
            where TCompositionRoot : class
        {
            try
            {
                var hostBuilder = ConfigureAsConsoleDaemon<TCompositionRoot>(args, webHostStartupType)
                    .UseRootSerilog(args, false);
                configure?.Invoke(hostBuilder);
                await hostBuilder.RunConsoleAsync(cancellationToken);

                return 0;
            }
            catch (Exception ex)
            {
                Log.Logger.Fatal(ex, "Unexpected error");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        [NotNull]
        public static IHostBuilder ConfigureAsConsoleDaemon<TCompositionRoot>(
            [NotNull] string[] args,
            [CanBeNull] Type webHostStartupType = null)
            where TCompositionRoot : class
        {
            #if DEBUG
            Serilog.Debugging.SelfLog.Enable(Console.Error);
            #endif

            return Host
                .CreateDefaultBuilder(args)
                .ConditionalConfigureWebHostDefaults(webHostStartupType)
                .ConfigureDefaultAppConfiguration(args)
                .ConfigureConsoleAppConfiguration()
                .ConfigureDefaultContainer<TCompositionRoot>();
        }

        [NotNull]
        public static string ShortApplicationName([NotNull] this IHostEnvironment hostEnvironment)
        {
            var name = ShortApplicationName(hostEnvironment.ApplicationName);

            return name;
        }

        [NotNull]
        public static string ShortApplicationName(string name)
        {
            name = (name ?? string.Empty)
                .Replace("Pulse", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("Webfarm", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("Daemon", string.Empty, StringComparison.InvariantCulture)
                .Replace("Persistence", string.Empty, StringComparison.InvariantCulture)
                .Replace("Web", string.Empty, StringComparison.InvariantCulture)
                ;
            name = Regex
                .Replace(name, "([A-Z][a-z])", "-$1")
                .Replace(".-", ".", StringComparison.InvariantCulture)
                .Replace("..", ".", StringComparison.InvariantCulture);

            #pragma warning disable CA1308 // Normalize strings to uppercase
            name = name
                .Trim('-', '.')
                .ToLower(CultureInfo.InvariantCulture);
            #pragma warning restore CA1308 // Normalize strings to uppercase

            return name;
        }

        /// <summary>
        /// allows to use initialized logged inside ConfigureServices and\or when
        /// ConfigureServices failed and we want to structure log bootstrap error
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="args"></param>
        /// <param name="lazy"></param>
        /// <returns></returns>
        [NotNull]
        public static IHostBuilder UseRootSerilog(
            [NotNull] this IHostBuilder builder,
            string[] args,
            bool lazy = true)
        {
            void ConfigureDelegate(HostBuilderContext hostBuilderContext, LoggerConfiguration loggerConfiguration)
            {
                var configuration = hostBuilderContext.HostingEnvironment.BuildSerilogLoggerConfiguration(args);
                loggerConfiguration.ReadFrom.Configuration(configuration);
            }

            if (lazy)
            {
                // logger configured after Startup.ConfigureServices
                // and does not require manual Dispose
                builder.UseSerilog(ConfigureDelegate);
            }
            else
            {
                // logger configured before Startup.ConfigureServices
                // and requires manual Dispose
                builder.ConfigureAppConfiguration((hostBuilderContext, cb) =>
                {
                    var loggerConfiguration = new LoggerConfiguration();
                    ConfigureDelegate(hostBuilderContext, loggerConfiguration);

                    Log.Logger = loggerConfiguration.CreateLogger();

                    // ReSharper disable once RedundantArgumentDefaultValue
                    builder.UseSerilog(Log.Logger, false);
                });
            }

            return builder;
        }

        [NotNull]
        private static IHostBuilder ConditionalConfigureWebHostDefaults(
            [NotNull] this IHostBuilder hostBuilder,
            [CanBeNull] Type webHostStartupType)
        {
            if (webHostStartupType != null)
            {
                hostBuilder.ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup(webHostStartupType);

                    var port = Environment.GetEnvironmentVariable("PORT");
                    if (!string.IsNullOrEmpty(port))
                    {
                        webBuilder.UseUrls("http://*:" + port);
                    }
                });
            }

            return hostBuilder;
        }

        [NotNull]
        private static IHostBuilder ConfigureDefaultAppConfiguration([NotNull] this IHostBuilder builder, string[] args)
        {
            return builder.ConfigureAppConfiguration((hostBuilderContext, config) =>
            {
                var environmentName = hostBuilderContext.HostingEnvironment.EnvironmentName;
                var environmentVariablePrefix = hostBuilderContext.HostingEnvironment.EnvironmentVariablePrefix();
                var environmentVariableAppPrefix = hostBuilderContext.HostingEnvironment.EnvironmentVariableAppPrefix();

                // in case of clear you should deal with URS while local development
                // config.Sources.Clear();
                config
                    .AddJsonFile("appsettings.json", true, false)
                    .AddJsonFile($"appsettings.{environmentName}.json", true, false)
                    .AddJsonFile($"appsettings.{Environment.MachineName}.json", true, false)
                    .AddKeyPerFile(Path.GetFullPath("settings"), true)
                    .AddEnvironmentVariables()
                    .AddEnvironmentVariables(environmentVariablePrefix)
                    .AddEnvironmentVariables(environmentVariableAppPrefix)
                    .AddCommandLine(args);
            });
        }

        [NotNull]
        private static IHostBuilder ConfigureConsoleAppConfiguration([NotNull] this IHostBuilder builder)
        {
            return builder.ConfigureAppConfiguration((hostBuilderContext, config) =>
            {
                var applicationName = hostBuilderContext.HostingEnvironment.ShortApplicationName();
                Console.Title = applicationName;
                Console.OutputEncoding = Encoding.UTF8;

                // allows to use %ApplicationName% placeholder for file logger inside appsettings.Development.json
                Environment.SetEnvironmentVariable("ApplicationName", applicationName);
            });
        }

        [NotNull]
        private static IHostBuilder ConfigureDefaultContainer<TCompositionRoot>([NotNull] this IHostBuilder builder)
            where TCompositionRoot : class
        {
            #pragma warning disable CA2000 // Dispose objects before losing scope
            var rootContainer = new Container(
                rules => rules
                    .WithConcreteTypeDynamicRegistrations(reuse: Reuse.Transient)
                    .WithMefAttributedModel());

            #pragma warning restore CA2000 // Dispose objects before losing scope

            var factory = new DryIocServiceProviderFactory(rootContainer);

            builder
                .UseServiceProviderFactory(factory)
                .ConfigureContainer<IContainer>((hostBuilderContext, container) =>
                {
                    container.WithCompositionRoot<TCompositionRoot>();
                });

            return builder;
        }

        private static IConfigurationRoot BuildSerilogLoggerConfiguration(
            [NotNull] this IHostEnvironment hostEnvironment,
            string[] args)
        {
            var applicationName = hostEnvironment.ShortApplicationName();
            var environmentName = hostEnvironment.EnvironmentName;
            var environmentVariablePrefix = hostEnvironment.EnvironmentVariablePrefix();
            var environmentVariableAppPrefix = hostEnvironment.EnvironmentVariableAppPrefix();

            var loggerConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Serilog:Properties:Application", applicationName }
                })
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile($"appsettings.{Environment.MachineName}.json", true)
                .AddJsonFile($"appsettings.{environmentName}.json", true)
                .AddEnvironmentVariables()
                .AddEnvironmentVariables(environmentVariablePrefix)
                .AddEnvironmentVariables(environmentVariableAppPrefix)
                .AddCommandLine(args)
                .Build();

            return loggerConfig;
        }

        // ReSharper disable once UnusedParameter.Local
        [NotNull]
        #pragma warning disable IDE0060 // Remove unused parameter
        private static string EnvironmentVariablePrefix([CanBeNull] this IHostEnvironment hostEnvironment)
        #pragma warning restore IDE0060 // Remove unused parameter
        {
            return "PULSE_";
        }

        [NotNull]
        private static string EnvironmentVariableAppPrefix([NotNull] this IHostEnvironment hostEnvironment)
        {
            var normalizedApplicationName = hostEnvironment.ShortApplicationName()
                .Replace("-", "_", StringComparison.OrdinalIgnoreCase)
                .Replace(".", "_", StringComparison.OrdinalIgnoreCase)
                .ToUpperInvariant();
            var environmentVariableAppPrefix =
                $"{hostEnvironment.EnvironmentVariablePrefix()}{normalizedApplicationName}_";
            return environmentVariableAppPrefix;
        }
    }
}
