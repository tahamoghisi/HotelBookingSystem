using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Domain.Entities
{
    public class Hotel : BaseEntity
    {
        [Required(ErrorMessage = "نام هتل الزامی است")]
        [MaxLength(200, ErrorMessage = "نام هتل نمی‌تواند بیشتر از 200 کاراکتر باشد")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "آدرس الزامی است")]
        [MaxLength(500, ErrorMessage = "آدرس نمی‌تواند بیشتر از 500 کاراکتر باشد")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "شهر الزامی است")]
        [MaxLength(100, ErrorMessage = "شهر نمی‌تواند بیشتر از 100 کاراکتر باشد")]
        public string City { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Country { get; set; }

        [Phone(ErrorMessage = "شماره تلفن معتبر نیست")]
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "ایمیل معتبر نیست")]
        [MaxLength(100)]
        public string? Email { get; set; }

        [Range(1, 5, ErrorMessage = "ستاره هتل باید بین 1 تا 5 باشد")]
        public int StarRating { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;


        // Navigation Properties
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
        public ICollection<Booking> Booking { get; set; } = new List<Booking>();
    }
}
