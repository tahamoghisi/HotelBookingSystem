using HotelBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Domain.Interfaces
{
    public interface IAccountRepository
    {
        Task<bool> Register(User registerUser);
        Task<bool> Login(User loginUser);
    }
}
