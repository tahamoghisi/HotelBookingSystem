using FluentValidation;
using HotelBooking.Application.DTOs.Hotel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Application.Validator.Hotel
{
    public class UpdateHotelValidator : AbstractValidator<UpdateHotelDTO>
    {
        public UpdateHotelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Address)
                .NotEmpty()
                .MaximumLength(500);

            RuleFor(x => x.City)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Country)
                .MaximumLength(50)
                .When(x => !string.IsNullOrEmpty(x.Country));

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20)
                .Matches(@"^[0-9+\-\s()]+$")
                .WithMessage("شماره تلفن معتبر نیست")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("ایمیل معتبر نیست")
                .MaximumLength(100)
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.StarRating)
                .InclusiveBetween(1, 5)
                .WithMessage("ستاره هتل باید بین 1 تا 5 باشد");

            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .When(x => !string.IsNullOrEmpty(x.Description));
        }
    }
}
