using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HotelBooking.Domain.Entities.Room;

namespace HotelBooking.Application.DTOs.Room
{
    public class CreateRoomDTo
    {
        public int RoomNumber { get; set; } 
        public int HotelId { get; set; }
        public RoomType Type { get; set; } // Single, Double, Suite
        public decimal PricePerNight { get; set; }
        public int Capacity { get; set; } //ظرفیت
        public bool IsAvialble { get; set; }
    }
}
