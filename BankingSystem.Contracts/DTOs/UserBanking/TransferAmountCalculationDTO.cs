namespace BankingSystem.Contracts.DTOs.UserBanking
{
    public class TransferAmountCalculationDTO
    {
        public decimal BankProfit { get; set; }
  
        public decimal AmountFromAccount { get; set; }
   
        public decimal AmountToAccount   { get; set; }
    }
}
