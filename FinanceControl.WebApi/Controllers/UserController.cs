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
using Microsoft.Extensions.Configuration;

namespace FinanceControl.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IValidator<CreateUserRequestDto> _createUserValidator;
        private readonly IValidator<UserLoginRequestDto> _userLoginValidator;
        private readonly IConfiguration _configuration;

        public UserController(IUserService userService, IValidator<CreateUserRequestDto> createUserValidator, IValidator<UserLoginRequestDto> userLoginValidator, IConfiguration configuration)
        {
            _userService = userService;
            _createUserValidator = createUserValidator;
            _userLoginValidator = userLoginValidator;
            _configuration = configuration;
        }

        [EnableRateLimiting("auth")]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUserAsync([FromBody] CreateUserRequestDto requestDto)
        {
            var validatonResult = _createUserValidator.Validate(requestDto);
            if (validatonResult.ToActionResult() is { } errorResult)
                return errorResult;

            var auth = await _userService.RegisterUserAsync(requestDto);
            if (auth is null)
                return BadRequest(new { error = "Email already exists." });

            return StatusCode(201, auth);
        }

        [EnableRateLimiting("auth")]
        [HttpPost("login")]
        public async Task<IActionResult> UserLoginAsync([FromBody] UserLoginRequestDto requestDto)
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

            return Ok(result.Auth);
        }

        [EnableRateLimiting("auth")]
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequestDto requestDto)
        {
            if (string.IsNullOrWhiteSpace(requestDto.RefreshToken))
                return BadRequest(new { error = "Refresh token is required." });

            var auth = await _userService.RefreshTokenAsync(requestDto.RefreshToken);
            if (auth is null)
                return Unauthorized(new { error = "Invalid or expired refresh token." });

            return Ok(auth);
        }

        [EnableRateLimiting("auth")]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordRequestDto requestDto)
        {
            if (string.IsNullOrWhiteSpace(requestDto.Email))
                return BadRequest(new { error = "Email is required." });

            await _userService.ForgotPasswordAsync(requestDto.Email);

            // Sempre 200 para não revelar se o e-mail existe
            return Ok(new { message = "If an account with that email exists, a reset link has been sent." });
        }

        [EnableRateLimiting("auth")]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordRequestDto requestDto)
        {
            if (string.IsNullOrWhiteSpace(requestDto.Token) || string.IsNullOrWhiteSpace(requestDto.NewPassword))
                return BadRequest(new { error = "Token and new password are required." });

            var success = await _userService.ResetPasswordAsync(requestDto.Token, requestDto.NewPassword);
            if (!success)
                return BadRequest(new { error = "Invalid or expired reset token." });

            return Ok(new { message = "Password updated successfully." });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync([FromBody] RefreshTokenRequestDto requestDto)
        {
            if (string.IsNullOrWhiteSpace(requestDto.RefreshToken))
                return BadRequest(new { error = "Refresh token is required." });

            await _userService.LogoutAsync(requestDto.RefreshToken);
            return NoContent();
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmailAsync([FromQuery] string token)
        {
            var frontendUrl = _configuration["AppSettings:FrontendUrl"];

            if (string.IsNullOrWhiteSpace(token))
                return Redirect($"{frontendUrl}/verify-email?success=false");

            var success = await _userService.VerifyEmailAsync(token);

            return success
                ? Redirect($"{frontendUrl}/verify-email?success=true")
                : Redirect($"{frontendUrl}/verify-email?success=false");
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

        [Authorize]
        [HttpPatch("me")]
        public async Task<IActionResult> PatchMeAsync([FromBody] PatchUserRequestDto requestDto)
        {
            var userId = GetUserId();
            var user = await _userService.UpdateUserAsync(userId, requestDto);
            if (user is null)
                return NotFound(new { error = "User not found." });
            return Ok(user);
        }

        [Authorize]
        [HttpDelete("me")]
        public async Task<IActionResult> DeleteMeAsync([FromBody] ConfirmPasswordRequestDto requestDto)
        {
            if (string.IsNullOrWhiteSpace(requestDto.Password))
                return BadRequest(new { error = "Password is required." });

            var userId = GetUserId();
            var success = await _userService.DeleteUserAsync(userId, requestDto.Password);
            if (!success)
                return BadRequest(new { error = "Invalid password." });

            return NoContent();
        }

        [Authorize]
        [HttpPost("me/reset-data")]
        public async Task<IActionResult> ResetDataAsync([FromBody] ConfirmPasswordRequestDto requestDto)
        {
            if (string.IsNullOrWhiteSpace(requestDto.Password))
                return BadRequest(new { error = "Password is required." });

            var userId = GetUserId();
            var success = await _userService.ResetUserDataAsync(userId, requestDto.Password);
            if (!success)
                return BadRequest(new { error = "Invalid password." });

            return Ok(new { message = "All financial data has been deleted." });
        }
    }
}
