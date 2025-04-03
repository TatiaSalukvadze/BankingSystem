using BankingSystem.Contracts.DTOs.Auth;
using BankingSystem.Contracts.DTOs.OnlineBank;
using BankingSystem.Contracts.Response;

namespace BankingSystem.Contracts.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<Response<object>> LoginPersonAsync(LoginDTO loginDto);
        Task<Response<string>> RegisterPersonAsync(RegisterPersonDTO registerPersonDto);
        Task<SimpleResponse> ConfirmEmailAsync(EmailConfirmationDTO emailConfirmationDto);
        Task<SimpleResponse> ForgotPasswordAsync(ForgotPasswordDTO forgotPasswordDto);
        Task<SimpleResponse> ResetPasswordAsync(ResetPasswordDTO resetPasswordDto);
        Task<Response<object>> RefreshTokensAsync(RefreshTokensDTO refreshTokensDto);
        Task<SimpleResponse> LogoutAsync(LogoutDTO logoutDto, string userEmail);
    }
}
