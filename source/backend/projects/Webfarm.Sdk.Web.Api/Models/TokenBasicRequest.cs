namespace Infoplus.Askod.Sdk.Web.Api.Models
{
    using System.ComponentModel.DataAnnotations;

    // todo [as]: move to auth
    public class TokenBasicRequest
    {
        [Required]
        [StringLength(256, MinimumLength = 1)]
        public string Username { get; set; }

        [Required]
        [StringLength(4096, MinimumLength = 1)]
        public string Password { get; set; }
    }
}
