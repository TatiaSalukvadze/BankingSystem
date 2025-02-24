using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Contracts.DTOs
{
    public class DateRangeDTO
    {
        [Required]

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }
}
