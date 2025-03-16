using BankingSystem.Infrastructure.Identity;
using DbCreation.DbSetup;
using DbCreation.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

var configurationManager = new ConfigurationManager();
configurationManager.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");

Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configurationManager).CreateLogger();

var serviceCollection = new ServiceCollection()
    .AddLogging(loggingBuilder => loggingBuilder.AddSerilog())
    .AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configurationManager.GetConnectionString("dbConnection")))
     //.AddScoped<IDbConnection>((s) => new SqlConnection(configurationManager.GetConnectionString("base")))
    .AddScoped<IDbConnectionFactory>((s) => new DbConnectionFactory(
         configurationManager.GetConnectionString("serverConnection"),configurationManager.GetConnectionString("dbConnection")))
    .AddScoped<IDbSetup, DbSetup>();

var serviceProvider = serviceCollection.BuildServiceProvider();

try
{
    using (var scope = serviceProvider.CreateScope())
    {
        var dbSetup = scope.ServiceProvider.GetRequiredService<IDbSetup>();
        await dbSetup.CreateDbAndTablesAsync();
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

