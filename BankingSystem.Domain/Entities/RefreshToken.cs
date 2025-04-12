using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Domain.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }

        public string IdentityUserId { get; set; }

        public string Token { get; set; }

        public DateTime ExpirationDate { get; set; }

        public string DeviceId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
