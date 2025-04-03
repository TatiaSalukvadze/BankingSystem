using BankingSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Contracts.DTOs.ATM
{
    public class TotalAtmWithdrawalDTO
    {
        [Required]
        public decimal TotalWithdrawnAmount { get; set; }

        [Required]
        public CurrencyType Currency { get; set; }
    }
}
