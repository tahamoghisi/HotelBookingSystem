using HotelBooking.Application.DTOs.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Application.ServiceInterface
{
    public interface ICustomerService
    {
        public interface ICustomerService
        {
            Task<IEnumerable<CustomerResponseDTO>> GetAllAsync();
            Task<CustomerResponseDTO?> GetByIdAsync(int id);
            Task<CustomerResponseDTO> CreateAsync(CreateCustomerDTO dto);
            Task<bool> UpdateAsync(int id, UpdateCustomerDTO dto);
            Task<bool> DeleteAsync(int id);
        }
    }
}
