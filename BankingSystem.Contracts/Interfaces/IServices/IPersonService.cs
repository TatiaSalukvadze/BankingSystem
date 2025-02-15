using BankingSystem.Contracts.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Contracts.Interfaces.IServices
{
    public interface IPersonService
    {
         Task<(bool Success, string Message, object? Data)> RegisterPersonAsync(RegisterDTO registerDto);
        

    }
}
