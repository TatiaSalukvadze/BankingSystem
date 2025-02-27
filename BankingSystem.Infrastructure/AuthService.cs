using BankingSystem.Contracts.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BankingSystem.Infrastructure
{
    public class AuthService : IAuthService
    {
        private readonly SymmetricSecurityKey _key;

        public AuthService(IConfiguration config)
        {
            _key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config["JwtSettings:PrivateKey"]!));
        }

        public string GenerateToken(IdentityUser user, string role)
        {
            var handler = new JwtSecurityTokenHandler();
            //var key = Encoding.ASCII.GetBytes(AuthSettings.PrivateKey);
            var credentials = new SigningCredentials(
                _key,
                SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = GenerateClaims(user, role),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = credentials,
            };

            var token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }

        private static ClaimsIdentity GenerateClaims(IdentityUser user, string role)
        {
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.Name, user.Email));
            claims.AddClaim(new Claim(ClaimTypes.Role, role));

            return claims;
        }
    }
}
