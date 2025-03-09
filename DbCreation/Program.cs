//using Microsoft.AspNetCore.Identity;

using BankingSystem.Infrastructure.Identity;
using DbCreation.DbSetup;
using DbCreation.Helpers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data;


var configurationManager = new ConfigurationManager();
configurationManager.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");


var serviceCollection = new ServiceCollection()
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
        await dbSetup.CreateDbAndTables();

    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

