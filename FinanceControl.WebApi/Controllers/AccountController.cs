using FinanceControl.Domain.Interfaces.Service;
using FinanceControl.Services.Extensions;
using FinanceControl.Services.Validations;
using FinanceControl.Shared.Dtos.Request;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.WebApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;
        private readonly IValidator<CreateAccountRequestDto> _createAccountValidator; 

        public AccountController(IAccountService accountService, IValidator<CreateAccountRequestDto> createAccountValidator)
        {
            _accountService = accountService;
            _createAccountValidator = createAccountValidator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccountAsync([FromBody]CreateAccountRequestDto requestDto)
        {
            var validatorResult = _createAccountValidator.Validate(requestDto);
            if (validatorResult.ToActionResult() is { } errorResult)
                return errorResult;

            var userId = GetUserId();

            var result = await _accountService.CreateAccountAsync(requestDto, userId);

            return Created($"/api/accounts", result.Value);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAccountAsync()
        {
            var userId = GetUserId();

            var result = await _accountService.GetAllAccountAsync(userId);
            return Ok(result);
        }
    }
}
