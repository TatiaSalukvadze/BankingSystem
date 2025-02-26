using BankingSystem.Domain.Enums;

namespace BankingSystem.Contracts.DTOs.ATM
{
    public class AtmWithdrawDTO
    {
        public CurrencyType Currency { get; set; }
        public decimal TotalWithdrawnAmount { get; set; }
    }
}
