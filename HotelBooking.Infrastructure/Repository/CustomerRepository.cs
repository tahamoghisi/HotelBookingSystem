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
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        private readonly ApplicationDBContext _dbContext;
        public CustomerRepository(ApplicationDBContext dBContext) : base(dBContext)
        {
            _dbContext = dBContext;
        }
        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _dbContext.Customers
                .AnyAsync(x => x.Email == email);
        }

        public async Task<bool> ExistsByNationalCodeAsync(string nationalCode)
        {
            return await _dbContext.Customers
                .AnyAsync(x => x.NationalCode == nationalCode);
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
                query =query.Where(c => c.FuullName == name);
            }
            if (!string.IsNullOrWhiteSpace(email))
            {
                query = query.Where(c => c.Email == email);
            }
            return await query.ToListAsync();
        }
    }
}
