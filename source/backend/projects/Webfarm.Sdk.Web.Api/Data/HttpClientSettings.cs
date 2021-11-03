namespace Webfarm.Sdk.Web.Api.Data
{
    using System;

    public class HttpClientSettings
    {
        public Uri Host { get; set; }

        public bool EnablePolicies { get; set; } = true;

        public int[] SleepDurations { get; set; }

        public int? TimeoutPerTrySeconds { get; set; }

        public int? TimeoutOverallSeconds { get; set; }

        public double? FailureThreshold { get; set; }

        public int? SamplingDurationSeconds { get; set; }

        public int? MinimumThroughput { get; set; }

        public int? DurationOfBreakSeconds { get; set; }
    }
}
