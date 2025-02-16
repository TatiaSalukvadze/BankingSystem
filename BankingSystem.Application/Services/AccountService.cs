using BankingSystem.Contracts.DTOs;
using BankingSystem.Contracts.Interfaces;
using BankingSystem.Contracts.Interfaces.IServices;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        public async Task<(bool Success, string Message, object? Data)> CreateAccountAsync(CreateAccountDTO createAccountDto)
        {
            try
            {
                int personId = await _unitOfWork.PersonRepository.FindIdByIDNumberAsync(createAccountDto.IDNumber);

                if (personId > 0)
                {
                    return (false, "Such person doesn't exist in our system!", null);
                }

                return (false, "dummy", null);//dummy
            }
            catch (Exception ex) { return (false, ex.Message, null); }
        }
    }
}
