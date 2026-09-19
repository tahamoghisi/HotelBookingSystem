using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Domain.Entities
{
    public class Booking : BaseEntity
    {
        public int RoomId { get; set; }
        public int HotelId { get; set; }
        public int CustomerId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; }

        public Hotel Hotel { get; set; }
        public Customer Customer { get; set; }
        public Room Room { get; set; }


        public enum BookingStatus
        {
            Pending = 0,
            Confirmed = 1,
            Cancelled = 2,
            Completed = 3,
            CheckedIn = 4
        }
    }
}
