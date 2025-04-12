
namespace BankingSystem.Domain.Entities
{
    public class TransactionDetails
    {
        public int Id { get; set; }

        public decimal BankProfit { get; set; }

        public decimal Amount { get; set; }

        public int FromAccountId { get; set; }

        public int ToAccountId { get; set; }

        public string Currency { get; set; }

        public bool IsATM { get; set; }

        public DateTime PerformedAt { get; set; }
    }
}
