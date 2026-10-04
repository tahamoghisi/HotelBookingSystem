using HotelBooking.Domain.Entities.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Domain.Interfaces
{
    public interface IHotelImageRepository
    {
        Task AddRangeAsync(IEnumerable<HotelImage> images);
        Task<bool> HasMainImageAsync(int hotelId);
        Task<bool> DeleteAsync(int imageId);
        Task<HotelImage?> GetByIdAsync(int hotelId,int imageId);
        Task<bool> SetMainImageAsync(int hotelId, int imageId);
        Task<HotelImage?> GetFirstImageAsync(int hotelId);
    }
}
