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
            await _dbContext.SaveChangesAsync();
        }
        public async Task<bool> HasMainImageAsync(int hotelId)
        {
            return await _dbContext.HotelImages
                .AnyAsync(x => x.HotelId == hotelId && x.IsMain);
        }
    }
}
