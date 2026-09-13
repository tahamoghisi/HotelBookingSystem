using HotelBooking.Application.DTOs.Customer;
using HotelBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Application.Mapping.CustomerMap
{
    public static class CustomerMapping
    {
        public static CustomerResponseDTO ToDto(Customer customer)
        {
            return new CustomerResponseDTO
            {
                Id = customer.Id,
                NationalCode = customer.NationalCode,
                FullName = customer.FullName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                UserId = customer.UserId,
            };
        }
        public static Customer ToEntity(CreateCustomerDTO CustomerDTO)
        {
            return new Customer
            {
                FullName = CustomerDTO.FullName,
                Email = CustomerDTO.Email,
                PhoneNumber = CustomerDTO.PhoneNumber,
                NationalCode = CustomerDTO.NationalCode
            };
        }
        public static Customer UpdateEntity(UpdateCustomerDTO CustomerDTO)
        {
            return new Customer
            {
                FullName = CustomerDTO.FullName,
                Email = CustomerDTO.Email,
                PhoneNumber = CustomerDTO.PhoneNumber,
                NationalCode = CustomerDTO.NationalCode
            };
        }
    }
}
