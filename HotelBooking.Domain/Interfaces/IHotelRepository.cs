using HotelBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Domain.Interfaces
{
    public interface IHotelRepository : IGenericRepositoy<Hotel>
    {
        Task<Hotel?> GetByNameAsync(string name);
        Task<IEnumerable<Hotel>> GetByCityAsync(string city);
        Task<IEnumerable<Hotel>> GetByStarRatingAsync(int starRating);
        Task<IEnumerable<Hotel>> GetActiveHotelsAsync();
        Task<Hotel?> GetHotelWithRoomsAsync(int hotelId);
        Task<IEnumerable<Hotel>> SearchHotelsAsync(string? city, int? minStarRating, int? maxStarRating);
        Task<IEnumerable<Hotel>> SearchHotelsAsync(string? name,string? city,int? minStars);
        Task<bool> ExistsByNameAsync(string name);
        Task<IEnumerable<Hotel>> GetHotelsWithAvailableRoomsAsync(DateTime checkIn,DateTime checkOut);
    }
}
}
