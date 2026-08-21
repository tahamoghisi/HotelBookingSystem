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
    public class HotelRepository : GenericRepository<Hotel>, IHotelRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public HotelRepository(ApplicationDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<Hotel>> GetActiveHotelsAsync()
        {
            return await _dbContext.Hotels
                .Where(h => h.IsActive == true)
                .ToListAsync();
        }

        public async Task<IEnumerable<Hotel>> GetByCityAsync(string city)
        {
            return await _dbContext.Hotels
                .Where(h => h.City == city)
                .ToListAsync();
        }

        public async Task<Hotel?> GetByNameAsync(string name)
        {
            return await _dbContext.Hotels
                .FirstOrDefaultAsync(h => h.Name == name);
        }

        public async Task<IEnumerable<Hotel>> GetByStarRatingAsync(int starRating)
        {
            return await _dbContext.Hotels
                .Where(h => h.StarRating == starRating).ToListAsync();
        }

        public async Task<Hotel?> GetHotelWithRoomsAsync(int hotelId)
        {
            return await _dbContext.Hotels
                .Include(r => r.Rooms)
                .FirstOrDefaultAsync(h => h.Id == hotelId);
        }

        public async Task<IEnumerable<Hotel>> SearchHotelsAsync(string? city, int? minStarRating, int? maxStarRating)
        {
            var query = _dbContext.Hotels.Where(h => h.IsActive == true);

            if (!string.IsNullOrWhiteSpace(city))
            {
                query =  query.Where(h => h.City.Contains(city));
            }

            if (minStarRating.HasValue)
            {
                query = query.Where(h => h.StarRating >= minStarRating.Value);
            }

            if (maxStarRating.HasValue)
            {
                query = query.Where(h => h.StarRating <= maxStarRating.Value);
            }

            return await query
                .OrderByDescending(h => h.StarRating)
                .ThenBy(h => h.Name)
                .ToListAsync();
        }
    }
}
