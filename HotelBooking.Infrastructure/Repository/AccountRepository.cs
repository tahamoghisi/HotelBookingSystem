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
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly IUnitOFWork _unitOFWork;
        public AccountRepository(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Task<bool> Login(User loginUser)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Register(User User)
        {
            var check = await _dbContext.Users.AnyAsync(u => u.UserName == User.UserName);
            if (check) return false;
            await _dbContext.Users.AddAsync(User);
            await _unitOFWork.SaveChangesAsync();
            return true;
        }
    }
}
