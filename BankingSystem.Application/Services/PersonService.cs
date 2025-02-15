using BankingSystem.Contracts.DTOs;
using BankingSystem.Contracts.Interfaces;
using BankingSystem.Contracts.Interfaces.IServices;
using BankingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Application.Services
{
    public class PersonService : IPersonService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;
        public PersonService(IUnitOfWork unitOfWork,IAuthService authService) {
            _unitOfWork = unitOfWork;
            _authService = authService;
        }
        public async Task<(bool Success, string Message, object? Data)> RegisterPersonAsync(RegisterDTO registerDto)
        {
            return (false, "fsdf", null);
            try
            {
                var person = new Person
                {
                    UserId = "",
                    Name = registerDto.Name,
                    Surname = registerDto.Surname,
                    Email = registerDto.Email,
                    CreatedAt = DateTime.Now

                };
                await _unitOfWork.PersonRepository.RegisterPersonAsync(person);
                var token = _authService.GenerateToken(person, "User");
                return (true, "User registered successfully!", token);
                //    return Ok(new { token });
            }
            catch (Exception ex) { return (false, ex.Message, null); }
        }
    }
}
