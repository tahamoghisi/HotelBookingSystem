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
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDBContext _dbContext;
        public RefreshTokenRepository(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(RefreshToken refreshToken)
        {
            await _dbContext.RefreshToken.AddAsync(refreshToken);
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _dbContext.RefreshToken.Include(t => t.User).FirstOrDefaultAsync(t => t.Token == token);
        }
    }
}
