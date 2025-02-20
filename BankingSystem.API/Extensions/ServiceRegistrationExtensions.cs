﻿using BankingSystem.Application.Services;
using BankingSystem.Contracts.Interfaces.IServices;
using BankingSystem.Contracts.Interfaces;
using BankingSystem.Infrastructure;
using BankingSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BankingSystem.Contracts.Interfaces.IRepositories;
using BankingSystem.Infrastructure.DataAccess.Repositories;
using BankingSystem.Infrastructure.DataAccess;
using BankingSystem.Contracts.Interfaces.IExternalServices;
using BankingSystem.Infrastructure.Configuration;
using BankingSystem.Infrastructure.ExternalServices;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BankingSystem.API.Extensions
{
    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection InjectApplicationDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options => 
            options.UseSqlServer(configuration.GetConnectionString("default")));

            return services;
        }
        public static IServiceCollection InjectConnectionAndTransaction(this IServiceCollection services, IConfiguration configuration)
        {
            //for db operations(unitofwork and repos)
            services.AddScoped((s) => new SqlConnection(configuration.GetConnectionString("Default")));
            services.AddScoped<IDbTransaction>(s =>
            {
                SqlConnection conn = s.GetRequiredService<SqlConnection>();
                conn.Open();
                return conn.BeginTransaction();
            });

            return services;
        }
        public static IServiceCollection InjectServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPersonService, PersonService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ICardService, CardService>();
            services.AddScoped<ITransactionService, TransactionService>();

            return services;
        }
        public static IServiceCollection InjectRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ICardRepository, CardRepository>();
            services.AddScoped<ITransactionDetailsRepository, TransactionDetailsRepository>();

            return services;
        }
        public static IServiceCollection InjectExternalServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CurrencyApiSettings>(configuration.GetSection("CurrencyApiSettings"));
            services.AddHttpClient<ICurrencyService, CurrencyService>();
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.AddScoped<IEmailService, EmailService>();

            return services;
        }
    }
}
