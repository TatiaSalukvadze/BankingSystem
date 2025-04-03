using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Contracts.DTOs.Auth
{
    public class RefreshTokensDTO
    {
        [Required]
        public string AccessToken { get; set; }

        [Required]
        [MaxLength(450)]
        public string RefreshToken { get; set; }

        [Required]
        [MaxLength(450)]
        public string DeviceId { get; set; }
    }
}
