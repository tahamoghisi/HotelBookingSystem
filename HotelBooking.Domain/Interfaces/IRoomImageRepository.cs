using HotelBooking.Domain.Entities.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Domain.Interfaces
{
    public interface IRoomImageRepository
    {
        Task AddRangeAsync(IEnumerable<RoomImage> images);
        Task<bool> HasMainImageAsync(int roomId);
    }
}
