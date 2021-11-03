using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using Infoplus.Askod.Web.Core.Api.Exceptions;
using Infoplus.Askod.Web.Core.Api.Exceptions.Handlers;
using Infoplus.Askod.Web.Core.Api.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Fluent;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration;
using Newtonsoft.Json;
using Serilog;

namespace Infoplus.Askod.Web.Core.Api
{
    using Infoplus.Askod.Business.Impl.Exceptions;

    using Microsoft.Extensions.DependencyInjection;

    public class Bootstrap
    {
        #region Fields

        protected readonly NameValueCollection DefaultHandlingSettings = new NameValueCollection
        {
            {
                WrapHandler<Exception>.MessageKey,
                "Unexpected error occured while processing your request. Please, contact administrator for details. [Error ID: {handlingInstanceID}]"
            },
            {
                WrapHandler<Exception>.AppendHandlingIdKey,
                bool.TrueString
            }
        };

        protected readonly NameValueCollection TraceLevelSettings =
            new NameValueCollection {{ApiLogWriteHandler.LevelKey, TraceLevel.Verbose.ToString("G")}};

        protected readonly NameValueCollection InfoLevelSettings =
            new NameValueCollection {{ApiLogWriteHandler.LevelKey, TraceLevel.Info.ToString("G")}};

        protected readonly NameValueCollection WarnLevelSettings =
            new NameValueCollection {{ApiLogWriteHandler.LevelKey, TraceLevel.Warning.ToString("G")}};

        protected readonly NameValueCollection FatalLevelSettings =
            new NameValueCollection {{ApiLogWriteHandler.LevelKey, TraceLevel.Error.ToString("G")}};

        private ExceptionManager exceptionManager;

        #endregion

        public virtual void RunConfiguration(IApplicationBuilder app, IHostingEnvironment env,
            ILoggerFactory loggerFactory)
        {
            //this.ConfigureLogging(app, env, loggerFactory);

            this.ConfigureExceptionHandling(app);

            //this.ConfigureGlobals(app);

            //this.ConfigureAuth(app);

            //this.ConfigureWebApi(app);
        }

        #region Logging

        protected virtual void ConfigureLogging(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            /*
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                //.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                //.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile($"serilog.json");
            var configurationRoot = builder.Build();

            // todo implement proper way to write to the file
            SelfLog.Enable(s => File.WriteAllText("Logs\\api.elastic.failure.txt", $"SERILOG ERROR: {s}"));

            // todo register in container
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configurationRoot)
                .CreateLogger();

            loggerFactory.AddSerilog(dispose: true);
            
            Log.Information("Api logger has been configured");

            ApiLogWriteHandler.Logger = Log.Logger; 
            */
        }

        #endregion

        #region ExceptionHandling

        private ExceptionManager BuildExceptionManager()
        {
            var policies = this.GetExceptionPolicyDataList().Select(data => data.BuildExceptionPolicy());
            var manager = new ExceptionManager(policies);
            return manager;
        }

        public const string DefaultExceptionPolicy = "Communication.Json";

        public const string TransactionRollbackExceptionPolicy = "Communication.Rollback";

        private void ConfigureExceptionHandling(IApplicationBuilder app)
        {
            //OperationErrorExtensions.DefaultIncludeErrorDetailPolicy = this.IncludeErrorDetailPolicy;
            //OperationErrorExtensions.DefaultResourceProvider = this.ResourceProvider;

            this.exceptionManager = this.BuildExceptionManager();

            app.UseMiddleware(typeof(ErrorHandlingMiddleware), this.exceptionManager);
        }

        private IEnumerable<ExceptionPolicyData> GetExceptionPolicyDataList()
        {
            using (var configurationSource = new DictionaryConfigurationSource())
            {
                var builder = new ConfigurationSourceBuilder();

                // ReSharper disable RedundantAssignment
                var policyBuilder = builder.ConfigureExceptionHandling()
                    .GivenPolicyWithName(DefaultExceptionPolicy);
                policyBuilder = this.ConfigureDefaultPolicyExceptionHandling(policyBuilder);

                policyBuilder = policyBuilder
                    .GivenPolicyWithName(TransactionRollbackExceptionPolicy);
                policyBuilder = this.ConfigureTransactionPolicyExceptionHandling(policyBuilder);
                // ReSharper restore RedundantAssignment

                builder.UpdateConfigurationWithReplace(configurationSource);

                return ExceptionHandlingSettings.GetExceptionHandlingSettings(configurationSource).ExceptionPolicies;
            }
        }

        protected virtual IExceptionConfigurationForExceptionType ConfigureDefaultPolicyExceptionHandling(
            IExceptionConfigurationForExceptionType policyBuilder)
        {
            policyBuilder = policyBuilder
                .LogThenThrowNewException<UnauthorizedApiAccessException,
                    AccessDeniedExceptionHandler<UnauthorizedApiAccessException>>(WarnLevelSettings,
                    DefaultHandlingSettings)
                .LogThenThrowNewException<ValidationBusinessException, ValidationExceptionHandler>(TraceLevelSettings)
                .LogThenThrowNewException<BusinessException, BusinessExceptionHandler>(TraceLevelSettings)
                .LogThenThrowNewException<ObjectNotFoundApiException, ObjectNotFoundExceptionHandler>(InfoLevelSettings)
                .LogThenThrowNewException<SecurityException, AccessDeniedExceptionHandler>(this.WarnLevelSettings,
                    this.DefaultHandlingSettings)
                .LogThenThrowNewException<Exception, UnexpectedExceptionHandler>(this.FatalLevelSettings,
                    this.DefaultHandlingSettings);
            return policyBuilder;
        }

        protected virtual IExceptionConfigurationForExceptionType ConfigureTransactionPolicyExceptionHandling(
            IExceptionConfigurationForExceptionType policyBuilder)
        {
            return policyBuilder
                .LogThenThrowNewException<Exception, UnexpectedExceptionHandler>(this.FatalLevelSettings,
                    this.DefaultHandlingSettings);
        }

        #endregion

        public class ErrorHandlingMiddleware
        {
            private readonly RequestDelegate next;

            private readonly ExceptionManager exceptionManager;

            public ErrorHandlingMiddleware(RequestDelegate next,
                ExceptionManager exceptionManager)
            {
                this.next = next;
                this.exceptionManager = exceptionManager;
            }

            public async Task Invoke(HttpContext context /* other dependencies */)
            {
                try
                {
                    await next(context);
                }
                catch (Exception ex)
                {
                    await this.HandleExceptionAsync(context, ex);
                }
            }

            private Task HandleExceptionAsync(HttpContext context, Exception exception)
            {
                var code = HttpStatusCode.InternalServerError; // 500 if unexpected

                var transformedEception =
                    this.exceptionManager.TransformException(exception, DefaultExceptionPolicy);

                object response;
                if (transformedEception is OperationErrorApiException exc)
                {
                    code = exc.Error.Status;
                    response = exc.Error;
                }
                else
                {
                    response = exception;
                }

                context.Response.StatusCode = (int) code;
                context.Response.ContentType = "application/json";
                var result = response.Serialize();

                return context.Response.WriteAsync(result);
            }
        }
    }
}
