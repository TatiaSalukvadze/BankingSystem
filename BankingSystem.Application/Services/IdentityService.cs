using BankingSystem.Contracts.Interfaces.IExternalServices;
using BankingSystem.Contracts.Interfaces.IServices;
using BankingSystem.Contracts.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BankingSystem.Contracts.DTOs.Auth;
using BankingSystem.Contracts.DTOs.Identity;
using BankingSystem.Contracts.DTOs.OnlineBank;
using BankingSystem.Contracts.Response;

namespace BankingSystem.Application.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly IAuthService _authService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailService _emailService;

        public IdentityService(IAuthService authService, UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager, IEmailService emailService)
        {
            _authService = authService;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        public async Task<Response<object>> LoginPersonAsync(LoginDTO loginDto)
        {
            var response = new Response<object>();
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.Username!.ToLower());
            if (user == null)
            {
                return response.Set(false, "Invalid username!", null, 400);
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password!, false);
            if (!result.Succeeded)
            {
                return response.Set(false, "Invalid username or password!", null, 400);
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "";
            var token = _authService.GenerateToken(user, role);

            return response.Set(true, "Login successful!", new { token }, 200);
        }

        public async Task<Response<string>> RegisterPersonAsync(RegisterPersonDTO registerDto)
        {
            var response = new Response<string>();
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return response.Set(false, "A user with this email already exists!", null, 409);
            }

            var user = new IdentityUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
            };

            var createdUser = await _userManager.CreateAsync(user, registerDto.Password);
            if (!createdUser.Succeeded)
            {
                return response.Set(false, "Adding user in identity system failed!", null, 400);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "User");
            if (!roleResult.Succeeded)
            {
                return response.Set(false, "Adding user corresponding role in system failed!", null, 400);
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await _emailService.SendTokenEmailAsync(token, registerDto.Email, registerDto.ClientUrl, "Email Confirmation Token");

            return response.Set(true, "User was registered successfully!", user.Id, 200);
        }

        public async Task<SimpleResponse> ConfirmEmailAsync(EmailConfirmationDTO emailConfirmationDto)
        {
            var response = new SimpleResponse();
            var user = await _userManager.FindByEmailAsync(emailConfirmationDto.Email);
            if (user == null)
            {
                return response.Set(false, "Email confirmation request is invalid, user was not found!", 404);
            }

            var confirmEmail = await _userManager.ConfirmEmailAsync(user, emailConfirmationDto.Token);
            if (!confirmEmail.Succeeded)
            {
                return response.Set(false, "Email confirmation request is invalid!", 400);
            }

            return response.Set(true, "Email was successfully confirmed!", 200);
        }

        public async Task<SimpleResponse> ForgotPasswordAsync(ForgotPasswordDTO forgotPasswordDTO)
        {
            var response = new SimpleResponse();
            var user = await _userManager.FindByEmailAsync(forgotPasswordDTO.Email);
            if (user == null)
            {
                return response.Set(false, "User not found!", 404);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _emailService.SendTokenEmailAsync(token, forgotPasswordDTO.Email, forgotPasswordDTO.ClientUrl, "Reset password token");

            return response.Set(true, "Check email to reset password!", 200);
        }

        public async Task<SimpleResponse> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO)
        {
            var response = new SimpleResponse();
            var user = await _userManager.FindByEmailAsync(resetPasswordDTO.Email);
            if (user == null)
            {
                return response.Set(false, "User not found!", 404);
            }

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDTO.Token, resetPasswordDTO.Password);
            if (!result.Succeeded)
            {
                return response.Set(false, "Password reset failed! Please try again.", 400);
            }

            return response.Set(true, "Password reset successful!", 200);
        }
    }
}
