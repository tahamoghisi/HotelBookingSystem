using HotelBooking.Application.DTOs.Auth;
using HotelBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Domain.Interfaces
{
    public interface IJWTService
    {
        ResultToken GenerateToken (User user);
    }
}
