using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Application.DTOs.Auth
{
    public class RegisterResponse
    {
        public string AccessToken {  get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
