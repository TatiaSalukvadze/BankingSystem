using BankingSystem.Contracts.Interfaces;
using BankingSystem.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BankingSystem.Infrastructure.Auth
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration config)
        {
            _configuration = config;
        }

        public string GenerateAccessToken(string userEmail, string role)
        {
            var handler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JwtSettings:PrivateKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            if (!int.TryParse(_configuration["JwtSettings:AccessTokenExpirationMinutes"], out int accessTokenValidity))
            {
                throw new InvalidOperationException("Invalid AccessTokenValidityInMinutes value in configuration!");
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = GenerateClaims(userEmail, role),
                Expires = DateTime.UtcNow.AddMinutes(accessTokenValidity),
                SigningCredentials = credentials,
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"]
            };

            var token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }

        public RefreshToken GenerateRefreshToken(string identityUserId, string deviceId)
        {
            string refreshToken;
            var randomNumber = new Byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                refreshToken =  Convert.ToBase64String(randomNumber);
            }

            if (!int.TryParse(_configuration["JwtSettings:RefreshTokenExpirationDays"], out int refreshTokenValidity))
            {
                throw new InvalidOperationException("Invalid RefreshTokenExpirationDays value in configuration!");
            }

            var refreshTokenModel = new RefreshToken()
            {
                IdentityUserId = identityUserId,
                Token = refreshToken,
                ExpirationDate = DateTime.UtcNow.AddDays(refreshTokenValidity),
                DeviceId = deviceId,
                CreatedAt = DateTime.UtcNow
            };

            return refreshTokenModel;
        }

        public string RenewAccessToken(string oldAccessToken)
        {
            var claimsPrincipal = GetPrincipalFromOldAccessToken(oldAccessToken);
            if (claimsPrincipal is null)
            {
                return null;
            }

            var emailClaim = claimsPrincipal.FindFirstValue(ClaimTypes.Name);
            var roleClaim = claimsPrincipal.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(emailClaim) || string.IsNullOrEmpty(roleClaim))
            {
                return null; 
            }

            return GenerateAccessToken(emailClaim, roleClaim);
        }

        #region helperMethods
        private static ClaimsIdentity GenerateClaims(string userEmail, string role)
        {
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.Name, userEmail));
            claims.AddClaim(new Claim(ClaimTypes.Role, role));

            return claims;
        }

        private ClaimsPrincipal GetPrincipalFromOldAccessToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JwtSettings:PrivateKey"]));
            
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false
            };

            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken is null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
        #endregion
    }
}

