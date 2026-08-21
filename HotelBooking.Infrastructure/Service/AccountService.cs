using HotelBooking.Application.DTOs.User;
using HotelBooking.Application.Mapping.UserMap;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Infrastructure.Service
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<bool> Register(RegisterDto registerDto)
        {
            if (registerDto == null)
            {
                return false;
            }
            else
            {
                var user = UserMapping.ToEntity(registerDto);
                var check = await _accountRepository.Register(user);
                if (check == false) return false;
                return true;
            }   
        }
    }
}
