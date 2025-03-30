using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
