namespace Pulse.Data.Auth
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class IdentityRefreshToken
    {
        public IdentityRefreshToken()
        {
            this.Created = DateTimeOffset.Now;
        }

        [NotMapped]
        public Guid Id
        {
            get => this.IdentityRefreshTokenId;
            set => this.IdentityRefreshTokenId = value;
        }

        [Key]
        public Guid IdentityRefreshTokenId { get; set; }

        [Required]
        public Guid ApplicationUserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Token { get; set; }

        public DateTimeOffset Created { get; set; }

        [MaxLength(100)]
        public string CreatedBy { get; set; }

        [Required]
        public DateTimeOffset Expired { get; set; }

        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser User { get; set; }
    }
}
