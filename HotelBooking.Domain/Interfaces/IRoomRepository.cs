using HotelBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HotelBooking.Domain.Entities.Room;

namespace HotelBooking.Domain.Interfaces
{
    public interface IRoomRepository : IGenericRepositoy<Room>
    {
        Task<IEnumerable<Room>> GetAviablelRooms();
        Task<Room?> GetByRoomNumberAsync(int roomNumber);
        Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut);
        Task <IEnumerable<Room>> GetRoomsByHotelId(int hotelId);
        Task<IEnumerable<Room>> SearchRoomsAsync(int hotelId,int? capacity,decimal? minPrice,decimal? maxPrice);
        Task<Room?> GetRoomWithBookingsAsync(int roomId);
        Task<IEnumerable<Room>> GetByHotelIdAsync(int hotelId);
        Task<bool> HasActiveRoomsAsync(int hotelId);
        Task<IEnumerable<Room>> GetPagedByHotelAsync(int hotelId,int pageNumber,int pageSize);
        Task<int> CountByHotelAsync(int hotelId);
        Task<(IEnumerable<Room> Items, int TotalCount)> SearchPagedAsync(int? hotelId,int? roomNumber, RoomStatus? status, int? MinPrice, int? maxPrice, int page, int pageSize, string? sortBy, bool descending);//صفخه بندی و تعداد کل و مرتب سازی

    }
}
