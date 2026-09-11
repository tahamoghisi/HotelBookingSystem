using HotelBooking.Application.DTOs.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }
}
