using BankingSystem.Contracts.Interfaces.IExternalServices;
using BankingSystem.Contracts.Interfaces.IServices;
using BankingSystem.Contracts.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BankingSystem.Contracts.DTOs.Auth;
using BankingSystem.Contracts.DTOs.OnlineBank;
using BankingSystem.Contracts.Response;

namespace BankingSystem.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(ITokenService tokenService, UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager, IEmailService emailService, IUnitOfWork unitOfWork)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Response<object>> LoginPersonAsync(LoginDTO loginDto)
        {
            var response = new Response<object>();
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());
            if (user is null)
            {
                return response.Set(false, "Invalid username!", null, 400);
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
            {
                return response.Set(false, "Invalid username or password!", null, 400);
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "";

            var accessToken = _tokenService.GenerateAccessToken(user.Email, role);
            var refreshToken = _tokenService.GenerateRefreshToken(user.Id, loginDto.DeviceId);

            var refreshTokenSaved = await _unitOfWork.RefreshTokenRepository.SaveRefreshTokenAsync(refreshToken);
            if (!refreshTokenSaved)
            {
                return response.Set(false, "Refresh Token was not saved!", null, 400);
            }
            return response.Set(true, "Login successful!", new { accessToken, refreshToken = refreshToken.Token }, 200);
        }

        public async Task<Response<string>> RegisterPersonAsync(RegisterPersonDTO registerPersonDto)
        {
            var response = new Response<string>();
            var existingUser = await _userManager.FindByEmailAsync(registerPersonDto.Email);
            if (existingUser is not null)
            {
                return response.Set(false, "A user with this email already exists!", null, 409);
            }

            var user = new IdentityUser
            {
                UserName = registerPersonDto.Email,
                Email = registerPersonDto.Email,
            };

            var createdUser = await _userManager.CreateAsync(user, registerPersonDto.Password);
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
            await _emailService.SendTokenEmailAsync(token, registerPersonDto.Email, registerPersonDto.ClientUrl, "Email Confirmation Token");

            return response.Set(true, "User was registered successfully!", user.Id, 200);
        }

        public async Task<SimpleResponse> ConfirmEmailAsync(EmailConfirmationDTO emailConfirmationDto)
        {
            var response = new SimpleResponse();
            var user = await _userManager.FindByEmailAsync(emailConfirmationDto.Email);
            if (user is null)
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

        public async Task<SimpleResponse> ForgotPasswordAsync(ForgotPasswordDTO forgotPasswordDto)
        {
            var response = new SimpleResponse();
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user is null)
            {
                return response.Set(false, "User not found!", 404);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _emailService.SendTokenEmailAsync(token, forgotPasswordDto.Email, forgotPasswordDto.ClientUrl, "Reset password token");

            return response.Set(true, "Check email to reset password!", 200);
        }

        public async Task<SimpleResponse> ResetPasswordAsync(ResetPasswordDTO resetPasswordDto)
        {
            var response = new SimpleResponse();
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user is null)
            {
                return response.Set(false, "User not found!", 404);
            }

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.Password);
            if (!result.Succeeded)
            {
                return response.Set(false, "Password reset failed, please try again!", 400);
            }

            return response.Set(true, "Password reset successful!", 200);
        }

        public async Task<Response<object>> RefreshTokensAsync(RefreshTokensDTO refreshTokensDto)
        {
            var response = new Response<object>();
            var refreshToken = await _unitOfWork.RefreshTokenRepository.GetRefreshTokenAsync(refreshTokensDto.RefreshToken);
            if (refreshToken is null || refreshToken.ExpirationDate <= DateTime.UtcNow || refreshTokensDto.DeviceId != refreshToken.DeviceId)
            {
                return response.Set(false, "Provided refresh token is invalid!", null, 400);
            }

            var newAccessToken = _tokenService.RenewAccessToken(refreshTokensDto.AccessToken);
            if(newAccessToken is null)
            {
                return response.Set(false, "New access token could not be generated!", null, 400);
            }

            var newRefreshToken = _tokenService.GenerateRefreshToken(refreshToken.IdentityUserId, refreshTokensDto.DeviceId);
            var refreshTokenSaved = await _unitOfWork.RefreshTokenRepository.SaveRefreshTokenAsync(newRefreshToken);
            if (!refreshTokenSaved)
            {
                return response.Set(false, "Refresh Token was not saved!", null, 400);
            }

            var oldRefreshTokenDeleted = await _unitOfWork.RefreshTokenRepository.DeleteRefreshTokenAsync(refreshToken.Id);
            if (!oldRefreshTokenDeleted)
            {
                return response.Set(false, "Failed to delete old refresh token!",null, 400);
            }

            return response.Set(true, "New access token and refresh token retrieved!", new { newAccessToken, newRefreshToken = newRefreshToken.Token }, 200);
        }

        public async Task<SimpleResponse> LogoutAsync(LogoutDTO logoutDto, string userEmail)
        {
            var response = new SimpleResponse();

            var refreshToken = await _unitOfWork.RefreshTokenRepository.GetRefreshTokenAsync(logoutDto.RefreshToken);
            if (refreshToken is null)
            {
                return response.Set(false, "Provided refresh token is invalid!", 400);
            }

            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user is null || refreshToken.IdentityUserId != user.Id || logoutDto.DeviceId != refreshToken.DeviceId)
            {
                return response.Set(false, "You are not allowed to logout!", 400);
            }

            var deleted = await _unitOfWork.RefreshTokenRepository.DeleteRefreshTokenAsync(refreshToken.Id);
            if(!deleted)
            {
                return response.Set(false, "Failed to logout!", 400);
            }

            return response.Set(true, "Logged out Successfully!", 200);
        }
    }
}
