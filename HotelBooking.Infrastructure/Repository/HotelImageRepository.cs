using HotelBooking.Domain.Entities.Images;
using HotelBooking.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Infrastructure.Repository
{
    public class HotelImageRepository : IHotelImageRepository
    {
        private readonly ApplicationDBContext _dbContext;
        public HotelImageRepository(ApplicationDBContext dBContext)
        {
            _dbContext = dBContext;
        }
        public async Task AddRangeAsync(IEnumerable<HotelImage> images)
        {
            await _dbContext.HotelImages.AddRangeAsync(images);
        }

        public async Task<bool> DeleteAsync(int imageId)
        {
            var image = await _dbContext.HotelImages.FirstOrDefaultAsync(i => i.Id == imageId);
            if (image == null) return false;
            _dbContext.HotelImages.Remove(image);
            return true;
        }

        public async Task<HotelImage?> GetByIdAsync(int hotelId, int imageId)
        {
            return await _dbContext.HotelImages.FirstOrDefaultAsync(i => i.Id == imageId && i.HotelId == hotelId);
        }

        public async Task<HotelImage?> GetFirstImageAsync(int hotelId)
        {
            return await _dbContext.HotelImages.Where(i => i.HotelId == hotelId).OrderBy(i => i.Id).FirstOrDefaultAsync();
        }

        public async Task<bool> HasMainImageAsync(int hotelId)
        {
            return await _dbContext.HotelImages
                .AnyAsync(x => x.HotelId == hotelId && x.IsMain);
        }

        public async Task<bool> SetMainImageAsync(int hotelId, int imageId)
        {
            var image = await _dbContext.HotelImages.FirstOrDefaultAsync(i => i.HotelId==hotelId && i.Id == imageId);
            if (image == null) return false;

            var hotelImages = await _dbContext.HotelImages
                .Where(i => i.HotelId == hotelId)
                .ToListAsync();

            foreach (var hotelImage in hotelImages)
            {
                hotelImage.IsMain = hotelImage.Id == imageId;
            }
            return true;
        }
    }
}
