using HotelBooking.Application.DTOs.Hotel;
using HotelBooking.Application.Mapping.HotelMap;
using HotelBooking.Application.Mapping.RoomMap;
using HotelBooking.Application.ServiceInterface;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Infrastructure.Service
{
    public class HotelService : IHotelService
    {
        private readonly IUnitOFWork _unitOFWork;
        public HotelService(IUnitOFWork unitOFWork)
        {
            _unitOFWork = unitOFWork;
        }
        public async Task<HotelResponseDTO> CreateAsync(CreateHotelDTO dto)
        {
            var hotel = HotelMapping.ToEntity(dto);

            await _unitOFWork.Hotels.AddAsync(hotel);
            await _unitOFWork.SaveChangesAsync();

            return HotelMapping.ToDto(hotel);

        }

        public async Task<bool> DeleteAsync(int id)
        {
            var hotel = await _unitOFWork.Hotels.GetByIdAsync(id);
            if (hotel == null) return false;
            var hasActiveRooms = await _unitOFWork.Rooms.HasActiveRoomsAsync(hotel.Id);
            if (hasActiveRooms)
            {
                throw new InvalidOperationException("Cannot delete a hotel with reserved or occupied rooms.");
            }
            _unitOFWork.Hotels.Remove(hotel);
            await _unitOFWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<HotelResponseDTO>> GetAllAsync()
        {
            var hotels = await _unitOFWork.Hotels.GetAllAsync();
            return hotels.Select(x => HotelMapping.ToDto(x)).ToList();
         }

        public async Task<HotelResponseDTO?> GetByIdAsync(int id)
        {
            var hotel = await _unitOFWork.Hotels.GetByIdAsync(id);
            if (hotel == null)
            {
                return null;
            }
            return HotelMapping.ToDto(hotel);
        }

        public async Task<bool> UpdateAsync(int id, UpdateHotelDTO dto)
        {
            var hotel = await _unitOFWork.Hotels.GetByIdAsync(id);
            if(hotel == null)
            {
                throw new InvalidOperationException("Hotel not found!");
            }
            hotel.Name = dto.Name;
            hotel.City = dto.City;
            hotel.Country = dto.Country;
            hotel.Address = dto.Address;
            hotel.Description = dto.Description;
            hotel.Email = dto.Email;
            hotel.StarRating = dto.StarRating;
            hotel.PhoneNumber = dto.PhoneNumber;
            _unitOFWork.Hotels.Update(hotel);
            await _unitOFWork.SaveChangesAsync();
            return true;
        }
    }
}
