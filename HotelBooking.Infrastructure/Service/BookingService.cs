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
using static HotelBooking.Domain.Entities.Room;

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
            if (booking.Status != BookingStatus.Pending &&
                booking.Status != BookingStatus.Confirmed)
            {
                throw new InvalidOperationException(
                    "Only pending or confirmed bookings can be cancelled.");
            }
            if (booking.Status == BookingStatus.Confirmed)
            {
                var room = await _unitOFWork.Rooms.GetByIdAsync(booking.RoomId);

                if (room == null)
                    throw new ArgumentException("Room Not Found!");

                room.Status = RoomStatus.Available;

                _unitOFWork.Rooms.Update(room);
            }
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
            var available = await _unitOFWork.Rooms.IsRoomAvailableAsync(dto.RoomId, dto.CheckInDate, dto.CheckOutDate);
            if (!available)
            {
                throw new Exception("Room is not available for the selected dates.");
            }
            var booking = BookingMapping.ToEntity(dto);
            booking.Status = BookingStatus.Pending;
            //محاسبه ی مبلغ کل
            var nights = (dto.CheckOutDate - dto.CheckInDate).Days;
            booking.TotalPrice = room.PricePerNight * nights;


            await _unitOFWork.Bookings.AddAsync(booking);
            await _unitOFWork.SaveChangesAsync();
            var response = BookingMapping.ToDto(booking);
            return response;
        }

        public async Task<IEnumerable<BookingResponseDTO>> GetAllAsync()
        {
            var bookings = await _unitOFWork.Bookings.GetAllBookingsAsync();
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
            var booking = await _unitOFWork.Bookings.GetBookingByIdAsync(id);
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

            if (booking == null)
                return false;

            if (dto.CheckInDate >= dto.CheckOutDate)
                throw new ArgumentException(
                    "Check-out date must be after check-in date.");

            var customer = await _unitOFWork.Customers.GetByIdAsync(dto.CustomerId);

            if (customer == null)
                throw new ArgumentException("Customer not found.");

            var hotel = await _unitOFWork.Hotels.GetByIdAsync(dto.HotelId);

            if (hotel == null)
                throw new ArgumentException("Hotel not found.");

            var room = await _unitOFWork.Rooms.GetByIdAsync(dto.RoomId);

            if (room == null)
                throw new ArgumentException("Room not found.");

            var available = await _unitOFWork.Bookings.IsRoomAvailableAsync(
                dto.RoomId,
                dto.CheckInDate,
                dto.CheckOutDate,
                booking.Id);

            if (!available)
                throw new InvalidOperationException(
                    "Room is not available for the selected dates.");

            var nights = (dto.CheckOutDate - dto.CheckInDate).Days;
            booking.CheckOutDate = dto.CheckOutDate;
            booking.CheckInDate = dto.CheckInDate;
            booking.HotelId = dto.HotelId;
            booking.RoomId = dto.RoomId;
            booking.CustomerId = dto.CustomerId;
            booking.TotalPrice = room.PricePerNight * nights;
            _unitOFWork.Bookings.Update(booking);
            await _unitOFWork.SaveChangesAsync();
            return true;
        }
        public async Task<bool> ConfirmAsync(int bookingId)
        {
            var booking = await _unitOFWork.Bookings.GetByIdAsync(bookingId);

            if (booking == null)
                return false;

            if (booking.Status != BookingStatus.Pending)
                throw new InvalidOperationException(
                    "Only pending bookings can be confirmed.");

            var room = await _unitOFWork.Rooms.GetByIdAsync(booking.RoomId);

            if (room == null)
                throw new ArgumentException("Room Not Found!");

            var available = await _unitOFWork.Bookings.IsRoomAvailableAsync(
                booking.RoomId,
                booking.CheckInDate,
                booking.CheckOutDate,
                booking.Id);

            if (!available)
                throw new InvalidOperationException(
                    "Room is no longer available for the selected dates.");

            booking.Status = BookingStatus.Confirmed;

            room.Status = RoomStatus.Reserved;

            _unitOFWork.Bookings.Update(booking);
            _unitOFWork.Rooms.Update(room);

            await _unitOFWork.SaveChangesAsync();

            return true;
        }
        public async Task<bool> CheckInAsync(int bookingId)
        {
            var booking = await _unitOFWork.Bookings.GetByIdAsync(bookingId);

            if (booking == null)
                return false;

            if (booking.Status != BookingStatus.Confirmed)
                throw new InvalidOperationException(
                    "Only confirmed bookings can be checked in.");

            var room = await _unitOFWork.Rooms.GetByIdAsync(booking.RoomId);

            if (room == null)
                throw new ArgumentException("Room Not Found!");

            booking.Status = BookingStatus.CheckedIn;
            room.Status = RoomStatus.Occupied;

            _unitOFWork.Bookings.Update(booking);
            _unitOFWork.Rooms.Update(room);

            await _unitOFWork.SaveChangesAsync();

            return true;
        }
        public async Task<bool> CheckOutAsync(int bookingId)
        {
            var booking = await _unitOFWork.Bookings.GetByIdAsync(bookingId);

            if (booking == null)
                return false;

            if (booking.Status != BookingStatus.CheckedIn)
                throw new InvalidOperationException(
                    "Only checked-in bookings can be checked out.");

            var room = await _unitOFWork.Rooms.GetByIdAsync(booking.RoomId);

            if (room == null)
                throw new ArgumentException("Room Not Found!");

            booking.Status = BookingStatus.Completed;
            room.Status = RoomStatus.Available;

            _unitOFWork.Bookings.Update(booking);
            _unitOFWork.Rooms.Update(room);

            await _unitOFWork.SaveChangesAsync();

            return true;
        }
    }
}
