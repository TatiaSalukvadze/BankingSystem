using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Contracts.DTOs.UserBanking
{
    public class TransferAmountCalculationDTO
    {
        [Required]
        public decimal BankProfit { get; set; }
        [Required]
        public decimal AmountFromAccount { get; set; }
        [Required]
        public decimal AmountToAccount   { get; set; }
    }
}
