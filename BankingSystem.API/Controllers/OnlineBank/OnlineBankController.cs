﻿using BankingSystem.Contracts.DTOs.OnlineBank;
using BankingSystem.Contracts.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.API.Controllers.OnlineBank
{
    [Authorize(policy: "OperatorOnly")]
    public class OnlineBankController : WrapperController
    {
        private readonly IPersonService _personService;
        private readonly IAccountService _accountService;
        private readonly ICardService _cardService;

        public OnlineBankController(IPersonService personService, IAccountService accountService, 
            ICardService cardService)
        {
            _personService = personService;
            _accountService = accountService;
            _cardService = cardService;
        }

        [HttpPost("RegisterPerson")]
        public async Task<IActionResult> RegisterPerson([FromForm] RegisterPersonDTO registerDto, [FromServices] IAuthService authService)
        {
            var response = await authService.RegisterPersonAsync(registerDto);        
            if (!response.Success) {
                return new ObjectResult(response);
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
        public async Task<IActionResult> DeleteAccount([FromForm] DeleteAccountDTO deleteAccountDto)
        {
            var response = await _accountService.DeleteAccountAsync(deleteAccountDto);
            return new ObjectResult(response);
        }

        [HttpPost("Card")]
        public async Task<IActionResult> CreateCard([FromForm] CreateCardDTO createCardDto)
        {
            var response = await _cardService.CreateCardAsync(createCardDto);
            return new ObjectResult(response);
        }

        [HttpDelete("Card")]
        public async Task<IActionResult> DeleteCard([FromForm] DeleteCardDTO deleteCardDto)
        {
            var response = await _cardService.CancelCardAsync(deleteCardDto);
            return new ObjectResult(response);
        }
    }
}
