using HotelBooking.Application.DTOs.Booking;
using HotelBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HotelBooking.Domain.Interfaces;

namespace HotelBooking.Application.Mapping.BookingMap
{
    public static class BookingMapping
    {
        public static BookingResponseDTO ToDto(Booking booking)
        {
            return new BookingResponseDTO
            {
                Id = booking.Id,
                CustomerId = booking.CustomerId,
                CustomerName = booking.Customer != null
                ? booking.Customer.FullName : string.Empty,
                CustomerEmail = booking.Customer?.Email ?? string.Empty,
                CustomerPhone = booking.Customer?.PhoneNumber ?? string.Empty,
                RoomId = booking.RoomId,
                RoomNumber = booking.Room.RoomNumber,
                HotelId = booking.HotelId,
                HotelName = booking.Hotel?.Name ?? string.Empty,
                CheckInDate = booking.CheckInDate,
                CheckOutDate = booking.CheckOutDate,
                TotalPrice = booking.TotalPrice,
                Status = booking.Status,
            };
        }
        public static Booking ToEntity(CreateBookingDTO dto)
        {
            return new Booking
            {
                CustomerId = dto.CustomerId,
                RoomId = dto.RoomId,
                HotelId = dto.HotelId,
                CheckInDate = dto.CheckInDate,
                CheckOutDate = dto.CheckOutDate,
                TotalPrice = dto.TotalPrice,
                //Status = dto.Status
            };
        }
        public static Booking UpdateEntity(UpdateBookingDTO dto)
        {
            return new Booking
            {
                CustomerId = dto.CustomerId,
                RoomId = dto.RoomId,
                CheckInDate = dto.CheckInDate,
                CheckOutDate = dto.CheckOutDate,
                TotalPrice = dto.TotalPrice,
                //Status = dto.Status,
                HotelId = dto.HotelId
            };
        }
    }
}
