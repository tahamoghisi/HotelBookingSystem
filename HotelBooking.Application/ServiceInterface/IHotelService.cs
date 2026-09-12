using HotelBooking.Application.DTOs.Hotel;
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
    }
}
