using BankingSystem.Contracts.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BankingSystem.Application.BackgroundServices
{
    public class CleanupExpiredTokensService : BackgroundService
    {
        private readonly ILogger<CleanupExpiredTokensService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public CleanupExpiredTokensService(ILogger<CleanupExpiredTokensService> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("CleanupExpiredTokensService is running.");
                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    int rowsAffected = await unitOfWork.RefreshTokenRepository.DeleteExpiredRefreshTokensAsync();
                    _logger.LogInformation("CleanupExpiredTokensService deleted {rowsAffected} expired tokens.", rowsAffected);
                }
                catch(Exception ex)
                {
                    _logger.LogError("An error occurred while cleaning up expired tokens: {message} {type}", ex.Message, ex.GetType().Name);
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}
