using HotelBooking.Application.Common.Models;
using HotelBooking.Application.DTOs.Auth;
using HotelBooking.Application.DTOs.User;
using HotelBooking.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        [HttpGet("test-auth")]
        public IActionResult TestAuth()
        {
            Console.WriteLine("========== CONTROLLER REACHED ==========");
            return Ok("Authentication successful!");
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
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("API works");
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{userId}/password")]
        public async Task<IActionResult> ChangePassword(int userId,AdminChangePasswordDto dto)
        {
            var result = await _authService.ChangePasswordByAdminAsync(userId,dto.newPassword);

            if (!result)
                return NotFound();

            return NoContent();
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var result = await _authService.RefreshTokenAsync(request.refreshToken);

            return Ok(ApiResponse<LoginResponse>.Ok(
                result,
                "Token refreshed successfully."));
        }
        [HttpPost("signout")]
        public async Task<IActionResult> SignOut([FromBody] RefreshTokenRequest request)
        {
            await _authService.LogoutAsync(request.refreshToken);

            return Ok(ApiResponse<object>.Ok(
                null,
                "Signed out successfully."));
        }
    }
}
