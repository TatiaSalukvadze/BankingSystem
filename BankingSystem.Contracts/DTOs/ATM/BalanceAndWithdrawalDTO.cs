using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Contracts.DTOs.ATM
{
    public class BalanceAndWithdrawalDTO
    {
        [Required]
        public decimal Amount { get; set; }

        [Required]
        public decimal WithdrawnAmountIn24Hours { get; set; }

        [Required]
        public string Currency { get; set; }
    }
}
