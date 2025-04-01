using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Contracts.DTOs.UserBanking
{
    public class PagingResponseDTO<T>
    {
        public List<T> Data { get; set; }
        public int TotalPages { get; set; }
        public int TotalDataCount { get; set; }
        public int CurrentPage { get; set; }
        public int DataCountPerPage { get; set; }
        public bool HasNext { get; set; }
        public bool HasPrevious { get; set; }
    }
}
