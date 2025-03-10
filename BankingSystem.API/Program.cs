using BankingSystem.API.Extensions;
using BankingSystem.API.Filters;
using BankingSystem.Infrastructure.Identity;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options => options.Filters.Add<CustomExceptionFilter>())
  .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())); 

builder.Services.AddSwaggerConfiguration();


builder.Services.InjectApplicationDbContext(builder.Configuration).ConfigureIdentity()
    .InjectAuthentication(builder.Configuration).InjectAuthorization()
    .InjectConnectionAndTransaction(builder.Configuration);

builder.Services.InjectServices().InjectRepositories().InjectExternalServices(builder.Configuration);

builder.Host.UseSerilog((context, configuration) =>
configuration.ReadFrom.Configuration(context.Configuration));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using(var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    await seeder.SeedDataAsync();
}

app.Run();
