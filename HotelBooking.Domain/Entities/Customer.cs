using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Domain.Entities
{
    public class Customer : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string NationalCode { get; set; } = string.Empty;
        public int UserId { get; set; }
        public User User { get; set; }
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
