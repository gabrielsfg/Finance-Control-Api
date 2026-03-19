using FinanceControl.Domain.Interfaces.Services;
using FinanceControl.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.WebApi.Controllers
{
    [Route("api/currencies")]
    [ApiController]
    [Authorize]
    public class CurrencyController : BaseController
    {
        private readonly IExchangeRateService _exchangeRateService;

        public CurrencyController(IExchangeRateService exchangeRateService)
        {
            _exchangeRateService = exchangeRateService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrenciesAsync()
        {
            var currencies = await _exchangeRateService.GetAvailableCurrenciesAsync();
            return Ok(new { currencies });
        }

        [HttpGet("rates")]
        public async Task<IActionResult> GetRatesAsync([FromQuery] string baseCurrency = "BRL")
        {
            var rates = await _exchangeRateService.GetExchangeRatesAsync(baseCurrency);
            if (rates is null)
                return Ok(new { baseCurrency, rates = new Dictionary<string, decimal>(), message = "Exchange rate API not configured." });

            return Ok(new { baseCurrency, rates });
        }
    }
}
