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
        //صفخه بندی و تعداد کل و مرتب سازی

        public async Task<(IEnumerable<Customer> Items, int TotalCount)> SearchPagedAsync(string? fullName, string? email, string? nationalCode, int page, int pageSize, string? sortBy, bool descending)
        {
            var query = _dbContext.Customers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(fullName))
                query = query.Where(c => c.FullName.Contains(fullName));

            if (!string.IsNullOrWhiteSpace(email))
                query = query.Where(c => c.Email.Contains(email));

            if (!string.IsNullOrWhiteSpace(nationalCode))
                query = query.Where(c => c.NationalCode.Contains(nationalCode));


            //var items = await query
            //    .OrderBy(c => c.Id)
            //    .Skip((page - 1) * pageSize)
            //    .Take(pageSize)
            //    .ToListAsync();
            return await GetPagedTotalAsync(query, page, pageSize, sortBy, descending);
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
