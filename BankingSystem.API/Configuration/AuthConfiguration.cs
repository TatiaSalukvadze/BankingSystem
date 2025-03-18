using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace BankingSystem.API.Extensions
{
    public static class AuthConfiguration
    {
        public static IServiceCollection InjectAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services
            .AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["JwtSettings:PrivateKey"]!)),
                    ValidateIssuer = true,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["JwtSettings:Audience"]
                };
            });

            return services;
        }

        public static IServiceCollection InjectAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(
                options =>
                {
                    options.AddPolicy("ManagerOnly", policy =>
                        policy.RequireClaim(ClaimTypes.Role, "Manager"));
                    options.AddPolicy("OperatorOnly", policy =>
                        policy.RequireClaim(ClaimTypes.Role, "Operator"));
                    options.AddPolicy("UserOnly", policy =>
                        policy.RequireClaim(ClaimTypes.Role, "User"));
                }
            );

            return services;
        }
    }
}
