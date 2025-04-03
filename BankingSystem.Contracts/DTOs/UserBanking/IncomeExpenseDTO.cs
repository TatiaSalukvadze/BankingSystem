using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Contracts.DTOs.UserBanking
{
    public class IncomeExpenseDTO
    {
        [Required]
        public Dictionary<string, decimal> Income { get; set; }

        [Required]
        public Dictionary<string, decimal> Expense { get; set; }
    }
}
