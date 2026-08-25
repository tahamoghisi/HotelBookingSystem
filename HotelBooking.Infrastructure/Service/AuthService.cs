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
    public class AuthService : IAuthService
    {
        private readonly IJWTService _jwtService;
        private readonly IUnitOFWork _unitOFWork;
        public AuthService(IJWTService jwtService,IUnitOFWork unitOFWork)
        {
            _unitOFWork = unitOFWork;
            _jwtService = jwtService;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            var user = await _unitOFWork.User.GetByUsernameAndPassword(loginRequest.userName , loginRequest.password);
            if (user == null) throw new Exception("UserName or Password is incorrect");
            var token = _jwtService.GenerateToken(user);
            return new LoginResponse
            {
                AccessToken = token.AccessToken,
                ExpiresAt = token.ExpiresAt
            };
        }
        //public async Task<bool> Register(RegisterDto registerDto)
        //{
        //    if (registerDto == null)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        var user = UserMapping.ToEntity(registerDto);
        //        var check = await _accountRepository.Register(user);
        //        if (check == false) return false;
        //        return true;
        //    }   
        //}

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest registerRequest)
        {
            var isExist = await _unitOFWork.User.ExistByUsername(registerRequest.Username);
            if (isExist) throw new Exception("UserName is already Exist");
            var user = new User
            {
                UserName = registerRequest.Username,
                Password = registerRequest.Password,
                Role = "User"
                //email , phoneNumber , nationalCode
            };
            await _unitOFWork.User.AddAsync(user);
            await _unitOFWork.SaveChangesAsync();
            var token = _jwtService.GenerateToken(user);
            return new RegisterResponse
            {
                AccessToken = token.AccessToken,
                ExpiresAt = token.ExpiresAt
            };
        }
    }
}
