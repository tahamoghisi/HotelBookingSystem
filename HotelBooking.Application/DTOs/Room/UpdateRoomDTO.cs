using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HotelBooking.Domain.Entities.Room;

namespace HotelBooking.Application.DTOs.Room
{
    public class UpdateRoomDTO
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public int RoomNumber { get; set; }
        public RoomType Type { get; set; } // Single, Double, Suite
        public decimal PricePerNight { get; set; }
        public int Capacity { get; set; } //ظرفیت
    }
}
