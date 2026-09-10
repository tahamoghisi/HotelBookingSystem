using HotelBooking.Application.DTOs.Booking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Application.ServiceInterface
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingResponseDTO>> GetAllAsync();

        Task<BookingResponseDTO?> GetByIdAsync(int id);

        Task<IEnumerable<BookingResponseDTO>> GetByCustomerIdAsync(int customerId);

        Task<IEnumerable<BookingResponseDTO>> GetByRoomIdAsync(int roomId);
        Task<BookingResponseDTO> CreateAsync(CreateBookingDTO dto);

        Task<bool> UpdateAsync(int id, UpdateBookingDTO dto);

        Task<bool> CancelAsync(int id);

        Task<bool> IsRoomAvailableAsync(
            int roomId,
            DateTime checkIn,
            DateTime checkOut);
    }
}

