using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Contracts.DTOs.UserBanking
{
    public class IncomeExpenseDTO
    {
        public Dictionary<string, decimal> Income { get; set; }

        public Dictionary<string, decimal> Expense { get; set; }
    }
}
