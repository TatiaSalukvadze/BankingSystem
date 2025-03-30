using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Domain.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }

        [Required]
        public string IdentityUserId { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public DateTime ExpirationDate { get; set; }

        [Required]
        public string DeviceId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
