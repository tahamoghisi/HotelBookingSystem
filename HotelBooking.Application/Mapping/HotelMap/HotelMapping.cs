using HotelBooking.Application.DTOs.Hotel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Mapping.HotelMap
{
    public static class HotelMapping
    {
        public static HotelResponseDTO ToDto(Hotel hotel)
        {
            return new HotelResponseDTO
            {
                Id = hotel.Id,
                Name = hotel.Name,
                City = hotel.City,
                Country = hotel.Country,
                Address = hotel.Address,
                Description = hotel.Description,
                Email = hotel.Email,
                StarRating = hotel.StarRating,
                PhoneNumber = hotel.PhoneNumber
            };
        }
        public static Hotel ToEntity(CreateHotelDTO hotelDto)
        {
            return new Hotel
            {
                Name = hotelDto.Name,
                City = hotelDto.City,
                Country = hotelDto.Country,
                Address = hotelDto.Address,
                Description = hotelDto.Description,
                Email = hotelDto.Email,
                StarRating = hotelDto.StarRating,
                PhoneNumber = hotelDto.PhoneNumber
            };
        }
        public static Hotel UpdateEntity(UpdateHotelDTO hotelDto)
        {
            return new Hotel
            {
                Name = hotelDto.Name,
                City = hotelDto.City,
                Country = hotelDto.Country,
                Address = hotelDto.Address,
                Description = hotelDto.Description,
                Email = hotelDto.Email,
                StarRating = hotelDto.StarRating,
                PhoneNumber = hotelDto.PhoneNumber
            };
        }
    }
}
