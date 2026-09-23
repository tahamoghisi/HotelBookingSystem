using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Application.Common.Models
{
    public class SortingRequest
    {
        public string? SortBy { get; set; } = "Id";
        public bool Descending { get; set; }
    }
}
