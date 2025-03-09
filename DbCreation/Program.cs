//using Microsoft.AspNetCore.Identity;

using BankingSystem.Infrastructure.Identity;
using DbCreation;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data;


var configurationManager = new ConfigurationManager();
configurationManager.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");


var serviceCollection = new ServiceCollection()
    //.AddSingleton<IConfiguration>(configurationManager)
    .AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configurationManager.GetConnectionString("default")))
     .AddScoped<IDbConnection>((s) => new SqlConnection(configurationManager.GetConnectionString("base")))
     .AddScoped<DbSetup>();
//.AddIdentity<IdentityUser, IdentityRole>(options =>
//        {
//            options.Password.RequireDigit = true;
//            options.Password.RequireLowercase = true;
//            options.Password.RequireUppercase = true;
//            options.Password.RequireNonAlphanumeric = true;
//            options.Password.RequiredLength = 8;
//        }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();




var serviceProvider = serviceCollection.BuildServiceProvider();


try
{
    using (var scope = serviceProvider.CreateScope())
    {
        var dbSetup = scope.ServiceProvider.GetRequiredService<DbSetup>();
        await dbSetup.CreateDbAndTables();

    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}


//async Task ApplyMigrations(ServiceProvider serviceProvider)
//{

//        var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

//            await dbContext.Database.MigrateAsync();

//    }
