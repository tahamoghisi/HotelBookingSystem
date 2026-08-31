using HotelBooking.Application.DTOs.Auth;
using HotelBooking.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
              _authService = authService;
        }
        public IActionResult Index()
        {
            return Ok();
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var user = await _authService.LoginAsync(loginRequest);
            return Ok(user);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            var user = await _authService.RegisterAsync(registerRequest);
            return Ok(user);
        }
    }
}
