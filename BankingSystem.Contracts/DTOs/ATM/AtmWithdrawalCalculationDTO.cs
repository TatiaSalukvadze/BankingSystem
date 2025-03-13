namespace BankingSystem.Contracts.DTOs.ATM
{
    public class AtmWithdrawalCalculationDTO
    {
        public decimal Fee { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; }
        public decimal TotalAmountToDeduct { get; set; }
    }
}
