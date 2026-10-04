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
        Task<bool> DeleteAsync(int imageId);
        Task<RoomImage?> GetByIdAsync(int roomId, int imageId);
        Task<bool> SetMainImageAsync(int roomId, int imageId);
        Task<RoomImage?> GetFirstImageAsync(int roomId);
    }
}
