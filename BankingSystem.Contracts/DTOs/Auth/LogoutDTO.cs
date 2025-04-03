using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Contracts.DTOs.Auth
{
    public class LogoutDTO
    {
        [Required]
        [MaxLength(450)]
        public string RefreshToken { get; set; }

        [Required]
        [MaxLength(450)]
        public string DeviceId { get; set; }
    }
}
