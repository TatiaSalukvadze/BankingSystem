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
        public async Task<IActionResult> SeeAccounts([FromQuery] int page = 1, [FromQuery] int perPage = 2)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Name);
            var response = await _accountService.SeeAccountsAsync(userEmail, page, perPage);
            return new ObjectResult(response);
        }

        [HttpGet("Cards")]
        public async Task<IActionResult> SeeCards([FromQuery] int page = 1, [FromQuery] int perPage = 2)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Name);
            var response = await _cardService.SeeCardsAsync(userEmail, page, perPage);
            return new ObjectResult(response);
        }

        [HttpPost("TransferToOwnAccount")]
        public async Task<IActionResult> TransferToOwnAccount([FromForm] CreateOnlineTransactionDTO createTransactionDto)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Name);
            var response = await _transactionOperationService.OnlineTransactionAsync(createTransactionDto, userEmail, isSelfTransfer: true);
            return new ObjectResult(response);
        }

        [HttpPost("TransferToOtherAccount")]
        public async Task<IActionResult> TransferToOtherAccount([FromForm] CreateOnlineTransactionDTO createTransactionDto)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Name);
            var response = await _transactionOperationService.OnlineTransactionAsync(createTransactionDto, userEmail, isSelfTransfer: false);
            return new ObjectResult(response);
        }

        [HttpGet("TotalIncomeExpense")]
        public async Task<IActionResult> TotalIncomeExpense([FromQuery] DateRangeDTO dateRangeDto)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Name);
            var response = await _transactionService.TotalIncomeExpenseAsync(dateRangeDto, userEmail);
            return new ObjectResult(response);
        }
    }
}
