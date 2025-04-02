namespace BankingSystem.Contracts.DTOs.ATM
{
    public class AtmWithdrawalCalculationDTO
    {
        public decimal BankProfit { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public decimal TotalAmountToDeduct { get; set; }
    }
}
