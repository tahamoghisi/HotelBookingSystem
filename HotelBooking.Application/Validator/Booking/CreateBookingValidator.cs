using FluentValidation;
using HotelBooking.Application.DTOs.Booking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Application.Validator.Booking
{
    public class CreateBookingValidator : AbstractValidator<CreateBookingDTO>
    {
        public CreateBookingValidator()
        {
            RuleFor(x => x.CustomerId)
                .GreaterThan(0)
                .WithMessage("CustomerId must be greater than 0.");

            RuleFor(x => x.HotelId)
                .GreaterThan(0)
                .WithMessage("HotelId must be greater than 0.");

            RuleFor(x => x.RoomId)
                .GreaterThan(0)
                .WithMessage("RoomId must be greater than 0.");

            RuleFor(x => x.CheckInDate)
                .NotEmpty()
                .WithMessage("Check-in date is required.");

            RuleFor(x => x.CheckOutDate)
                .NotEmpty()
                .WithMessage("Check-out date is required.");

            RuleFor(x => x)
                .Must(x => x.CheckOutDate > x.CheckInDate)
                .WithMessage("Check-out date must be after check-in date.");
        }
    }
}
