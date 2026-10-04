using HotelBooking.Domain.Entities;
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

        public async Task<bool> DeleteAsync(int imageId)
        {
            var image = await _dbContext.RoomImages.FirstOrDefaultAsync(x => x.Id == imageId);
            if (image == null) return false;
            _dbContext.RoomImages.Remove(image);
            return true;
        }

        public async Task<RoomImage?> GetByIdAsync(int roomId, int imageId)
        {
            return await _dbContext.RoomImages.FirstOrDefaultAsync(i => i.RoomId == roomId && i.Id == imageId);
        }

        public async Task<RoomImage?> GetFirstImageAsync(int roomId)
        {
            return await _dbContext.RoomImages.Where(i => i.RoomId == roomId).OrderBy(i => i.Id).FirstOrDefaultAsync();
        }

        public async Task<bool> HasMainImageAsync(int roomId)
        {
            return await _dbContext.RoomImages.AnyAsync(i => i.RoomId ==  roomId && i.IsMain);
        }

        public async Task<bool> SetMainImageAsync(int roomId, int imageId)
        {
            var image = await _dbContext.RoomImages.FirstOrDefaultAsync(i => i.RoomId == roomId && i.Id == imageId);
            if (image == null) return false;

            var roomImages = await _dbContext.RoomImages
                .Where(i => i.RoomId == roomId)
                .ToListAsync();

            foreach (var roomImage in roomImages)
            {
                roomImage.IsMain = roomImage.Id == imageId;
            }
            return true;
        }
    }
}
