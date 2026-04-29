using Microsoft.AspNetCore.Server.Kestrel.Core;
using SmsIntegrationTest.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    if (int.TryParse(Environment.GetEnvironmentVariable("HTTP_PORT"), out var httpPort))
    {
        options.ListenAnyIP(httpPort, listenOptions =>
        {
            listenOptions.Protocols = HttpProtocols.Http1;
        });
    }

    if (int.TryParse(Environment.GetEnvironmentVariable("GRPC_PORT"), out var grpcPort))
    {
        options.ListenAnyIP(grpcPort, listenOptions =>
        {
            listenOptions.Protocols = HttpProtocols.Http2;
        });
    }
}); 

builder.Services.AddGrpc();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.MapGrpcService<SmsService>();

app.MapGet("/", () => "SMS Integration Test Server is running.");

app.Run();