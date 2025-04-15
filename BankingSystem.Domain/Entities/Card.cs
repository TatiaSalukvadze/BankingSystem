namespace BankingSystem.Domain.Entities
{
    public class Card
    {
        public int Id { get; set; }

        public int AccountId { get; set; }

        public string CardNumber { get; set; }

        public string ExpirationDate { get; set; }

        public string CVV { get; set; }

        public string PIN { get; set; }
    }
}
