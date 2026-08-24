using HotelBooking.Application.DTOs.Auth;
using HotelBooking.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Domain.Interfaces
{
    public interface IAuthService
    {
        Task<bool> Register(RegisterDto registerDto);
        Task<LoginResponse> LoginAsync(LoginRequest loginRequest);
        Task<RegisterResponse> RegisterAsync(RegisterRequest registerRequest);
    }
}
