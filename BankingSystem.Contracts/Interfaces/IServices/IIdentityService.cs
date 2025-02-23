using BankingSystem.Contracts.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Contracts.Interfaces.IServices
{
    public interface IIdentityService
    {
        Task<(bool Success, string Message, object? Data)> LoginPersonAsync(LoginDTO loginDto);
        Task<(bool Success, string Message, string? Data)> RegisterPersonAsync(RegisterPersonDTO registerDto);
        Task<(bool Success, string Message)> ConfirmEmailAsync(EmailConfirmationDTO emailConfirmationDto
            );
        Task<(bool Success, string Message)> ForgotPasswordAsync(ForgotPasswordDTO forgotPasswordDTO);
        Task<(bool Success, string Message)> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO);
    }
}
