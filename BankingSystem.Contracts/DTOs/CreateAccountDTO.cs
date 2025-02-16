using BankingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BankingSystem.Contracts.DTOs
{
    public class CreateAccountDTO
    {
        [Required]
        public string IDNumber { get; set; }
        [Required]
        public string IBAN { get; set; }
        [Required]
        public Decimal Amount { get; set; }
        [Required]
        public CurrenctType Currency { get; set; }
    }
}
