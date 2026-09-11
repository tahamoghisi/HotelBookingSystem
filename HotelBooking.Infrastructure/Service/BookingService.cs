using HotelBooking.Application.DTOs.Booking;
using HotelBooking.Application.Mapping.BookingMap;
using HotelBooking.Application.ServiceInterface;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Interfaces;
using HotelBooking.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HotelBooking.Domain.Entities.Booking;

namespace HotelBooking.Infrastructure.Service
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOFWork _unitOFWork;
        public BookingService(IUnitOFWork unitOFWork)
        {
            _unitOFWork = unitOFWork;
        }
        public async Task<bool> CancelAsync(int id)
        {
            var booking = await _unitOFWork.Bookings.GetByIdAsync(id);

            if (booking == null)
                return false;
            booking.Status = BookingStatus.Cancelled;

            _unitOFWork.Bookings.Update(booking);

            await _unitOFWork.SaveChangesAsync();

            return true;
        }

        public async Task<BookingResponseDTO> CreateAsync(CreateBookingDTO dto)
        {
            var room = await _unitOFWork.Rooms.GetByIdAsync(dto.RoomId);
            if (room == null)
            {
                throw new ArgumentException("Room Not Found!");
            }
            if (dto.CheckInDate >= dto.CheckOutDate)
            {
                throw new ArgumentException(
                    "Check-out date must be after check-in date.");
            }
            var customer = await _unitOFWork.Customers.GetByIdAsync(dto.CustomerId);
            if (customer == null)
            {
                throw new ArgumentException("Customer Not Found!");
            }
            var hotel = await _unitOFWork.Hotels.GetByIdAsync(dto.HotelId);
            if (hotel == null)
            {
                throw new ArgumentException("Hotel Not Found!");
            }
            var available = await _unitOFWork.Rooms.IsRoomAvailableAsync(dto.RoomId,dto.CheckInDate,dto.CheckOutDate);
            if (!available)
            {
                throw new Exception("Room is not available for the selected dates.");
            }
            var booking = BookingMapping.ToEntity(dto);
            booking.Status = BookingStatus.Pending;
            await _unitOFWork.Bookings.AddAsync(booking);
            await _unitOFWork.SaveChangesAsync();
            var response = BookingMapping.ToDto(booking);
            return response;
        }

        public async Task<IEnumerable<BookingResponseDTO>> GetAllAsync()
        {
            var bookings = await _unitOFWork.Bookings.GetAllAsync();
            //return bookings.Select(BookingMapping.ToDto);      این متد هم درست است.

            var result = bookings
            .Select(x => BookingMapping.ToDto(x))
            .ToList();

            return result;
        }

        public async Task<IEnumerable<BookingResponseDTO>> GetByCustomerIdAsync(int customerId)
        {
            var booking = await _unitOFWork.Bookings.GetByCustomerIdAsync(customerId);
            if (!booking.Any())
                return Enumerable.Empty<BookingResponseDTO>();
            var result = booking
            .Select(x => BookingMapping.ToDto(x))
            .ToList();
            return result;
        }

        public async Task<BookingResponseDTO?> GetByIdAsync(int id)
        {
            var booking = await _unitOFWork.Bookings.GetByIdAsync(id);
            if (booking == null)
            {
                return null;
            }
            var result = BookingMapping.ToDto(booking);
            return result;
        }

        public async Task<IEnumerable<BookingResponseDTO>> GetByRoomIdAsync(int roomId)
        {
            var booking = await _unitOFWork.Bookings.GetByRoomIdAsync(roomId);
            if (!booking.Any())
                return Enumerable.Empty<BookingResponseDTO>();
            var result = booking
            .Select(x => BookingMapping.ToDto(x))
            .ToList();
            return result;
        }

        public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut)
        {
            return await _unitOFWork.Bookings.IsRoomAvailableAsync(roomId, checkIn, checkOut);
        }

        public async Task<bool> UpdateAsync(int id, UpdateBookingDTO dto)
        {
            var booking = await _unitOFWork.Bookings.GetByIdAsync(id);
            if(booking == null) return false;
            booking.CheckOutDate = dto.CheckOutDate;
            booking.CheckInDate = dto.CheckInDate;
            booking.HotelId = dto.HotelId;
            booking.RoomId = dto.RoomId;
            booking.CustomerId = dto.CustomerId;
            booking.TotalPrice = dto.TotalPrice;
             _unitOFWork.Bookings.Update(booking);
            await _unitOFWork.SaveChangesAsync();
            return true;
        }
    }
}
