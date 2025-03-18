namespace BankingSystem.Contracts.DTOs.Report
{
    public class TransactionCountDTO
    {
        public int LastMonthCount { get; set; }
        public int LastSixMonthCount { get; set; }
        public int LastYearCount { get; set; }
    }
}
