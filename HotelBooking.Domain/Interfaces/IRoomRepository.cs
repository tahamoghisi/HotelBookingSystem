using HotelBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Domain.Interfaces
{
    public interface IRoomRepository : IGenericRepositoy<Room>
    {
        Task<IEnumerable<Room>> GetAviablelRooms();
        Task<Room?> GetByRoomNumberAsync(int roomNumber);
        Task<bool> IsRoomAvailableAsync(int roomId);
        Task <IEnumerable<Room>> GetRoomsByHotelId(int hotelId);
        Task<IEnumerable<Room>> SearchRoomsAsync(int hotelId,int? capacity,decimal? minPrice,decimal? maxPrice);
        Task<Room?> GetRoomWithBookingsAsync(int roomId);
    }
}
