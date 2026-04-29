using Google.Protobuf;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Sms.Application.Interfaces;
using Sms.ConsoleApp.Data;
using Sms.ConsoleApp.Services;
using Sms.Infrastructure.Servises;
using SmsConsole.Services;

namespace Sms.ConsoleApp;

class Program
{
    static async Task Main(string[] args)
    {
        // Настройка Serilog
        var logFile = Path.Combine("logs", "test-sms-console-app-.log");
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            //.WriteTo.Console()
            .WriteTo.File(logFile, rollingInterval: RollingInterval.Day)
            .CreateLogger();

        try
        {
            var host = CreateHostBuilder(args).Build();

            var dbContext = host.Services.GetRequiredService<SmsDbContext>();
            await dbContext.Database.EnsureCreatedAsync();

            var app = host.Services.GetRequiredService<SmsConsoleService>();
            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Приложение завершилось с ошибкой");
        }
        finally
        {
            Log.CloseAndFlush();
        }

        Console.WriteLine("Нажмите любую кнопку для завершения работы");
        Console.ReadKey();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                var configuration = context.Configuration;

                services.AddDbContext<SmsDbContext>(options =>
                    options.UseNpgsql(configuration.GetConnectionString("Postgres")));

                services.AddScoped<IMenuRepository, MenuRepository>();

                var clientType = configuration["SmsClient:Type"]?.ToLowerInvariant();
                services.AddSingleton<ISmsClient>(sp =>
                {
                    var section = configuration.GetSection("SmsClient");
                    return clientType switch
                    {
                        "http" => new HttpSmsClient(
                            section["BaseUrl"],
                            section["Username"],
                            section["Password"],
                            section["Endpoint"]),
                        "grpc" => new GrpcSmsClient(section["GrpcAddress"]),
                        _ => throw new InvalidOperationException($"Неизвестный тип клиента: {clientType}")
                    };
                });

                services.AddScoped<IOrderService, OrderService>();
                services.AddSingleton<IConsoleService, ConsoleService>();
                services.AddScoped<SmsConsoleService>();

                // Настройка логирования через Microsoft.Extensions.Logging + Serilog
                services.AddLogging(loggingBuilder =>
                {
                    loggingBuilder.ClearProviders();
                    loggingBuilder.AddSerilog(dispose: true);
                });
            })
            .UseSerilog(); // интеграция Serilog в Generic Host
}