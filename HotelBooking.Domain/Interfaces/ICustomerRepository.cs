using HotelBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Domain.Interfaces
{
    public interface ICustomerRepository : IGenericRepositoy<Customer>
    {
        Task<Customer?> GetByEmailAsync(string email);
        Task<Customer?> GetByUserIdAsync(int userId);
        Task<Customer?> GetByNationalCodeAsync(string nationalCode);
        Task<bool> ExistsByEmailAsync(string email, int customerId);
        Task<bool> ExistsByNationalCodeAsync(string nationalCode, int customerId);
        Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, int customerId);
        Task<Customer?> GetCustomerWithBookingsAsync(int customerId);
        Task<IEnumerable<Customer>> SearchCustomersAsync(string? name, string? email);
        Task<(IEnumerable<Customer> Items, int TotalCount)> SearchPagedAsync(string? fullName, string? email, string? nationalCode, int page, int pageSize, string? sortBy, bool descending);//صفخه بندی و تعداد کل و مرتب سازی

    }
}
