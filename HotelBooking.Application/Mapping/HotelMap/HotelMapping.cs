using HotelBooking.Application.DTOs.Hotel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HotelBooking.Domain.Entities;
using HotelBooking.Application.DTOs.Image;

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
                PhoneNumber = hotel.PhoneNumber,
                Images = hotel.Images.Select(image => new HotelImageResponseDTO
                {
                    Id = image.Id,
                    ImageUrl = image.ImageUrl,
                    IsMain = image.IsMain
                }).ToList()
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
        public static Hotel UpdateEntity(Hotel hotel,UpdateHotelDTO hotelDto)
        {
            hotel.Name = hotelDto.Name;
            hotel.City = hotelDto.City;
            hotel.Country = hotelDto.Country;
            hotel.Address = hotelDto.Address;
            hotel.Description = hotelDto.Description;
            hotel.Email = hotelDto.Email;
            hotel.StarRating = hotelDto.StarRating;
            hotel.PhoneNumber = hotelDto.PhoneNumber;
            return hotel;
        }
    }
}
