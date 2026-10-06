using HotelBooking.Application.DTOs.Auth;
using HotelBooking.Application.DTOs.User;
using HotelBooking.Application.Mapping.UserMap;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Interfaces;
using Microsoft.Extensions.Logging;
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
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<AuthService> _logger;
        public AuthService(IJWTService jwtService,IUnitOFWork unitOFWork,IPasswordHasher passwordHasher, ILogger<AuthService> logger)
        {
            _passwordHasher = passwordHasher;
            _unitOFWork = unitOFWork;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<bool> ChangePasswordByAdminAsync(int userId, string newPassword)
        {
            var user = await _unitOFWork.User.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("user {UserId} not found.", userId);
                return false;
            }
            var passwordHash = _passwordHasher.Hash(newPassword);
            user.Password = passwordHash;
            _unitOFWork.User.Update(user);
            await _unitOFWork.SaveChangesAsync();
            _logger.LogInformation("User {UserId} password updated successfully.", user.Id);
            return true;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            var user = await _unitOFWork.User.GetByUsername(loginRequest.userName);
            if (user == null)
            {
                _logger.LogWarning("username {UserName} or password is incorrect.", loginRequest.userName);
                throw new UnauthorizedAccessException("UserName or Password is incorrect");
            }
               
            var isPasswordValid = _passwordHasher.Verify(loginRequest.password, user.Password);
            if (!isPasswordValid)
            {
                _logger.LogWarning("username {UserName} or password is incorrect.", loginRequest.userName);
                throw new UnauthorizedAccessException("UserName or Password is incorrect");
            }
            _logger.LogInformation("User {UserId} logined successfully.", user.Id);
            var accesstoken = _jwtService.GenerateToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();
            var RT = new RefreshToken
            {
                Token = refreshToken,
                ExpireAt = DateTime.UtcNow.AddDays(30),
                IsRevoked = false,
                UserId = user.Id,
                User = user
            };
            await _unitOFWork.RefreshToken.AddAsync(RT);
            await _unitOFWork.SaveChangesAsync();
            return new LoginResponse
            {
                AccessToken = accesstoken.AccessToken,
                ExpiresAt = accesstoken.ExpiresAt,
                RefreshToken = refreshToken,
            };
        }

        public async Task LogoutAsync(string refreshToken)
        {
            var RT = await _unitOFWork.RefreshToken.GetByTokenAsync(refreshToken);
            if (RT == null)
            {
                throw new UnauthorizedAccessException("Invalid refresh token.");
            }
            RT.IsRevoked = true;
            await _unitOFWork.SaveChangesAsync();
        }

        public async Task<LoginResponse> RefreshTokenAsync(string refreshToken)
        {
            var RT = await _unitOFWork.RefreshToken.GetByTokenAsync(refreshToken);
            if (RT  == null)
            {
                throw new UnauthorizedAccessException("Invalid refresh token.");
            }
            if (RT.IsRevoked)
            {
                _logger.LogWarning("Refresh token {RefreshTokenId} is revoked.", RT.Id);
                throw new UnauthorizedAccessException("Refresh token has been revoked.");
            }
            if (RT.ExpireAt <= DateTime.UtcNow)
            {
                _logger.LogWarning("Refresh token {RefreshTokenId} has expired.", RT.Id);
                throw new UnauthorizedAccessException("Refresh token has expired.");
            }
            RT.IsRevoked = true;

            var newRefreshToken = _jwtService.GenerateRefreshToken();

            var newRT = new RefreshToken
            {
                Token = newRefreshToken,
                ExpireAt = DateTime.UtcNow.AddDays(30),
                IsRevoked = false,
                UserId = RT.UserId,
                User = RT.User
            };
            await _unitOFWork.SaveChangesAsync();
            await _unitOFWork.RefreshToken.AddAsync(newRT);
            var accesstoken = _jwtService.GenerateToken(RT.User);
            return new LoginResponse
            {
                AccessToken = accesstoken.AccessToken,
                ExpiresAt = accesstoken.ExpiresAt,
                RefreshToken = newRT.Token,
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
            if (isExist)
            {
                _logger.LogWarning("username {UserName} already exist.", registerRequest.Username);
                throw new Exception("UserName already Exist");
            }
            var passwordHash = _passwordHasher.Hash(registerRequest.Password);
            var user = new User
            {
                UserName = registerRequest.Username,
                Password = passwordHash,
                Role = "User"
                //email , phoneNumber , nationalCode
            };
            var customer = new Customer
            {
                FullName = registerRequest.Username,
                Email = registerRequest.Email,
                PhoneNumber = registerRequest.PhoneNumber,
                NationalCode = registerRequest.NationalCode,
                User = user
            };
            await _unitOFWork.User.AddAsync(user);
            await _unitOFWork.Customers.AddAsync(customer);
            await _unitOFWork.SaveChangesAsync();
            _logger.LogInformation("User {UserId} registered successfully.",user.Id);
            var token = _jwtService.GenerateToken(user);
            return new RegisterResponse
            {
                AccessToken = token.AccessToken,
                ExpiresAt = token.ExpiresAt
            };
        }
    }
}
