using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Contracts.DTOs.Auth
{
    public class LogoutDTO
    {
        [Required]
        public string RefreshToken { get; set; }

        [Required]
        public string DeviceId { get; set; }
    }
}
