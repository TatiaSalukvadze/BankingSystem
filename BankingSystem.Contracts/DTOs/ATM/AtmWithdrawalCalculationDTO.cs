using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Contracts.DTOs.ATM
{
    public class AtmWithdrawalCalculationDTO
    {
        [Required]
        public decimal BankProfit { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Currency { get; set; }

        [Required]
        public decimal TotalAmountToDeduct { get; set; }
    }
}
 