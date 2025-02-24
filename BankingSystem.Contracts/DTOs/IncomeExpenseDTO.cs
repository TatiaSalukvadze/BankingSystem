using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Contracts.DTOs
{
    public class IncomeExpenseDTO
    {
        public Dictionary<string,decimal> Income { get; set; }
        public Dictionary<string, decimal> Expense { get; set; }
    }
}
