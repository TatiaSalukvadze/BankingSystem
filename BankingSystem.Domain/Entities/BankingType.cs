using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Domain.Entities
{
    public class BankingType
    {
        public int Id { get; set; }

        [Required]
        public string Type { get; set; }
    }
}
