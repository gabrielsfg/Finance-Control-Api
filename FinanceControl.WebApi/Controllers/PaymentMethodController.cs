using FinanceControl.Domain.Interfaces.Service;
using FinanceControl.Shared.Helpers;
using FinanceControl.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.WebApi.Controllers
{
    [Route("api/payment-methods")]
    [ApiController]
    [Authorize]
    public class PaymentMethodController : BaseController
    {
        private readonly IUserService _userService;

        public PaymentMethodController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaymentMethodsAsync([FromQuery] string? country)
        {
            string? countryCode = country;

            // If no country provided, use user's country
            if (string.IsNullOrWhiteSpace(countryCode))
            {
                var userId = GetUserId();
                var user = await _userService.GetUserByIdAsync(userId);
                countryCode = user?.Country;
            }

            var methods = PaymentMethodsByCountry.GetForCountry(countryCode);

            return Ok(new { country = countryCode, paymentMethods = methods });
        }
    }
}
