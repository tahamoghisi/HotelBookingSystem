using HotelBooking.Application.DTOs.User;
using HotelBooking.Application.Mapping.UserMap;
using HotelBooking.Application.ServiceInterface;
using HotelBooking.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Infrastructure.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOFWork _unitOFWork;
        public UserService(IUnitOFWork unitOFWork)
        {
            _unitOFWork = unitOFWork;
        }
        public async Task<IEnumerable<UserResponseDto>> GetAllAsync()
        {
            var users = await _unitOFWork.User.GetAllAsync();
            return users.Select(x => UserMapping.ToDto(x)).ToList();
        }
    }
}
