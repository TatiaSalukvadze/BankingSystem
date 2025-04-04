﻿using BankingSystem.Contracts.Interfaces;
using BankingSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace BankingSystem.API.Extensions
{
    public static class IdentityConfiguration
    {
        public static IServiceCollection ConfigureIdentity(this IServiceCollection services)
        {
            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
            
            services.AddTransient<IDataSeeder,DataSeeder>();

            return services;
        }
    }
}
