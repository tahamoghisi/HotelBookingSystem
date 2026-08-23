using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Application.ServiceInterface
{
    public class JWTService : IJWTService
    {
        private readonly IConfiguration configuration;
        public JWTService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public string GenerateToken(User user)
        {
            var claim = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,
                user.Id.ToString()),
                new Claim(ClaimTypes.Name,
                user.UserName),
                new Claim(ClaimTypes.Role,
                user.Role)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
            var credential = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(15);
            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claim,
                expires: expires,
                signingCredentials: credential);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
