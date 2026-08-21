using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Application.DTOs.Hotel
{
    public class CreateHotelDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public int StarRating { get; set; }
        public string? Description { get; set; }
    }
}
