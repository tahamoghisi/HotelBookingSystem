using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelBooking.Domain;

using static HotelBooking.Domain.Entities.Room;
namespace HotelBooking.Application.DTOs.Room
{
    public class RoomResponseDTO
    {
        public int Id { get; set; }
        public int RoomNumber { get; set; } 
        public RoomType RoomType { get; set; } 
        public decimal PricePerNight { get; set; }
        public int Capacity { get; set; }
        public int HotelId { get; set; }
        public string HotelName { get; set; } = string.Empty; // برای نمایش
        public RoomStatus Status { get; set; }

    }
}
