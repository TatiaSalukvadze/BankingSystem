using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Contracts.DTOs.UserBanking
{
    public class DateRangeDTO
    {
        [Required]

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }
}
