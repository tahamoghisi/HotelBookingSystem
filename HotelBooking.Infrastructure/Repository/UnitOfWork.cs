using HotelBooking.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOFWork
    {
        private readonly ApplicationDBContext _dbContext;
        public UnitOfWork(
        ApplicationDBContext context,
        IHotelRepository hotels,
        IRoomRepository rooms,
        ICustomerRepository customers,
        IBookingRepository bookings
            )
        {
            _dbContext = context;
            Hotels = hotels;
            Rooms = rooms;
            Customers = customers;
            Bookings = bookings;
        }
        public IHotelRepository Hotels { get; }

        public IRoomRepository Rooms { get; }

        public ICustomerRepository Customers { get; }

        public IBookingRepository Bookings { get; }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
