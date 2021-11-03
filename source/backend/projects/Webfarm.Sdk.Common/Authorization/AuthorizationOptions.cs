namespace Webfarm.Sdk.Common.Authorization
{
    public class AuthorizationOptions
    {
        public string AuthorizationStrategy { get; set; } = AllowAllGrantManager.Key;
    }
}
