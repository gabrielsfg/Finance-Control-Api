using FinanceControl.Data.Data;
using FinanceControl.Domain.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace FinanceControl.Services.Services
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ApplicationDbContext _context;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        private const string CurrenciesCacheKey = "exchange_rate_currencies";
        private const string RatesCacheKeyPrefix = "exchange_rate_";

        public ExchangeRateService(
            HttpClient httpClient,
            IMemoryCache cache,
            ApplicationDbContext context,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _cache = cache;
            _context = context;
            _apiKey = configuration["ExchangeRateApi:ApiKey"] ?? string.Empty;
            _baseUrl = configuration["ExchangeRateApi:BaseUrl"] ?? "https://v6.exchangerate-api.com/v6";
        }

        public async Task<IEnumerable<string>> GetAvailableCurrenciesAsync()
        {
            if (_cache.TryGetValue(CurrenciesCacheKey, out List<string>? cached) && cached is not null)
                return cached;

            // Try to get currencies from database (populated by worker)
            var dbCurrencies = await _context.CurrencyRates
                .Select(cr => cr.TargetCurrency)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            if (dbCurrencies.Count > 0)
            {
                _cache.Set(CurrenciesCacheKey, dbCurrencies, TimeSpan.FromMinutes(30));
                return dbCurrencies;
            }

            // Fallback: call API directly if no DB data
            if (!string.IsNullOrWhiteSpace(_apiKey))
            {
                var rates = await FetchFromApiAsync("USD");
                if (rates is not null)
                {
                    var currencies = rates.Keys.OrderBy(k => k).ToList();
                    _cache.Set(CurrenciesCacheKey, currencies, TimeSpan.FromMinutes(30));
                    return currencies;
                }
            }

            return GetDefaultCurrencies();
        }

        public async Task<Dictionary<string, decimal>?> GetExchangeRatesAsync(string baseCurrency)
        {
            var cacheKey = $"{RatesCacheKeyPrefix}{baseCurrency.ToUpper()}";

            if (_cache.TryGetValue(cacheKey, out Dictionary<string, decimal>? cached) && cached is not null)
                return cached;

            // Try to get from database first (populated by worker every 30 min)
            var dbRates = await _context.CurrencyRates
                .Where(cr => cr.BaseCurrency == baseCurrency.ToUpper())
                .Select(cr => new { cr.TargetCurrency, cr.Rate })
                .ToListAsync();

            if (dbRates.Count > 0)
            {
                var rates = dbRates.ToDictionary(r => r.TargetCurrency, r => r.Rate);
                _cache.Set(cacheKey, rates, TimeSpan.FromMinutes(30));
                return rates;
            }

            // Fallback: call API directly if no DB data
            if (string.IsNullOrWhiteSpace(_apiKey))
                return null;

            var apiRates = await FetchFromApiAsync(baseCurrency);
            if (apiRates is not null)
                _cache.Set(cacheKey, apiRates, TimeSpan.FromMinutes(30));

            return apiRates;
        }

        private async Task<Dictionary<string, decimal>?> FetchFromApiAsync(string baseCurrency)
        {
            try
            {
                var url = $"{_baseUrl}/{_apiKey}/latest/{baseCurrency.ToUpper()}";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (!root.TryGetProperty("result", out var resultProp) || resultProp.GetString() != "success")
                    return null;

                if (!root.TryGetProperty("conversion_rates", out var ratesProp))
                    return null;

                var rates = new Dictionary<string, decimal>();
                foreach (var rate in ratesProp.EnumerateObject())
                    rates[rate.Name] = rate.Value.GetDecimal();

                return rates;
            }
            catch
            {
                return null;
            }
        }

        private static List<string> GetDefaultCurrencies()
        {
            return
            [
                "AED","AFN","ALL","AMD","ANG","AOA","ARS","AUD","AWG","AZN",
                "BAM","BBD","BDT","BGN","BHD","BMD","BND","BOB","BRL","BSD",
                "BWP","BYN","BZD","CAD","CHF","CLP","CNY","COP","CRC","CZK",
                "DKK","DOP","DZD","EGP","ETB","EUR","FJD","GBP","GEL","GHS",
                "GTQ","HKD","HNL","HRK","HUF","IDR","ILS","INR","IQD","IRR",
                "ISK","JMD","JOD","JPY","KES","KGS","KHR","KRW","KWD","KYD",
                "KZT","LAK","LBP","LKR","MAD","MDL","MKD","MMK","MXN","MYR",
                "NAD","NGN","NIO","NOK","NPR","NZD","OMR","PAB","PEN","PGK",
                "PHP","PKR","PLN","PYG","QAR","RON","RSD","RUB","SAR","SCR",
                "SEK","SGD","SLL","SOS","SRD","THB","TND","TRY","TTD","TWD",
                "TZS","UAH","UGX","USD","UYU","UZS","VES","VND","XAF","XCD",
                "XOF","YER","ZAR","ZMW"
            ];
        }
    }
}
