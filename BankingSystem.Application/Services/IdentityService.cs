using BankingSystem.Contracts.DTOs;
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

namespace BankingSystem.Application.Services
{

    public class IdentityService : IIdentityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailService _emailService;

        public IdentityService(IUnitOfWork unitOfWork, IAuthService authService, UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _authService = authService;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        //tamar
        public async Task<(bool Success, string Message, object? Data)> LoginPersonAsync(LoginDTO loginDto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.Username!.ToLower());
            if (user == null)
            {
                return (false, "Invalid username!", null);
            }

            if (!user.EmailConfirmed)
            {
                return (false, "Email is not confirmed. Please verify your email before logging in.", null);
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password!, false);
            if (!result.Succeeded)
            {
                return (false, "Invalid username or password!", null);
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "";

            var customUser = await _unitOfWork.PersonRepository.FindByIdentityIdAsync(user.Id);

            if (role == "User" && customUser == null)
            {
                return (false, "Custom user data not found for this account!", null);
            }

            var token = _authService.GenerateToken(user, role);
          
            return (true, "Login successful!", new { token, customUser });

        }

        //tatia
        public async Task<(bool Success, string Message, string? Data)> RegisterPersonAsync(RegisterPersonDTO registerDto)
        {

            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return (false, "A user with this email already exists!", null);
            }
            var user = new IdentityUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
            };
            var createdUser = await _userManager.CreateAsync(user, registerDto.Password);
            if (!createdUser.Succeeded)
            {
                return (false, "Adding user in identity system failed!", null);
            }
            var roleResult = await _userManager.AddToRoleAsync(user, "User");
            if (!roleResult.Succeeded)
            {
                return (false, "Adding user corresponding role  in system failed!", null);
            }
            //var person = new Person
            //{
            //    IdentityUserId = user.Id,
            //    Name = registerDto.Name,
            //    Surname = registerDto.Surname,
            //    IDNumber = registerDto.IDNumber,
            //    Birthdate = registerDto.Birthdate,
            //    Email = registerDto.Email,
            //    CreatedAt = DateTime.Now

            //};

            //var userId = await _unitOfWork.PersonRepository.RegisterPersonAsync(person);
            //if (userId <= 0)
            //{
            //    return (false, "Adding user in physical person system failed!", null);
            //}
            //_unitOfWork.SaveChanges();

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var tokenEmail = new Dictionary<string, string?>()
            {
                {"email", registerDto.Email},
                {"token", token }
            };
            var verificationUrl = QueryHelpers.AddQueryString(registerDto.ClientUrl!, tokenEmail);
            await _emailService.SendEmailPlaint(registerDto.Email, "Email Confirmation Token", verificationUrl);
            return (true, "User was registered successfully!",  user.Id );
        }

        public async Task<(bool Success, string Message)> ConfirmEmailAsync(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return (false, "Email confirmation request is invalid, user was not found!");
            }

            var confirmEmail = await _userManager.ConfirmEmailAsync(user, token);
            if (!confirmEmail.Succeeded)
            {
                return (false, "Email confirmation request is invalid!");
            }
            return (true, "Email was successfully confirmed!");
        }


        public async Task<(bool Success, string Message)> ForgotPasswordAsync(ForgotPasswordDTO forgotPasswordDTO)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDTO.Email);
            if (user == null)
            {
                return (false, "User not found!");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var tokenEmail = new Dictionary<string, string>()
            {
                { "email", forgotPasswordDTO.Email },
                { "token", token }
            };

            var url = QueryHelpers.AddQueryString(forgotPasswordDTO.ClientUrl, tokenEmail);
            await _emailService.SendEmailPlaint(forgotPasswordDTO.Email, "Reset password token", url);
            return (true, "Check email to reset password!");
        }

        public async Task<(bool Success, string Message)> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDTO.Email);
            if (user == null)
            {
                return (false, "User not found!");
            }

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDTO.Token, resetPasswordDTO.Password);
            if (!result.Succeeded)
            {
                return (false, "Password reset failed! Please try again.");
            }

            return (true, "Password reset successful!");
        }
    }
}
