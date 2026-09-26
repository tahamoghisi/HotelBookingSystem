using HotelBooking.Application.Common.Models;
using HotelBooking.Application.DTOs.Booking;
using HotelBooking.Application.DTOs.Customer;
using HotelBooking.Application.DTOs.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HotelBooking.Domain.Entities.Booking;

namespace HotelBooking.Application.ServiceInterface
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingResponseDTO>> GetAllAsync();

        Task<BookingResponseDTO?> GetByIdAsync(int id);

        Task<IEnumerable<BookingResponseDTO>?> GetByUserIdAsync(int userId);
        Task<BookingResponseDTO?> GetByIdForUserAsync(int bookingId, int userId);

        Task<IEnumerable<BookingResponseDTO>> GetByRoomIdAsync(int roomId);
        Task<BookingResponseDTO> CreateAsync(CreateBookingDTO dto, int userId);

        Task<bool> UpdateAsync(int id, UpdateBookingDTO dto);

        Task<bool> CancelAsync(int id);

        Task<bool> IsRoomAvailableAsync(
            int roomId,
            DateTime checkIn,
            DateTime checkOut);
        Task<bool> ConfirmAsync(int bookingId);
        Task<bool> CheckInAsync(int bookingId);
        Task<bool> CheckOutAsync(int bookingId);
        Task<PagedResult<BookingResponseDTO>> GetPagedAsync(int page, int pageSize, int? hotelId, int? customerId, int? roomId, BookingStatus? status);
        Task<PagedResult<BookingResponseDTO>> SearchPagedAsync(int? customerId, int? roomId, BookingStatus? status, DateTime? checkInFrom, DateTime? checkInTo, PaginationRequest pagination, SortingRequest sorting);//صفخه بندی و تعداد کل و مرتب سازی


    }
}

