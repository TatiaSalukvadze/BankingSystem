using BankingSystem.Domain.Enums;

namespace BankingSystem.Contracts.DTOs
{
    public class AtmWithdrawDTO
    {
        public CurrencyType Currency { get; set; } 
        public decimal TotalWithdrawnAmount { get; set; }
    }
}
