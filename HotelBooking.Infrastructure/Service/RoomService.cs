using HotelBooking.Application.DTOs.Room;
using HotelBooking.Application.Mapping.RoomMap;
using HotelBooking.Application.ServiceInterface;
using HotelBooking.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HotelBooking.Domain.Entities.Room;

namespace HotelBooking.Infrastructure.Service
{
    public class RoomService : IRoomService
    {
        private readonly IUnitOFWork _unitOFWork;
        public RoomService(IUnitOFWork unitOFWork)
        {
            _unitOFWork = unitOFWork;
        }
        public async Task<RoomResponseDTO> CreateAsync(CreateRoomDTo dto)
        {
            var hotel = await _unitOFWork.Hotels.GetByIdAsync(dto.HotelId);

            if (hotel == null)
                throw new ArgumentException("Hotel Not Found!");

            // ساخت Room
            var room = RoomMapping.ToEntity(dto);
            room.Status = RoomStatus.Available;

            await _unitOFWork.Rooms.AddAsync(room);
            await _unitOFWork.SaveChangesAsync();

            return RoomMapping.ToDto(room);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var room = await _unitOFWork.Rooms.GetByIdAsync(id);
            if (room == null) return false;
            if (room.Status == RoomStatus.Occupied)
                throw new InvalidOperationException(
                    "Cannot delete an occupied room.");

            if (room.Status == RoomStatus.Reserved)
                throw new InvalidOperationException(
                    "Cannot delete a reserved room.");

            var hasBookings = await _unitOFWork.Bookings
                .HasActiveBookingsAsync(id);

            if (hasBookings)
                throw new InvalidOperationException(
                    "Cannot delete a room with active bookings.");
            _unitOFWork.Rooms.Remove(room);
            await _unitOFWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<RoomResponseDTO>> GetAllAsync()
        {
            var rooms = await _unitOFWork.Rooms.GetAllAsync();
            return rooms.Select(x => RoomMapping.ToDto(x)).ToList();
        }

        public async Task<IEnumerable<RoomResponseDTO>> GetByHotelIdAsync(int hotelId)
        {
            var rooms = await _unitOFWork.Rooms.GetByHotelIdAsync(hotelId);
            return rooms.Select(x => RoomMapping.ToDto(x)).ToList();
        }

        public async Task<RoomResponseDTO?> GetByIdAsync(int id)
        {
            var room = await _unitOFWork.Rooms.GetByIdAsync(id);

            if (room == null)
                return null;

            return RoomMapping.ToDto(room);
        }

        public async Task<bool> UpdateAsync(int id, UpdateRoomDTO dto)
        {
            var room = await _unitOFWork.Rooms.GetByIdAsync(id);
            if (room == null) return false;
            var hotel = await _unitOFWork.Hotels.GetByIdAsync(dto.HotelId);
            if (hotel == null) throw new ArgumentException("Hotel Not Found!");
            room.HotelId = dto.HotelId;
            room.RoomNumber = dto.RoomNumber;
            room.Type = dto.Type;
            room.PricePerNight = dto.PricePerNight;
            room.Capacity = dto.Capacity;

            _unitOFWork.Rooms.Update(room);

            await _unitOFWork.SaveChangesAsync();

            return true;
        }
    }
}
