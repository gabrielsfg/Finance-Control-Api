namespace FinanceControl.Domain.Interfaces.Services
{
    public interface IExchangeRateService
    {
        Task<IEnumerable<string>> GetAvailableCurrenciesAsync();
        Task<Dictionary<string, decimal>?> GetExchangeRatesAsync(string baseCurrency);
    }
}
