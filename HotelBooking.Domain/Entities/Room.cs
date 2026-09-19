using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Domain.Entities
{
    public class Room : BaseEntity
    {
        public int RoomNumber { get; set; }
        public int HotelId { get; set; }
        public RoomType Type { get; set; }  // Single, Double, Suite
        public decimal PricePerNight { get; set; }
        public int Capacity { get; set; } //ظرفیت
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public Hotel Hotel { get; set; }
        public RoomStatus Status { get; set; }


        public enum RoomType
        {
            Single = 1,
            Double = 2,
            Suite = 3,
            Deluxe = 4,
            Presidential = 5
        }
        public enum RoomStatus
        {
            Available,
            Reserved,
            Occupied,
            Maintenance
        }
    }
}
