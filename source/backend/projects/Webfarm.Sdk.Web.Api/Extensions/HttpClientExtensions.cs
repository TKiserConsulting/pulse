namespace Webfarm.Sdk.Web.Api.Extensions
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Polly;
    using Polly.CircuitBreaker;
    using Polly.Extensions.Http;
    using Polly.Retry;
    using Polly.Timeout;
    using Serilog;
    using Webfarm.Sdk.Web.Api.Data;

    public static class HttpClientExtensions
    {
        private static readonly int[] DefaultRetrySleepDuration = { 1, 5, 15 };

        private static readonly TimeSpan DefaultTryTimeout = TimeSpan.FromSeconds(30);

        private static readonly TimeSpan DefaultRequestTimeout = TimeSpan.FromSeconds(100);

        public static void AddHttpClientDefaults<TClient, TSettings>(
            this IServiceCollection services,
            [NotNull] IConfiguration configuration,
            string clientSettingsKey,
            [CanBeNull] Action<HttpClient, TSettings> configureClient = null,
            [CanBeNull] Action<IHttpClientBuilder> configureBuilder = null)
            where TClient : class
            where TSettings : HttpClientSettings
        {
            var clientSettings =
                configuration.GetSection(clientSettingsKey).Get<TSettings>();

            if (clientSettings?.Host != null)
            {
                var builder = services.AddHttpClient<TClient>(client =>
                {
                    client.BaseAddress = clientSettings.Host;
                    client.DefaultRequestHeaders.Accept
                        .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.Timeout = clientSettings.TimeoutOverallSeconds.HasValue
                        ? TimeSpan.FromSeconds(clientSettings.TimeoutOverallSeconds.Value)
                        : DefaultRequestTimeout;

                    configureClient?.Invoke(client, clientSettings);
                });

                configureBuilder?.Invoke(builder);

                builder.AddDefaultPolicies(clientSettings);
            }
        }

        private static void AddDefaultPolicies(this IHttpClientBuilder builder, [NotNull] HttpClientSettings clientSettings)
        {
            if (clientSettings.EnablePolicies)
            {
                var retryPolicy = CreateRetryPolicy(clientSettings);

                var circuitBreakerPolicy = CreateCircuitBreakerPolicy(clientSettings);

                var timeoutPolicy = CreateTimeoutPolicy(clientSettings);

                builder
                    .AddPolicyHandler(retryPolicy)
                    .AddPolicyHandler(circuitBreakerPolicy)
                    .AddPolicyHandler(timeoutPolicy)
                    ;
            }
        }

        private static AsyncRetryPolicy<HttpResponseMessage> CreateRetryPolicy([NotNull] HttpClientSettings clientSettings)
        {
            var sleepDurations =
                (clientSettings.SleepDurations ?? DefaultRetrySleepDuration).Select(duration => TimeSpan.FromSeconds(duration));
            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutRejectedException>()
                .WaitAndRetryAsync(
                    sleepDurations,
                    (outcome, timespan, retryAttempt, context) =>
                    {
                        Log.Logger.Error(
                            outcome?.Exception,
                            $"Request: {outcome?.Result?.RequestMessage?.RequestUri} failed with status: {outcome?.Result?.StatusCode} and message: {outcome?.Result?.ReasonPhrase}. Delaying for {timespan.TotalMilliseconds}ms, then making retry {retryAttempt}.");
                    });
            return retryPolicy;
        }

        private static AsyncTimeoutPolicy<HttpResponseMessage> CreateTimeoutPolicy([NotNull] HttpClientSettings clientSettings)
        {
            var callTimeout = clientSettings.TimeoutPerTrySeconds.HasValue
                ? TimeSpan.FromSeconds(clientSettings.TimeoutPerTrySeconds.Value)
                : DefaultTryTimeout;
            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(
                callTimeout,
                (context, timeSpan, task) =>
                {
                    Log.Logger.Warning($"Request timeout after {timeSpan.TotalMilliseconds}ms.");
                    return Task.CompletedTask;
                });
            return timeoutPolicy;
        }

        private static AsyncCircuitBreakerPolicy<HttpResponseMessage> CreateCircuitBreakerPolicy([NotNull] HttpClientSettings clientSettings)
        {
            var failureThreshold = clientSettings.FailureThreshold ?? 0.5;
            var samplingDuration = clientSettings.SamplingDurationSeconds.HasValue
                ? TimeSpan.FromSeconds(clientSettings.SamplingDurationSeconds.Value)
                : TimeSpan.FromSeconds(50);
            var minimumThroughput = clientSettings.MinimumThroughput ?? 20;
            var durationOfBreak = clientSettings.DurationOfBreakSeconds.HasValue
                ? TimeSpan.FromSeconds(clientSettings.DurationOfBreakSeconds.Value)
                : TimeSpan.FromSeconds(30);

            var circuitBreakerPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutRejectedException>()
                .OrResult(r => r.StatusCode == (HttpStatusCode)429)
                .AdvancedCircuitBreakerAsync(
                    failureThreshold,
                    samplingDuration,
                    minimumThroughput,
                    durationOfBreak,
                    (result, state, timeSpan, context) =>
                    {
                        Log.Logger.Information($"Breaking circuit breaker. State: {state:G}, TimeSpan: {timeSpan}.");
                    },
                    context =>
                    {
                        Log.Logger.Information("Resetting circuit breaker.");
                    },
                    () =>
                    {
                        Log.Logger.Information("Half opening circuit breaker.");
                    });
            return circuitBreakerPolicy;
        }
    }
}
