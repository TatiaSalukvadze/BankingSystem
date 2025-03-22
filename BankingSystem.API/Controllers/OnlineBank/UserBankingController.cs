using BankingSystem.Contracts.DTOs.UserBanking;
using BankingSystem.Contracts.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankingSystem.API.Controllers.OnlineBank
{
    [Authorize(policy: "UserOnly")]
    [Route("/OnlineBank/[controller]")]
    public class UserBankingController : WrapperController
    {
        private readonly IAccountService _accountService;
        private readonly ICardService _cardService;
        private readonly ITransactionDetailsService _transactionService;
        private readonly ITransactionOperationService _transactionOperationService;

        public UserBankingController(IAccountService accountService,
            ICardService cardService, ITransactionDetailsService transactionService,
            ITransactionOperationService transactionOperationService)
        {
            _accountService = accountService;
            _cardService = cardService;
            _transactionService = transactionService;
            _transactionOperationService = transactionOperationService;
        }

        [HttpGet("Accounts")]
        public async Task<IActionResult> SeeAccounts()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Name);
            var response = await _accountService.SeeAccountsAsync(userEmail);
            return new ObjectResult(response);
            //var (success, message, data) = (response.Success, response.Message, response.Data);
            //return await HandleResult(success, message, data);
        }

        [HttpGet("Cards")]
        public async Task<IActionResult> SeeCards()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Name);
            var response = await _cardService.SeeCardsAsync(userEmail);
            return new ObjectResult(response);
            //var (success, message, data) = (response.Success, response.Message, response.Data);
            //return await HandleResult(success, message, data);
        }

        [HttpPost("TransferToOwnAccount")]
        public async Task<IActionResult> TransferToOwnAccount([FromForm] CreateOnlineTransactionDTO createTransactionDto)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Name);
            var response = await _transactionOperationService.OnlineTransactionAsync(createTransactionDto, userEmail, isSelfTransfer: true);
            return new ObjectResult(response);
            //var (success, message) = (response.Success, response.Message);
            //return await HandleResult(success, message);
        }

        [HttpPost("TransferToOtherAccount")]
        public async Task<IActionResult> TransferToOtherAccount([FromForm] CreateOnlineTransactionDTO createTransactionDto)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Name);
            var response = await _transactionOperationService.OnlineTransactionAsync(createTransactionDto, userEmail, isSelfTransfer: false);
            return new ObjectResult(response);
            //var (success, message) = (response.Success, response.Message);
            //return await HandleResult(success, message);
        }

        [HttpGet("TotalIncomeExpense")]
        public async Task<IActionResult> TotalIncomeExpense([FromQuery] DateRangeDTO dateRangeDto)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Name);
            var response = await _transactionService.TotalIncomeExpenseAsync(dateRangeDto, userEmail);
            return new ObjectResult(response);
            //var (success, message, data) = (response.Success, response.Message, response.Data);
            //return await HandleResult(success, message, data);
        }
    }
}
