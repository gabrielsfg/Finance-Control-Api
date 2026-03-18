using FinanceControl.Domain.Interfaces.Service;
using FinanceControl.Services.Extensions;
using FinanceControl.Services.Validations;
using FinanceControl.Shared.Dtos;
using FinanceControl.Shared.Dtos.Request;
using FinanceControl.WebApi.Controllers.Base;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FinanceControl.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IValidator<CreateUserRequestDto> _createUserValidator;
        private readonly IValidator<UserLoginRequestDto> _userLoginValidator;

        public UserController(IUserService userService, IValidator<CreateUserRequestDto> createUserValidator, IValidator<UserLoginRequestDto> userLoginValidator)
        {
            _userService = userService;
            _createUserValidator = createUserValidator;
            _userLoginValidator = userLoginValidator;
        }

        [EnableRateLimiting("auth")]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUserAsync([FromBody]CreateUserRequestDto requestDto)
        {
            var validatonResult = _createUserValidator.Validate(requestDto);
            if (validatonResult.ToActionResult() is { } errorResult)
                return errorResult;

            var token = await _userService.RegisterUserAsync(requestDto);
            if (token is null)
                return BadRequest(new { error = "Email already exists." });

            return StatusCode(201, token);
        }

        [EnableRateLimiting("auth")]
        [HttpPost("login")]
        public async Task<IActionResult> UserLoginAsync([FromBody]UserLoginRequestDto requestDto)
        {
            var validatonResult = _userLoginValidator.Validate(requestDto);
            if (validatonResult.ToActionResult() is { } errorResult)
                return errorResult;

            var result = await _userService.UserLoginAsync(requestDto);

            if (result.IsLocked)
            {
                var remainingSeconds = (int)Math.Ceiling((result.LockoutEnd!.Value - DateTime.UtcNow).TotalSeconds);
                return StatusCode(423, new { error = "Account is locked.", remainingSeconds });
            }

            if (!result.IsSuccess)
                return BadRequest(new { error = "Invalid email or password." });

            return Ok(result.Token);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMeAsync()
        {
            var userId = GetUserId();
            var user = await _userService.GetUserByIdAsync(userId);
            if (user is null)
                return NotFound(new { error = "User not found." });
            return Ok(user);
        }
    }
}
