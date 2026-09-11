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
    public class RoomRepository : GenericRepository<Room>, IRoomRepository
    {
        private readonly ApplicationDBContext _dbContext;
        public RoomRepository(ApplicationDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<Room>> GetAviablelRooms()
        {
            return await _dbContext.Rooms
                .Where(r => r.Status == Room.RoomStatus.Available)
                .ToListAsync();
        }

        public async Task<IEnumerable<Room>> GetByHotelIdAsync(int hotelId)
        {
            var rooms = await _dbContext.Rooms.Where(r => r.HotelId == hotelId).ToListAsync();
            return rooms;
        }

        public async Task<Room?> GetByRoomNumberAsync(int roomNumber)
        {
            return await _dbContext.Rooms
                .FirstOrDefaultAsync(r => r.RoomNumber == roomNumber);
        }

        public async Task<IEnumerable<Room>> GetRoomsByHotelId(int hotelId)
        {
            return await _dbContext.Rooms.Where(x => x.HotelId == hotelId).ToListAsync();
        }

        public async Task<Room?> GetRoomWithBookingsAsync(int roomId)
        {
            return await _dbContext.Rooms
            .Include(r => r.Bookings)
            .FirstOrDefaultAsync(r => r.Id == roomId);
        }

        public async Task<bool> IsRoomAvailableAsync(int roomId,DateTime checkIn,DateTime checkOut)
        {
            return !await _dbContext.Bookings
                .AnyAsync(b => b.RoomId == roomId && checkIn < b.CheckOutDate && checkOut > b.CheckInDate);
        }

        public async Task<IEnumerable<Room>> SearchRoomsAsync(int hotelId, int? capacity, decimal? minPrice, decimal? maxPrice)
        {
            var query = _dbContext.Rooms.Where(x => x.HotelId == hotelId);
            if (capacity.HasValue)
            {
                query = query.Where(x => x.Capacity >= capacity.Value);
            }
            if (minPrice.HasValue)
            {
                query = query.Where(x => x.PricePerNight >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(x => x.PricePerNight <= maxPrice.Value);
            }
            return await query.ToListAsync();
        }
    }
}
