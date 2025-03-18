using BankingSystem.API.Extensions;
using BankingSystem.API.Filters;
using BankingSystem.API.Middlewares;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options => options.Filters.Add<CustomExceptionFilter>())
  .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())); 

builder.Services.AddSwaggerConfiguration();

builder.Services.InjectApplicationDbContext(builder.Configuration).ConfigureIdentity()
    .InjectAuthentication(builder.Configuration).InjectAuthorization()
    .InjectDbConnection(builder.Configuration);

builder.Services.InjectServices().InjectRepositories().InjectExternalServices(builder.Configuration);

builder.Host.UseSerilog((context, configuration) =>
configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseMiddleware<ResponseCachingMiddleware>();
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
