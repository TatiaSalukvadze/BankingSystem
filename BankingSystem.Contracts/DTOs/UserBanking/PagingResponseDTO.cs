using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Contracts.DTOs.UserBanking
{
    public class PagingResponseDTO<T>
    {
        [Required]
        public List<T> Data { get; set; }

        [Required]
        public int TotalPages { get; set; }

        [Required]
        public int TotalDataCount { get; set; }

        [Required]
        public int CurrentPage { get; set; }

        [Required]
        public int DataCountPerPage { get; set; }

        public bool HasNext { get; set; }
        public bool HasPrevious { get; set; }
    }
}
