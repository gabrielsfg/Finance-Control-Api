using FinanceControl.Data.Data;
using FinanceControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FinanceControl.Worker;

public class CurrencyRateWorker : BackgroundService
{
    private readonly ILogger<CurrencyRateWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(30);

    public CurrencyRateWorker(
        ILogger<CurrencyRateWorker> logger,
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("CurrencyRate Worker started. Interval: {Interval} minutes.", Interval.TotalMinutes);

        // Run immediately on startup, then every 30 minutes
        while (!stoppingToken.IsCancellationRequested)
        {
            await RunJobAsync(stoppingToken);
            await Task.Delay(Interval, stoppingToken);
        }
    }

    private async Task RunJobAsync(CancellationToken stoppingToken)
    {
        var apiKey = _configuration["ExchangeRateApi:ApiKey"];
        var baseUrl = _configuration["ExchangeRateApi:BaseUrl"] ?? "https://v6.exchangerate-api.com/v6";

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogWarning("CurrencyRate Worker skipped: ExchangeRateApi:ApiKey not configured.");
            return;
        }

        var baseCurrencies = (_configuration["ExchangeRateApi:BaseCurrencies"] ?? "USD,BRL,EUR")
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        _logger.LogInformation("CurrencyRate job started at {Time} UTC for bases: {Bases}.",
            DateTime.UtcNow, string.Join(", ", baseCurrencies));

        var client = _httpClientFactory.CreateClient("ExchangeRate");
        var fetchedAt = DateTime.UtcNow;
        int updated = 0;
        int errors = 0;

        foreach (var baseCurrency in baseCurrencies)
        {
            if (stoppingToken.IsCancellationRequested) break;

            try
            {
                var url = $"{baseUrl}/{apiKey}/latest/{baseCurrency.ToUpper()}";
                var response = await client.GetAsync(url, stoppingToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("ExchangeRate API returned {Status} for base {Base}.",
                        response.StatusCode, baseCurrency);
                    errors++;
                    continue;
                }

                var json = await response.Content.ReadAsStringAsync(stoppingToken);
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (!root.TryGetProperty("result", out var resultProp) || resultProp.GetString() != "success")
                {
                    _logger.LogWarning("ExchangeRate API returned non-success result for base {Base}.", baseCurrency);
                    errors++;
                    continue;
                }

                if (!root.TryGetProperty("conversion_rates", out var ratesProp))
                {
                    errors++;
                    continue;
                }

                var rates = new Dictionary<string, decimal>();
                foreach (var rate in ratesProp.EnumerateObject())
                    rates[rate.Name] = rate.Value.GetDecimal();

                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                foreach (var (target, rate) in rates)
                {
                    var existing = await context.CurrencyRates
                        .FirstOrDefaultAsync(
                            cr => cr.BaseCurrency == baseCurrency.ToUpper() && cr.TargetCurrency == target,
                            stoppingToken);

                    if (existing is not null)
                    {
                        existing.Rate = rate;
                        existing.FetchedAt = fetchedAt;
                    }
                    else
                    {
                        context.CurrencyRates.Add(new CurrencyRate
                        {
                            BaseCurrency = baseCurrency.ToUpper(),
                            TargetCurrency = target,
                            Rate = rate,
                            FetchedAt = fetchedAt
                        });
                    }

                    updated++;
                }

                await context.SaveChangesAsync(stoppingToken);
                _logger.LogInformation("Updated {Count} rates for base {Base}.", rates.Count, baseCurrency);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                errors++;
                _logger.LogError(ex, "Error fetching rates for base currency {Base}.", baseCurrency);
            }
        }

        _logger.LogInformation(
            "CurrencyRate job finished. Updated={Updated}, Errors={Errors}.", updated, errors);
    }
}
