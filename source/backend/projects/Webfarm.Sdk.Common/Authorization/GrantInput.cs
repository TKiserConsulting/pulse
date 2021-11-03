namespace Webfarm.Sdk.Common.Authorization
{
    using System.Security.Principal;

    public class GrantInput
    {
        public IPrincipal Principal { get; set; }

        public GrantDescriptor[] Grants { get; set; }
    }
}
