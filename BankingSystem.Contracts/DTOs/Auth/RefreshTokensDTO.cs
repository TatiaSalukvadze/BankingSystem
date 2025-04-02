using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Contracts.DTOs.Auth
{
    public class RefreshTokensDTO
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public string RefreshToken { get; set; }

        [Required]
        public string DeviceId { get; set; }
    }
}
