using HotelBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Domain.Interfaces
{
    public interface IBookingRepository : IGenericRepositoy<Booking>
    {
        Task<IEnumerable<Booking>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<Booking>> GetByRoomIdAsync(int roomId);
        Task<IEnumerable<Booking>> GetActiveBookingsAsync();
        Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut);// بررسی اینکه آیا اتاق در بازه زمانی خاص رزرو شده؟
        Task<IEnumerable<Booking>> GetPastBookingsAsync();// دریافت رزروهای گذشته
        Task<Booking?> GetBookingWithDetailsAsync(int id);
        Task<bool> HasActiveBookingsAsync(int roomId);
        Task<bool> HasCustomerActiveBookingsAsync(int customerId); //بررسی وجود امانت فعال مشتری

    }
}
