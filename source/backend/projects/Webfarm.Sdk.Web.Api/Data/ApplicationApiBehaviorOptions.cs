namespace Webfarm.Sdk.Web.Api.Data
{
    public class ApplicationApiBehaviorOptions
    {
        public ApplicationApiBehaviorOptions()
        {
            this.IncludeErrorDetailPolicy = IncludeErrorDetailPolicyType.Omit;
        }

        public IncludeErrorDetailPolicyType IncludeErrorDetailPolicy { get; set; }
    }
}
