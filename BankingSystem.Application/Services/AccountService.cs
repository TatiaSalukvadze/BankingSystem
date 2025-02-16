﻿using BankingSystem.Contracts.DTOs;
using BankingSystem.Contracts.Interfaces;
using BankingSystem.Contracts.Interfaces.IServices;
using BankingSystem.Domain.Entities;
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

                if (personId <= 0)
                {
                    return (false, "Such person doesn't exist in our system!", null);
                }

                bool IBANExists = await _unitOfWork.AccountRepository.IBANExists(createAccountDto.IDNumber);
                if (IBANExists)
                {
                    return (false, "Such IBAN already exist in our system!", null);
                }
                var account = new Account
                {
                    PersonId = personId,
                    IBAN = createAccountDto.IBAN,
                    Amount = createAccountDto.Amount,
                    CurrencyId = (int)createAccountDto.Currency
                };
                 int insertedId = await _unitOfWork.AccountRepository.CreateAccountAsync(account);
                if (insertedId <= 0)
                {
                    return (false, "Account could not be created, something happened!", null);
                }

                account.Id = insertedId;
                _unitOfWork.SaveChanges();
                return (true, "Account was created successfully!", account);
            }
            catch (Exception ex) { return (false, ex.Message, null); }
        }
    }
}
