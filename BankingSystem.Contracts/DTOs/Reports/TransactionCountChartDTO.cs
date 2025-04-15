namespace BankingSystem.Contracts.DTOs.Report
{
    public class TransactionCountChartDTO
    {
        public DateOnly Date { get; set; }

        public int Count { get; set; }
    }
}
