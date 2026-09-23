using HotelBooking.Application.Common.Models;
using HotelBooking.Application.DTOs.Customer;
using HotelBooking.Application.DTOs.Hotel;
using HotelBooking.Application.DTOs.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Application.ServiceInterface
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerResponseDTO>> GetAllAsync();
        Task<CustomerResponseDTO?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(int id, UpdateCustomerDTO dto);
        Task<bool> DeleteAsync(int id);
        Task<PagedResult<CustomerResponseDTO>> SearchPagedAsync(string? fullName, string? email, string? nationalCode, PaginationRequest pagination, SortingRequest sorting);//صفخه بندی و تعداد کل و مرتب سازی
    }
}
