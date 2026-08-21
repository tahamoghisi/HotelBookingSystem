using HotelBooking.Application.DTOs.Room;
using HotelBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Application.Mapping.RoomMap
{
    public static class RoomMapping
    {
        public static RoomResponseDTO ToDto(Room room)
        {
            return new RoomResponseDTO
            {
                Id = room.Id,
                RoomNumber = room.RoomNumber,
                PricePerNight = room.PricePerNight,
                Capacity = room.Capacity,
                IsAvailable = room.IsAvailable,
                HotelId = room.HotelId,
                RoomType = room.Type,
                HotelName = room.Hotel?.Name ?? "نامشخص"
            };
        }
        public static Room ToEntity(CreateRoomDTo RoomDto)
        {
            return new Room
            {
                RoomNumber = RoomDto.RoomNumber,
                PricePerNight = RoomDto.PricePerNight,
                Capacity = RoomDto.Capacity,
                HotelId = RoomDto.HotelId,
                Type = RoomDto.Type,
                IsAvailable = RoomDto.IsAvialble
            };
        }
        public static Room UpdateEntity(UpdateRoomDTO RoomDto)
        {
            return new Room
            {
                RoomNumber = RoomDto.RoomNumber,
                PricePerNight = RoomDto.PricePerNight,
                Capacity = RoomDto.Capacity,
                HotelId = RoomDto.HotelId,
                Type = RoomDto.Type,
                IsAvailable = RoomDto.IsAvailable
            };
        }
    }
}
