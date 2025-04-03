using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Domain.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }

        [Required]
        public string IdentityUserId { get; set; }

        [Required]
        [MaxLength(450)]
        public string Token { get; set; }

        [Required]
        public DateTime ExpirationDate { get; set; }

        [Required]
        [MaxLength(450)]
        public string DeviceId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
