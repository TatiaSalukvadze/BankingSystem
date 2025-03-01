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
        private readonly IPersonService _personService;
        private readonly IAccountService _accountService;
        private readonly ICardService _cardService;
        private readonly ITransactionDetailsService _transactionService;
        private readonly ITransactionOperationService _transactionOperationService;

        public UserBankingController(IPersonService personService, IAccountService accountService,
            ICardService cardService, ITransactionDetailsService transactionService,
            ITransactionOperationService transactionOperationService)
        {
            _personService = personService;
            _accountService = accountService;
            _cardService = cardService;
            _transactionService = transactionService;
            _transactionOperationService = transactionOperationService;
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
        public async Task<IActionResult> TransferToOwnAccount([FromForm] CreateOnlineTransactionDTO createTransactionDto)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Name);
            var (success, message) = await _transactionOperationService.OnlineTransactionAsync(createTransactionDto, userEmail, isSelfTransfer: true);
            return await HandleResult(success, message);
            //if (!success)
            //{
            //    return BadRequest(message);
            //}
            //return Ok(new { message, data });
        }

        [HttpPost("TransferToOtherAccount")]
        public async Task<IActionResult> TransferToOtherAccount([FromForm] CreateOnlineTransactionDTO createTransactionDto)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Name);
            var (success, message) = await _transactionOperationService.OnlineTransactionAsync(createTransactionDto, userEmail, isSelfTransfer: false);
            return await HandleResult(success, message);
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
