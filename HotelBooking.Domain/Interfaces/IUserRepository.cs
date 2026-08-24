using HotelBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Domain.Interfaces
{
    public interface IUserRepository : IGenericRepositoy<User>
    {
        Task<User?> GetByUsernameAndPassword(string username , string passsword);
        Task<User?> GetByUserId(int userId);
        Task<bool> ExistByUsername(string username);

    }
}
