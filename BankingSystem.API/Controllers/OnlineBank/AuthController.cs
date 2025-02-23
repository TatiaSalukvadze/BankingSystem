using BankingSystem.Contracts.DTOs;
using BankingSystem.Contracts.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.API.Controllers.OnlineBank
{
    [Route("OnlineBank")]
    public class AuthController : WrapperController
    {
        private readonly IPersonService _personService;
        private readonly IIdentityService _identityService;

        public AuthController(IPersonService personService, IIdentityService identityService)
        {
            _personService = personService;
            _identityService = identityService;
        }

        //tamar
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromForm] LoginDTO loginDto)
        {
            var (success, message, data) = await _identityService.LoginPersonAsync(loginDto);

            if (!success)
            {
                return BadRequest(message);
            }

            return Ok(new { message, data });
        }
        //tatia
        //https://fusionauth.io/dev-tools/url-encoder-decoder used for decode
        [HttpPost("EmailConfirmation")]
        public async Task<IActionResult> EmailConfirmation([FromQuery] string email, [FromQuery] string token)
        {
            var (success, message) = await _identityService.ConfirmEmailAsync(email, token);

            if (!success)
            {
                return BadRequest(message);
            }

            return Ok(new { message});
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromForm] ForgotPasswordDTO forgotPasswordDTO)
        {
            var (success, message) = await _identityService.ForgotPasswordAsync(forgotPasswordDTO);

            if (!success)
            {
                return BadRequest(message);
            }

            return Ok(new { message });
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordDTO resetPasswordDTO)
        {
            var (success, message) = await _identityService.ResetPasswordAsync(resetPasswordDTO);

            if (!success)
            {
                return BadRequest(message);
            }

            return Ok(new { message });
        }

    }
}
