using HotelBooking.Application.DTOs.User;
using HotelBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Application.Mapping.UserMap
{
    public static class UserMapping
    {
        public static User ToEntity(RegisterDto registerDto)
        {
            var user = new User
            {
                UserName = registerDto.UserName,
                Password = registerDto.Password
            };
            return user;
        }
    }
}
