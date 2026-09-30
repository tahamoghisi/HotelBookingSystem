using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Domain.Entities.Images
{
    public class RoomImage : BaseEntity
    {
        public string ImageUrl { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public bool IsMain { get; set; }
        public Room Room { get; set; } = null!;
    }
}
