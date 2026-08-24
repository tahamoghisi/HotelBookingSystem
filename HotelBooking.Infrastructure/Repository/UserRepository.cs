using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Infrastructure.Repository
{
    public class UserRepository : GenericRepository<User> , IUserRepository
    {
        private readonly ApplicationDBContext _context;
        public UserRepository(ApplicationDBContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> ExistByUsername(string username)
        {
            var isExist = await _context.Users.AnyAsync(u => u.UserName == username);
            return isExist;
        }

        public async Task<User?> GetByUserId(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return null;
            return user;
        }

        public async Task<User?> GetByUsernameAndPassword(string username , string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username && u.Password == password);
            if (user == null) return null;
            return user;
        }
    }
}
