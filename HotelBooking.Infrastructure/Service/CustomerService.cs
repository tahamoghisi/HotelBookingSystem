using HotelBooking.Application.DTOs.Customer;
using HotelBooking.Application.Mapping.CustomerMap;
using HotelBooking.Application.ServiceInterface;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Infrastructure.Service
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOFWork _unitOFWork;
        public CustomerService(IUnitOFWork unitOFWork)
        {
            _unitOFWork = unitOFWork;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var customer = await _unitOFWork.Customers.GetByIdAsync(id);
            if (customer == null) return false;
            var hasBooking = await _unitOFWork.Bookings.HasCustomerActiveBookingsAsync(customer.Id);
            if (hasBooking) throw new InvalidOperationException(
            "Cannot delete a customer with active bookings.");
            _unitOFWork.Customers.Remove(customer);
            await _unitOFWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CustomerResponseDTO>> GetAllAsync()
        {
            var customers = await _unitOFWork.Customers.GetAllAsync();
            return customers.Select(x => CustomerMapping.ToDto(x));
        }

        public async Task<CustomerResponseDTO?> GetByIdAsync(int id)
        {
            var customer = await _unitOFWork.Customers.GetByIdAsync(id);
            if (customer == null) return null;
            return CustomerMapping.ToDto(customer);
        }

        public async Task<bool> UpdateAsync(int id, UpdateCustomerDTO dto)
        {
            var customer = await _unitOFWork.Customers.GetByIdAsync(id);
            if (customer == null) return false;
            if (await _unitOFWork.Customers
                .ExistsByEmailAsync(dto.Email, id))
            {
                throw new InvalidOperationException(
                    "Email already exists.");
            }

            if (await _unitOFWork.Customers
                .ExistsByNationalCodeAsync(dto.NationalCode, id))
            {
                throw new InvalidOperationException(
                    "National code already exists.");
            }

            if (await _unitOFWork.Customers
                .ExistsByPhoneNumberAsync(dto.PhoneNumber, id))
            {
                throw new InvalidOperationException(
                    "Phone number already exists.");
            }
            customer.FullName = dto.FullName;
            customer.Email = dto.Email;
            customer.PhoneNumber = dto.PhoneNumber;
            customer.NationalCode = dto.NationalCode;
            _unitOFWork.Customers.Update(customer);
            await _unitOFWork.SaveChangesAsync();
            return true;
        }
    }
}
