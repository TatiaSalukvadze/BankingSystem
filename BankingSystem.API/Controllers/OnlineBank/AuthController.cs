using BankingSystem.Contracts.DTOs.Auth;
using BankingSystem.Contracts.DTOs.Identity;
using BankingSystem.Contracts.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.API.Controllers.OnlineBank
{
    [Route("OnlineBank/[Controller]")]
    public class AuthController : WrapperController
    {
        private readonly IIdentityService _identityService;

        public AuthController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromForm] LoginDTO loginDto)
        {
            var response = await _identityService.LoginPersonAsync(loginDto);
            return new ObjectResult(response);
        }

        //https://fusionauth.io/dev-tools/url-encoder-decoder used for decode
        [HttpGet("EmailConfirmation")]
        public async Task<IActionResult> EmailConfirmation([FromQuery] EmailConfirmationDTO emailConfirmationDto)
        {
            var response = await _identityService.ConfirmEmailAsync(emailConfirmationDto);
            return new ObjectResult(response);
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromForm] ForgotPasswordDTO forgotPasswordDTO)
        {
            var response = await _identityService.ForgotPasswordAsync(forgotPasswordDTO);
            return new ObjectResult(response);
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordDTO resetPasswordDTO)
        {
            var response = await _identityService.ResetPasswordAsync(resetPasswordDTO);
            return new ObjectResult(response);
        }
    }
}
