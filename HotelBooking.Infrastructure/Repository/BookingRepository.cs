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
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        private readonly ApplicationDBContext _dbContext;
        public BookingRepository(ApplicationDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Booking?> GetBookingWithDetailsAsync(int id)
        {
            return await _dbContext.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Room)
                .Include(b => b.Hotel)
                .FirstOrDefaultAsync(b => b.Id == id);
        }
        public async Task<IEnumerable<Booking>> GetActiveBookingsAsync()
        {
            var today = DateTime.UtcNow.Date;
            return await _dbContext.Bookings
                .Include(x => x.Customer)
                .Include(x => x.Room)
                .ThenInclude(x => x.Hotel)
                .Where(
                    x =>
                    x.CheckInDate <= today &&  // CheckIn شده یا امروز CheckIn میشه
                    x.CheckOutDate >= today)   // هنوز CheckOut نشده
                .OrderBy(x => x.CheckInDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetByCustomerIdAsync(int customerId)
        {
            return await _dbContext.Bookings
                .Include(x => x.Customer)
                .Include(x => x.Room)
                .ThenInclude(x => x.Hotel)
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.CheckInDate).ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetByRoomIdAsync(int roomId)
        {
            return await _dbContext.Bookings
                .Include(x => x.Customer)
                .Include(x => x.Room)
                .Where(x => x.RoomId == roomId)
                .OrderByDescending(x => x.CheckInDate).ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetPastBookingsAsync()
        {
            var today = DateTime.UtcNow.Date;
            return await _dbContext.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Room)
                .Where(b => b.CheckOutDate < today)
                .OrderByDescending(b => b.CheckOutDate)
                .ToListAsync();
        }
        //true یعنی رزرو متداخل وجود دارد
        //false یعنی رزرو متداخلی وجود ندارد

        public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut)
        {
            return await _dbContext.Bookings
                .AnyAsync(x => x.RoomId == roomId &&
               checkIn < x.CheckOutDate && checkOut > x.CheckInDate);
        }
    }
}
