using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Domain.Entities
{
    public class Room : BaseEntity
    {
        [Required(ErrorMessage = "شماره اتاق الزامی است")]
        [MaxLength(10, ErrorMessage = "شماره اتاق نمی‌تواند بیشتر از 10 کاراکتر باشد")]
        public int RoomNumber { get; set; }
        [Required(ErrorMessage = "شماره هتل الزامی است")]
        [MaxLength(10, ErrorMessage = "شماره هتل نمی‌تواند بیشتر از 10 کاراکتر باشد")]
        public int HotelId { get; set; }
        [Required(ErrorMessage = "نوع اتاق الزامی است")]
        [MaxLength(50, ErrorMessage = "نوع اتاق نمی‌تواند بیشتر از 50 کاراکتر باشد")]
        public RoomType Type { get; set; }  // Single, Double, Suite
        [Range(0, double.MaxValue, ErrorMessage = "قیمت نمی‌تواند منفی باشد")]
        public decimal PricePerNight { get; set; }
        [Range(1, 10, ErrorMessage = "ظرفیت باید بین 1 تا 10 نفر باشد")]
        public int Capacity { get; set; } //ظرفیت
        public bool IsAvailable { get; set; } = true;
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public Hotel Hotel { get; set; }


        public enum RoomType
        {
            Single = 1,
            Double = 2,
            Suite = 3,
            Deluxe = 4,
            Presidential = 5
        }
    }
}
