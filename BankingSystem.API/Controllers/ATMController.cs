using BankingSystem.Contracts.DTOs.ATM;
using BankingSystem.Contracts.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.API.Controllers
{
    [Route("/[controller]")]
    public class ATMController : WrapperController
    {
        private readonly ITransactionOperationService _transactionOperationService;
        private readonly ICardService _cardService;

        public ATMController(ITransactionOperationService transactionOperationService, ICardService cardService)
        {
            _transactionOperationService = transactionOperationService;
            _cardService = cardService;
        }

        [HttpGet("Balance")]
        public async Task<IActionResult> SeeBalanceAsync([FromQuery] CardAuthorizationDTO cardAuthorizationDto)
        {
            var response = await _cardService.SeeBalanceAsync(cardAuthorizationDto);
            var (success, message, data) = (response.Success, response.Message, response.Data);
            return await HandleResult(success, message, data);
        }

        [HttpPost("Withdraw")]
        public async Task<IActionResult> WithdrawAsync([FromForm] WithdrawalDTO withdrawalDto)
        {
            var response = await _transactionOperationService.WithdrawAsync(withdrawalDto);
            var (success, message) = (response.Success, response.Message);
            return await HandleResult(success, message);
        }

        [HttpPut("PIN")]
        public async Task<IActionResult> ChangeCardPINAsync([FromForm] ChangeCardPINDTO changeCardDtp)
        {
            var response = await _cardService.ChangeCardPINAsync(changeCardDtp);
            return new ObjectResult(response);
            //var (success, message) = (response.Success, response.Message);
            //return await HandleResult(success, message);
        }
    }
}
