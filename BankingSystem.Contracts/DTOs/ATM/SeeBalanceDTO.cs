using BankingSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Contracts.DTOs.ATM
{
    public class SeeBalanceDTO
    {
        [Required]
        public decimal Amount { get; set; }

        [Required]
        public CurrencyType Currency { get; set; }
    }
}
