using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HotelBooking.Domain.Entities.Booking;

namespace HotelBooking.Infrastructure.Repository
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        private readonly ApplicationDBContext _dbContext;
        public CustomerRepository(ApplicationDBContext dBContext) : base(dBContext)
        {
            _dbContext = dBContext;
        }
        public async Task<bool> ExistsByEmailAsync(string email, int customerId)
        {
            return await _dbContext.Customers
                .AnyAsync(x => x.Email == email && x.Id != customerId);
        }

        public async Task<bool> ExistsByNationalCodeAsync(string nationalCode, int customerId)
        {
            return await _dbContext.Customers
                .AnyAsync(x => x.NationalCode == nationalCode && x.Id != customerId);
        }

        public async Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, int customerId)
        {
            return await _dbContext.Customers
                .AnyAsync(x => x.PhoneNumber == phoneNumber && x.Id != customerId);
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            return await _dbContext.Customers
                .FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<Customer?> GetByNationalCodeAsync(string nationalCode)
        {
            return await _dbContext.Customers
                            .FirstOrDefaultAsync(x => x.NationalCode == nationalCode);
        }

        public async Task<Customer?> GetCustomerWithBookingsAsync(int customerId)
        {
            return await _dbContext.Customers.Include(c => c.Bookings)
                .FirstOrDefaultAsync(x => x.Id == customerId);
        }

        public async Task<IEnumerable<Customer>> SearchCustomersAsync(string? name, string? email)
        {
            var query = _dbContext.Customers.AsQueryable();
            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(c => c.FullName == name);
            }
            if (!string.IsNullOrWhiteSpace(email))
            {
                query = query.Where(c => c.Email == email);
            }
            return await query.ToListAsync();
        }
    }
}
