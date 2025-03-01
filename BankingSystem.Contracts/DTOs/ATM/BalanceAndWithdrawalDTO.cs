using BankingSystem.Domain.Enums;

namespace BankingSystem.Contracts.DTOs.ATM
{
    public class BalanceAndWithdrawalDTO
    {
        public decimal Amount { get; set; }
        public decimal WithdrawnAmountIn24Hours { get; set; }
        public CurrencyType Currency { get; set; }
    }
}
