using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Contracts.DTOs
{
    public class TransactionCountChartDTO
    {
        public DateOnly Date { get; set; }
        public int Count { get; set; }
    }
}
