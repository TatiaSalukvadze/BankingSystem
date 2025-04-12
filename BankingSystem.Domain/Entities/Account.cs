
namespace BankingSystem.Domain.Entities
{
    public class Account
    {
        public int Id { get; set; }

        public int PersonId { get; set; }
        
        public string IBAN { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }
    }
}
