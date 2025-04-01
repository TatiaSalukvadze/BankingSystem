using Azure;
using BankingSystem.Contracts.DTOs.UserBanking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Application.HelperServices
{
    public class PagingService
    {
        public PagingResponseDTO<T> PageResult<T>(int currentPage, int perPage, int totalDataCount, List<T> paginatedData)
        {
            var offset = (currentPage - 1) * perPage;

            var totalPages = (int)Math.Ceiling((double)totalDataCount / perPage);



            var response = new PagingResponseDTO<T>() 
            {
                Data = paginatedData,
                TotalPages = (int)Math.Ceiling((double)totalDataCount / perPage),
                TotalDataCount = totalDataCount,
                CurrentPage = currentPage,
                DataCountPerPage = perPage,
                HasNext = currentPage < totalPages,
                HasPrevious = currentPage > 1

            };
            return response;
        }
    }
}
