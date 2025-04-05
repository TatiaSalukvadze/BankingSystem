using BankingSystem.Contracts.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BankingSystem.Application.BackgroundServices
{
    public class DataSeedHostedService : IHostedService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<DataSeedHostedService> _logger;

        public DataSeedHostedService(IServiceScopeFactory serviceScopeFactory, ILogger<DataSeedHostedService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Seeding initial data in db if not present!");          

            try
            {
                using var scope = _serviceScopeFactory.CreateAsyncScope();
                var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
                await seeder.SeedDataAsync();
                _logger.LogInformation("Seeding initial data ended!");
            }
            catch (Exception ex) {
                _logger.LogError("Problem happened during seeding initial data: {message} {type}", ex.Message, ex.GetType().Name);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
