using FinanceControl.Domain.Interfaces.Services;
using FinanceControl.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.WebApi.Controllers
{
    [Route("api/banks")]
    [ApiController]
    [Authorize]
    public class BankController : BaseController
    {
        private readonly IBankService _bankService;

        public BankController(IBankService bankService)
        {
            _bankService = bankService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBanksAsync([FromQuery] string? country)
        {
            var banks = await _bankService.GetBanksAsync(country);
            return Ok(banks);
        }
    }
}
