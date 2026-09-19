using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Domain.Entities
{
    public class Hotel : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public int StarRating { get; set; }
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;


        // Navigation Properties
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
        public ICollection<Booking> Booking { get; set; } = new List<Booking>();
    }
}
