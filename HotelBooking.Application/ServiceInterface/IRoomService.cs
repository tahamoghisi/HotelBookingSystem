using HotelBooking.Application.Common.Models;
using HotelBooking.Application.DTOs.Hotel;
using HotelBooking.Application.DTOs.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HotelBooking.Domain.Entities.Room;

namespace HotelBooking.Application.ServiceInterface
{
    public interface IRoomService
    {
        Task<IEnumerable<RoomResponseDTO>> GetAllAsync();
        Task<RoomResponseDTO?> GetByIdAsync(int id);
        Task<IEnumerable<RoomResponseDTO>> GetByHotelIdAsync(int hotelId);
        Task<RoomResponseDTO> CreateAsync(CreateRoomDTo dto);
        Task<bool> UpdateAsync(int id, UpdateRoomDTO dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> SetMaintenanceAsync(int roomId);
        Task<bool> SetAvailableAsync(int roomId);
        Task<PagedResult<RoomResponseDTO>> GetPagedAsync(int hotelId, int page, int pageSize);
        Task<PagedResult<RoomResponseDTO>> SearchPagedAsync(int? roomNumber, RoomStatus? status, PaginationRequest pagination, SortingRequest sorting);// صرچ و صفحه بندی و مرتب سازی


    }
}
