using BankingSystem.Contracts.DTOs.Auth;
using BankingSystem.Contracts.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankingSystem.API.Controllers.OnlineBank
{
    [Route("OnlineBank/[Controller]")]
    public class AuthController : WrapperController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromForm] LoginDTO loginDto)
        {
            var response = await _authService.LoginPersonAsync(loginDto);
            return new ObjectResult(response);
        }
        
        [HttpGet("EmailConfirmation")]
        public async Task<IActionResult> EmailConfirmation([FromQuery] EmailConfirmationDTO emailConfirmationDto)
        {
            var response = await _authService.ConfirmEmailAsync(emailConfirmationDto);
            return new ObjectResult(response);
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromForm] ForgotPasswordDTO forgotPasswordDTO)
        {
            var response = await _authService.ForgotPasswordAsync(forgotPasswordDTO);
            return new ObjectResult(response);
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordDTO resetPasswordDTO)
        {
            var response = await _authService.ResetPasswordAsync(resetPasswordDTO);
            return new ObjectResult(response);
        }


        [HttpPost("Refresh")]
        public async Task<IActionResult> Refresh([FromForm] RefreshTokensDTO refreshTokensDto)
        {
            var response = await _authService.RefreshTokensAsync(refreshTokensDto);
            return new ObjectResult(response);
        }

        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromForm] LogoutDTO logoutDto)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Name);
            var response = await _authService.LogoutAsync(logoutDto, userEmail);
            return new ObjectResult(response);
        }
    }
}
