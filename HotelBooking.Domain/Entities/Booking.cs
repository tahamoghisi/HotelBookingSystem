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
        [Required]
        public int RoomId { get; set; }
        [Required]
        public int HotelId { get; set; }
        [Required]
        public int CustomerId { get; set; }
        [Required]
        public DateTime CheckInDate { get; set; }
        [Required]
        public DateTime CheckOutDate { get; set; }
        [Required]
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
            Completed = 3
        }
    }
}
