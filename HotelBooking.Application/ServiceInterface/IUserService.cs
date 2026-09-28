using HotelBooking.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Application.ServiceInterface
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetAllAsync();    
    }
}
