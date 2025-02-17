using BankingSystem.Contracts.DTOs;
using BankingSystem.Contracts.Interfaces.IExternalServices;
using BankingSystem.Contracts.Interfaces.IServices;
using BankingSystem.Infrastructure.ExternalServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankingSystem.API.Controllers.OnlineBank
{
    [Route("/OnlineBank/[controller]")]
    public class PersonController : WrapperController
    {
        private readonly IPersonService _personService;
        private readonly IAccountService _accountService;
        private readonly ICardService _cardService;
        //private readonly ICurrencyService _currencyService;
        private readonly ITransactionService _transactionService;

        public PersonController(IPersonService personService, IAccountService accountService,
            ICardService cardService, ITransactionService transactionService)
        {
            _personService = personService;
            _accountService = accountService;
            _cardService = cardService;
            //_currencyService = currencyService;
            _transactionService = transactionService;
        }

        [Authorize(policy: "UserOnly")]
        [HttpGet("Accounts")]
        public async Task<IActionResult> SeeAccounts()
        {
            //string email = User.Identity.Name; 
            var userEmail = User.FindFirstValue(ClaimTypes.Name);
            var (success, message, data) = await _accountService.SeeAccountsAsync(userEmail);

            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message, data });
        }


        [Authorize(policy:"UserOnly")]
        [HttpGet("Cards")]
        public async Task<IActionResult> SeeCards()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Name);
            var (success, message, data) = await _cardService.SeeCardsAsync(userEmail);
            if (!success)
            {
                return BadRequest(message);
            }
            return Ok(new { message, data });
        }

        [Authorize(policy: "UserOnly")]
        [HttpPost("TransferToOwnAccount")]
        public async Task<IActionResult> TransferToOwnAccount([FromForm] CreateTransactionDTO createTransactionDto)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Name);
            var (success, message, data) = await _transactionService.OnlineTransactionAsync(createTransactionDto, userEmail, true);
            if (!success)
            {
                return BadRequest(message);
            }
            return Ok(new { message, data });
        }

        [Authorize(policy: "UserOnly")]
        [HttpPost("TransferToOtherAccount")]
        public async Task<IActionResult> TransferToOtherAccount([FromForm] CreateTransactionDTO createTransactionDto)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Name);
            var (success, message, data) = await _transactionService.OnlineTransactionAsync(createTransactionDto, userEmail, false);
            if (!success)
            {
                return BadRequest(message);
            }
            return Ok(new { message, data });
        }

    }
}
