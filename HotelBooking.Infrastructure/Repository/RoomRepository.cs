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
                .Where(r => r.IsAvailable == true)
                .ToListAsync();
        }

        public async Task<Room?> GetByRoomNumberAsync(int roomNumber)
        {
            return await _dbContext.Rooms
                .FirstOrDefaultAsync(r => r.RoomNumber == roomNumber);
        }

        public async Task<List<Room>> GetRoomsByHotelId(int hotelId)
        {
            return await _dbContext.Rooms.Where(x => x.HotelId == hotelId).ToListAsync();
        }

        public async Task<bool> IsRoomAvailableAsync(int roomId)
        {
            var room = await GetByIdAsync(roomId);
            if (room == null) return false;
            if (room.IsAvailable == true) return true;
            return false;
        }
    }
}
