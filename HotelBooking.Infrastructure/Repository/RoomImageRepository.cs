using HotelBooking.Domain.Entities.Images;
using HotelBooking.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Infrastructure.Repository
{
    public class RoomImageRepository : IRoomImageRepository
    {
        private readonly ApplicationDBContext _dbContext;
        public RoomImageRepository(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddRangeAsync(IEnumerable<RoomImage> images)
        {
            await _dbContext.AddRangeAsync(images);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> HasMainImageAsync(int roomId)
        {
            return await _dbContext.RoomImages.AnyAsync(i => i.RoomId ==  roomId && i.IsMain);
        }
    }
}
