using BankingSystem.Contracts.DTOs;
using BankingSystem.Contracts.Interfaces;
using BankingSystem.Contracts.Interfaces.IServices;
using BankingSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
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

        private readonly UserManager<IdentityUser> _userManager;
        public PersonService(IUnitOfWork unitOfWork,IAuthService authService, UserManager<IdentityUser> userManager) {
            _unitOfWork = unitOfWork;
            _authService = authService;

            _userManager = userManager;
        }
        public async Task<(bool Success, string Message, object? Data)> RegisterPersonAsync(RegisterDTO registerDto)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
                if (existingUser != null)
                {
                    return (false,"A user with this email already exists!",null);
                }
                var user = new IdentityUser
                {
                    UserName = registerDto.Email,
                    Email = registerDto.Email,
                };
                var createdUser = await _userManager.CreateAsync(user, registerDto.Password);
                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, "User");
                    if (roleResult.Succeeded)
                    {
                        var person = new Person
                        {
                            IdentityUserId = user.Id,
                            Name = registerDto.Name,
                            Surname = registerDto.Surname,
                            IDNumber = registerDto.IDNumber,
                            Birthdate = registerDto.Birthdate,
                            Email = registerDto.Email,
                            CreatedAt = DateTime.Now

                        };

                        var userId = await _unitOfWork.PersonRepository.RegisterPersonAsync(person);
                        if (userId > 0)
                        {
                            _unitOfWork.SaveChanges();
                            return (true, "User was registered successfully!",new { IdentityUserId = user.Id, CustomUserId = userId });
                        }
                        else
                        {
                            return (false, "Adding user in physical person system failed!", null);
                        }
                    }
                    else
                    {
                        return (false, "Adding user corresponding role  in system failed!", null);
                    }
                }
                else
                {
                    return (false, "Adding user in identity system failed!", null);
                }



            }
            catch (Exception ex) { return (false, ex.Message, null); }
        }
    }
}
