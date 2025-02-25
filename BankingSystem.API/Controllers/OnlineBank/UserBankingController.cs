﻿using BankingSystem.Contracts.DTOs.UserBanking;
using BankingSystem.Contracts.Interfaces.IExternalServices;
using BankingSystem.Contracts.Interfaces.IServices;
using BankingSystem.Infrastructure.ExternalServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankingSystem.API.Controllers.OnlineBank
{
    [Authorize(policy: "UserOnly")]
    [Route("/OnlineBank/[controller]")]
    public class UserBankingController : WrapperController
    {
        private readonly IPersonService _personService;
        private readonly IAccountService _accountService;
        private readonly ICardService _cardService;
        //private readonly ICurrencyService _currencyService;
        private readonly ITransactionService _transactionService;

        public UserBankingController(IPersonService personService, IAccountService accountService,
            ICardService cardService, ITransactionService transactionService)
        {
            _personService = personService;
            _accountService = accountService;
            _cardService = cardService;
            //_currencyService = currencyService;
            _transactionService = transactionService;
        }

        [HttpGet("Accounts")]
        public async Task<IActionResult> SeeAccounts()
        {
            //string email = User.Identity.Name; 
            var userEmail = User.FindFirstValue(ClaimTypes.Name);
            var (success, message, data) = await _accountService.SeeAccountsAsync(userEmail);
            return await HandleResult(success, message, data);
            //if (!success)
            //{
            //    return BadRequest(new { message });
            //}

            //return Ok(new { message, data });
        }


        [HttpGet("Cards")]
        public async Task<IActionResult> SeeCards()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Name);
            var (success, message, data) = await _cardService.SeeCardsAsync(userEmail);
            return await HandleResult(success, message, data);
            //if (!success)
            //{
            //    return BadRequest(message);
            //}
            //return Ok(new { message, data });
        }


        [HttpPost("TransferToOwnAccount")]
        public async Task<IActionResult> TransferToOwnAccount([FromForm] CreateTransactionDTO createTransactionDto)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Name);
            var (success, message, data) = await _transactionService.OnlineTransactionAsync(createTransactionDto, userEmail, isSelfTransfer: true);
            return await HandleResult(success, message, data);
            //if (!success)
            //{
            //    return BadRequest(message);
            //}
            //return Ok(new { message, data });
        }


        [HttpPost("TransferToOtherAccount")]
        public async Task<IActionResult> TransferToOtherAccount([FromForm] CreateTransactionDTO createTransactionDto)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Name);
            var (success, message, data) = await _transactionService.OnlineTransactionAsync(createTransactionDto, userEmail, isSelfTransfer: false);
            return await HandleResult(success, message, data);
            //if (!success)
            //{
            //    return BadRequest(message);
            //}
            //return Ok(new { message, data });
        }

        [HttpGet("TotalIncomeExpense")]
        public async Task<IActionResult> TotalIncomeExpense([FromQuery] DateRangeDTO dateRangeDto)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Name);
            var (success, message, data) = await _transactionService.TotalIncomeExpenseAsync(dateRangeDto, userEmail);
            return await HandleResult(success, message, data);
            //if (!success)
            //{
            //    return BadRequest(message);
            //}
            //return Ok(new { message, data });
        }

    }
}
