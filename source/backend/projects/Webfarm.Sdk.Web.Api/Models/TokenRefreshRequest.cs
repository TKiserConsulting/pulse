namespace Infoplus.Askod.Sdk.Web.Api.Models
{
    using Newtonsoft.Json;

    // todo [as]: move to auth
    public class TokenRefreshRequest
    {
        [JsonProperty(PropertyName = "refresh_token")]
        public string RefreshToken { get; set; }

        public bool? Silent { get; set; }
    }
}
