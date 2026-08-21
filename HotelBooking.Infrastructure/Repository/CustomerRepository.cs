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
    }
}
