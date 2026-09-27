using FluentValidation;
using HotelBooking.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Application.Validator.User
{
    public class ChangePasswordValidator : AbstractValidator<AdminChangePasswordDto>
    {
        public ChangePasswordValidator() 
        {
            RuleFor(x => x.newPassword)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters.");
        }
    }
}
