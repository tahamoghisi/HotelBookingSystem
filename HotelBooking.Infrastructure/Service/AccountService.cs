using HotelBooking.Application.DTOs.Auth;
using HotelBooking.Application.DTOs.User;
using HotelBooking.Application.Mapping.UserMap;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Infrastructure.Service
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IUserRepository _userRepository;
        private readonly IJWTService _jwtService;
        public AccountService(IAccountRepository accountRepository, IUserRepository userRepository, IJWTService jwtService)
        {
            _accountRepository = accountRepository;
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            var user = await _userRepository.GetByUsernameAndPassword(loginRequest.userName , loginRequest.password);
            if (user == null) throw new Exception("UserName or Password is incorrect");
            var token = _jwtService.GenerateToken(user);
            return new LoginResponse
            {
                AccessToken = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15)
            };
        }

        public async Task<bool> Register(RegisterDto registerDto)
        {
            if (registerDto == null)
            {
                return false;
            }
            else
            {
                var user = UserMapping.ToEntity(registerDto);
                var check = await _accountRepository.Register(user);
                if (check == false) return false;
                return true;
            }   
        }
    }
}
