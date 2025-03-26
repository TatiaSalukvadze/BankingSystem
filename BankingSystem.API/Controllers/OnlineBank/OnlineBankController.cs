using BankingSystem.Contracts.DTOs.OnlineBank;
using BankingSystem.Contracts.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.API.Controllers.OnlineBank
{
    //[Authorize(policy: "OperatorOnly")]
    public class OnlineBankController : WrapperController
    {
        private readonly IPersonService _personService;
        private readonly IAccountService _accountService;
        private readonly ICardService _cardService;
        private readonly IIdentityService _identityService;

        public OnlineBankController(IPersonService personService, IAccountService accountService, 
            ICardService cardService, IIdentityService identityService)
        {
            _personService = personService;
            _accountService = accountService;
            _cardService = cardService;
            _identityService = identityService;
        }

        [HttpPost("RegisterPerson")]
        public async Task<IActionResult> RegisterPerson([FromForm] RegisterPersonDTO registerDto)
        {
            var response = await _identityService.RegisterPersonAsync(registerDto);        
            if (!response.Success) {
                return BadRequest(response.Message);
            }
            var finalResponse = await _personService.RegisterCustomPersonAsync(registerDto, response.Data);
            return new ObjectResult(finalResponse);
        }

        [HttpPost("Account")]
        public async Task<IActionResult> CreateAccount([FromForm] CreateAccountDTO createAccountDto)
        {
            var response = await _accountService.CreateAccountAsync(createAccountDto);
            return new ObjectResult(response);
        }

        [HttpDelete("Account")]
        public async Task<IActionResult> DeleteAccount(string iban)
        {
            var response = await _accountService.DeleteAccountAsync(iban);
            return new ObjectResult(response);
        }

        [HttpPost("Card")]
        public async Task<IActionResult> CreateCard([FromForm] CreateCardDTO createCardDto)
        {
            var response = await _cardService.CreateCardAsync(createCardDto);
            return new ObjectResult(response);
        }

        [HttpDelete("Card")]
        public async Task<IActionResult> DeleteCard([FromForm] string cardNumber)
        {
            var response = await _cardService.CancelCardAsync(cardNumber);
            return new ObjectResult(response);
        }
    }
}
