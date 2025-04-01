using BankingSystem.Contracts.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BankingSystem.Application.BackgroundServices
{
    public class DataSeedHostedService : IHostedService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private ILogger<DataSeedHostedService> _logger;

        public DataSeedHostedService(IServiceScopeFactory serviceScopeFactory, ILogger<DataSeedHostedService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Seeding initial data in db if not present!");
            using var scope = _serviceScopeFactory.CreateAsyncScope();
            var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();

            try
            {
                await seeder.SeedDataAsync();
            }
            catch (Exception ex) {
                _logger.LogError("Problem happended during seeding initial data!");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Seeding initial data ended!");
            return Task.CompletedTask;
        }
    }
}
