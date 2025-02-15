using BankingSystem.Contracts.DTOs;
using BankingSystem.Contracts.Interfaces;
using BankingSystem.Contracts.Interfaces.IServices;
using BankingSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly SignInManager<IdentityUser> _signInManager;

        public PersonService(IUnitOfWork unitOfWork,IAuthService authService, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _unitOfWork = unitOfWork;
            _authService = authService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<(bool Success, string Message, object? Data)> LoginPersonAsync(LoginDTO loginDto)
        {
            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.Username!.ToLower());
                if (user == null)
                {
                    return (false, "Invalid username!", null);
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password!, false);
                if (!result.Succeeded)
                {
                    return (false, "Invalid username or password!", null);
                }

                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault() ?? ""; 

                var customUser = await _unitOfWork.PersonRepository.FindByIdentityIdAsync(user.Id);
                var token = _authService.GenerateToken(user, role);

                return (true, "Login successful!", new { token, customUser });
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
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
