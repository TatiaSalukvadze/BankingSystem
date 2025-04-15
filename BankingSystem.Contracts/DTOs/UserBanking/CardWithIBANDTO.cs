namespace BankingSystem.Contracts.DTOs.UserBanking
{
    public class CardWithIBANDTO
    {
        public string IBAN { get; set; }

         public string Name { get; set; }

        public string Surname { get; set; }

        public string CardNumber { get; set; }

        public string ExpirationDate { get; set; }

        public string CVV { get; set; }
    }
}
