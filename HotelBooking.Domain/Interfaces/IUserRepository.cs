using HotelBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAndPassword(string username , string passsword);
        Task<User?> GetByUserId(int userId);
    }
}
