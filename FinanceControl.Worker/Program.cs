using FinanceControl.Data.Data;
using FinanceControl.Worker;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddHttpClient("ExchangeRate");

builder.Services.AddHostedService<Worker>();
builder.Services.AddHostedService<CurrencyRateWorker>();

var host = builder.Build();
host.Run();
