using BankingSystem.Contracts.DTOs.Auth;
using BankingSystem.Contracts.DTOs.Identity;
using BankingSystem.Contracts.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.API.Controllers.OnlineBank
{
    [Route("OnlineBank/[Controller]")]
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
            var response = await _identityService.LoginPersonAsync(loginDto);
            var (success, message, data) = (response.Success, response.Message, response.Data);
            return await HandleResult(success, message, data);
            //if (!success)
            //{
            //    return BadRequest(message);
            //}

            //return Ok(new { message, data });
        }
        //tatia
        //https://fusionauth.io/dev-tools/url-encoder-decoder used for decode
        [HttpGet("EmailConfirmation")]
        public async Task<IActionResult> EmailConfirmation([FromQuery] EmailConfirmationDTO emailConfirmationDto)//string email, [FromQuery] string token)
        {
            var response = await _identityService.ConfirmEmailAsync(emailConfirmationDto);
            var (success, message) = (response.Success, response.Message);
            return await HandleResult(success, message);
            //if (!success)
            //{
            //    return BadRequest(message);
            //}

            //return Ok(new { message});
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromForm] ForgotPasswordDTO forgotPasswordDTO)
        {
            var response = await _identityService.ForgotPasswordAsync(forgotPasswordDTO);
            var (success, message) = (response.Success, response.Message);
            return await HandleResult(success, message);
         
            //if (!success)
            //{
            //    return BadRequest(message);
            //}

                //return Ok(new { message });
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordDTO resetPasswordDTO)
        {
            var response = await _identityService.ResetPasswordAsync(resetPasswordDTO);
            var (success, message) = (response.Success, response.Message);
            return await HandleResult(success, message);
            //if (!success)
            //{
            //    return BadRequest(message);
            //}

            //return Ok(new { message });
        }

    }
}
