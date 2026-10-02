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
    }
}
