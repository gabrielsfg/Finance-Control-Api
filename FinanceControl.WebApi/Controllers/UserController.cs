using FinanceControl.Domain.Interfaces.Service;
using FinanceControl.Shared.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUserAsync([FromBody]CreateUserRequestDto requestDto)
        {
            var user = await _userService.RegisterUserAsync(requestDto);
            if (user is null)
                return BadRequest("Email already existis.");

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> UserLoginAsync([FromBody]UserLoginRequestDto requestDto)
        {
            var token = await _userService.UserLoginAsync(requestDto);
            if (token is null)
                return BadRequest("Invalid email or password.");
            return Ok(token);
        }
    }
}
