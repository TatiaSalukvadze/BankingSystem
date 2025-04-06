using BankingSystem.Application.Services;
using BankingSystem.Application.FacadeServices;
using BankingSystem.Contracts.Interfaces.IServices;
using BankingSystem.Contracts.Interfaces;
using BankingSystem.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using BankingSystem.Contracts.Interfaces.IRepositories;
using BankingSystem.Infrastructure.DataAccess.Repositories;
using BankingSystem.Infrastructure.DataAccess;
using BankingSystem.Contracts.Interfaces.IExternalServices;
using BankingSystem.Infrastructure.ExternalServices;
using Microsoft.Data.SqlClient;
using BankingSystem.Infrastructure.ExternalServices.Configuration;
using BankingSystem.Infrastructure.Auth;
using BankingSystem.Application.BackgroundServices;
using BankingSystem.Application.HelperServices;

namespace BankingSystem.API.Extensions
{
    public static class ServiceRegistration
    {
        public static IServiceCollection InjectApplicationDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options => 
            options.UseSqlServer(configuration.GetConnectionString("default")));

            return services;
        }

        public static IServiceCollection InjectDbConnection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped((s) => new SqlConnection(configuration.GetConnectionString("default")));
    
            return services;
        }

        public static IServiceCollection InjectServices(this IServiceCollection services)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IPersonService, PersonService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ICardService, CardService>();
            services.AddScoped<ITransactionDetailsService, TransactionDetailsService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITransactionOperationService, TransactionOperationService>();
            services.AddScoped<IHashingService, HashingService>();
            services.AddScoped<IEncryptionService, EncryptionService>();
            services.AddMemoryCache();
            services.AddHostedService<DataSeedHostedService>();
            services.AddHostedService<CleanupExpiredTokensService>();

            return services;
        }

        public static IServiceCollection InjectRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ICardRepository, CardRepository>();
            services.AddScoped<ITransactionDetailsRepository, TransactionDetailsRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            return services;
        }

        public static IServiceCollection InjectExternalServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CurrencyApiSettings>(configuration.GetSection("CurrencyApiSettings"));
            services.AddHttpClient<IExchangeRateService, ExchangeRateService>();
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.AddScoped<IEmailService, EmailService>();

            return services;
        }
    }
}
