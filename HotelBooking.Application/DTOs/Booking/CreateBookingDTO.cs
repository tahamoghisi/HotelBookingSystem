using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HotelBooking.Domain.Entities.Booking;

namespace HotelBooking.Application.DTOs.Booking
{
    public class CreateBookingDTO
    {
        public int CustomerId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalPrice { get; set; }
        public int RoomId { get; set; }
        public int HotelId { get; set; }
        //public BookingStatus Status { get; set; }

    }
}
