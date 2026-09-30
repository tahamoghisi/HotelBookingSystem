using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Domain.Entities.Images
{
    public class HotelImage : BaseEntity
    {
        public string ImageUrl {  get; set; } = string.Empty;
        public int HotelId { get; set; }
        public bool IsMain { get; set; }
        public Hotel Hotel { get; set; }
    }
}
