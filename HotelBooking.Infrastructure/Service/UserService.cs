using HotelBooking.Application.DTOs.User;
using HotelBooking.Application.Mapping.UserMap;
using HotelBooking.Application.ServiceInterface;
using HotelBooking.Domain.Interfaces;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<UserService> _logger;
        public UserService(IUnitOFWork unitOFWork, ILogger<UserService> logger)
        {
            _unitOFWork = unitOFWork;
            _logger = logger;
        }
        public async Task<IEnumerable<UserResponseDto>> GetAllAsync()
        {
            var users = await _unitOFWork.User.GetAllAsync();
            var result = users.Select(x => UserMapping.ToDto(x)).ToList();
            _logger.LogDebug("Retrieved {UserCount} users.", result.Count);
            return result;
        }
    }
}
