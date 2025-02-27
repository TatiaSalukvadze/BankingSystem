using BankingSystem.Contracts.Interfaces;
using BankingSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Infrastructure
{
    public class AuthService : IAuthService
    {
        private readonly SymmetricSecurityKey _key;
        public AuthService(IConfiguration config)
        {
            _key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config["JwtSettings:PrivateKey"]!));
        }
        public string GenerateToken(IdentityUser User, string role)
        {
            var handler = new JwtSecurityTokenHandler();
            //var key = Encoding.ASCII.GetBytes(AuthSettings.PrivateKey);
            var credentials = new SigningCredentials(
                _key,
                SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = GenerateClaims(User, role),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = credentials,
            };

            var token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }

        private static ClaimsIdentity GenerateClaims(IdentityUser User, string role)
        {
            var claims = new ClaimsIdentity();
            if (User is IdentityUser user)//aq unda iyos IdentityUser
            {
                claims.AddClaim(new Claim(ClaimTypes.Name, user.Email));
                claims.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }
    }
}
