using BankingSystem.Contracts.Interfaces.IExternalServices;
using BankingSystem.Contracts.Interfaces.IServices;
using BankingSystem.Contracts.Interfaces;
using BankingSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BankingSystem.Contracts.DTOs.Auth;
using BankingSystem.Contracts.DTOs.Identity;
using BankingSystem.Contracts.DTOs.OnlineBank;
using BankingSystem.Contracts.Response;

namespace BankingSystem.Application.Services
{

    public class IdentityService : IIdentityService
    {
        //private readonly IUnitOfWork _unitOfWork;
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

        //tamar
        public async Task<Response<object>> LoginPersonAsync(LoginDTO loginDto)
        {
            var response = new Response<object>();
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.Username!.ToLower());
            if (user == null)
            {
                return response.Set(false, "Invalid username!");
            }

            //if (!user.EmailConfirmed)
            //{
            //    return (false, "Email is not confirmed. Please verify your email before logging in.", null);
            //}

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password!, false);
            if (!result.Succeeded)
            {
                return response.Set(false, "Invalid username or password!");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "";

            //var customUser = await _unitOfWork.PersonRepository.FindByIdentityIdAsync(user.Id);

            //if (role == "User" && customUser == null)
            //{
            //    return (false, "Custom user data not found for this account!", null);
            //}

            var token = _authService.GenerateToken(user, role);

            return response.Set(true, "Login successful!", new { token });//, customUser });
        }

        //tatia
        public async Task<Response<string>> RegisterPersonAsync(RegisterPersonDTO registerDto)
        {
            var response = new Response<string>();
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return response.Set(false, "A user with this email already exists!");
            }
            var user = new IdentityUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
            };
            var createdUser = await _userManager.CreateAsync(user, registerDto.Password);
            if (!createdUser.Succeeded)
            {
                return response.Set(false, "Adding user in identity system failed!");
            }
            var roleResult = await _userManager.AddToRoleAsync(user, "User");
            if (!roleResult.Succeeded)
            {
                return response.Set(false, "Adding user corresponding role  in system failed!");
            }


            //var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //var tokenEmail = new Dictionary<string, string?>()
            //{
            //    {"email", registerDto.Email},
            //    {"token", token }
            //};
            //var verificationUrl = QueryHelpers.AddQueryString(registerDto.ClientUrl!, tokenEmail);
            //await _emailService.SendEmailPlaint(registerDto.Email, "Email Confirmation Token", verificationUrl);
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await _emailService.SendTokenEmailAsync(token, registerDto.Email, registerDto.ClientUrl, "Email Confirmation Token");
            //await GenerateEmailConfirmationTokenAsync(user, registerDto.Email, registerDto.ClientUrl);
            return response.Set(true, "User was registered successfully!",  user.Id );

            //async Task GenerateEmailConfirmationTokenAsync(IdentityUser identityUser, string email, string ClientUrl)
            //{
            //    var token = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);
            //    var tokenEmail = new Dictionary<string, string?>()
            //    {
            //        {"email", email},
            //        {"token", token }
            //    };
            //    var verificationUrl = QueryHelpers.AddQueryString(ClientUrl!, tokenEmail);
            //    await _emailService.SendEmailPlaint(email, "Email Confirmation Token", verificationUrl);
            //}

        }

        public async Task<SimpleResponse> ConfirmEmailAsync(EmailConfirmationDTO emailConfirmationDto)
        {
            var response = new SimpleResponse();
            var user = await _userManager.FindByEmailAsync(emailConfirmationDto.Email);

            if (user == null)
            {
                return response.Set(false, "Email confirmation request is invalid, user was not found!");
            }

            var confirmEmail = await _userManager.ConfirmEmailAsync(user, emailConfirmationDto.Token);
            if (!confirmEmail.Succeeded)
            {
                return response.Set(false, "Email confirmation request is invalid!");
            }
            return response.Set(true, "Email was successfully confirmed!");
        }


        public async Task<SimpleResponse> ForgotPasswordAsync(ForgotPasswordDTO forgotPasswordDTO)
        {
            var response = new SimpleResponse();
            var user = await _userManager.FindByEmailAsync(forgotPasswordDTO.Email);
            if (user == null)
            {
                return response.Set(false, "User not found!");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _emailService.SendTokenEmailAsync(token, forgotPasswordDTO.Email, forgotPasswordDTO.ClientUrl, "Reset password token");

            //var tokenEmail = new Dictionary<string, string>()
            //{
            //    { "email", forgotPasswordDTO.Email },
            //    { "token", token }
            //};

            //var url = QueryHelpers.AddQueryString(forgotPasswordDTO.ClientUrl, tokenEmail);
            //await _emailService.SendEmailPlaint(forgotPasswordDTO.Email, "Reset password token", url);
            return response.Set(true, "Check email to reset password!");
        }

        public async Task<SimpleResponse> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO)
        {
            var response = new SimpleResponse();
            var user = await _userManager.FindByEmailAsync(resetPasswordDTO.Email);
            if (user == null)
            {
                return response.Set(false, "User not found!");
            }

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDTO.Token, resetPasswordDTO.Password);
            if (!result.Succeeded)
            {
                return response.Set(false, "Password reset failed! Please try again.");
            }

            return response.Set(true, "Password reset successful!");
        }
    }
}
