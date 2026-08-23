using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Application.DTOs.Auth
{
    public class LoginRequest
    {
        public string userName {  get; set; }
        public string password { get; set; }
    }
}
