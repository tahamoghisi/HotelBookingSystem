using HotelBooking.Application.Common.Models;
using HotelBooking.Application.DTOs.Customer;
using HotelBooking.Application.DTOs.Hotel;
using HotelBooking.Application.DTOs.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Application.ServiceInterface
{
    public interface IHotelService
    {
        Task<IEnumerable<HotelResponseDTO>> GetAllAsync();

        Task<HotelResponseDTO?> GetByIdAsync(int id);

        Task<HotelResponseDTO> CreateAsync(CreateHotelDTO dto);

        Task<bool> UpdateAsync(int id, UpdateHotelDTO dto);

        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<RoomResponseDTO>> GetHotelRoomsAsync(int hotelId);
        Task<PagedResult<HotelResponseDTO>> GetPagedAsync(int page, int pageSize);
        Task<PagedResult<HotelResponseDTO>> SearchPagedAsync(string? name, string? city, int? minStarRating, PaginationRequest pagination, SortingRequest sorting);//صفخه بندی و تعداد کل و مرتب سازی

    }
}
