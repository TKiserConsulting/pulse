namespace Webfarm.Sdk.Common.Authorization
{
    public class GrantResult
    {
        public bool Allow { get; set; }

        public GrantAvailabilityData[] GrantsAvailability { get; set; }
    }
}
