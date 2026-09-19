using FluentValidation;
using HotelBooking.Application.DTOs.Booking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Application.Validator.Booking
{
    public class UpdateBookingValidator : AbstractValidator<UpdateBookingDTO>
    {
        public UpdateBookingValidator()
        {
            RuleFor(x => x.CustomerId)
                .GreaterThan(0);

            RuleFor(x => x.HotelId)
                .GreaterThan(0);

            RuleFor(x => x.RoomId)
                .GreaterThan(0);

            RuleFor(x => x.CheckInDate)
                .NotEmpty();

            RuleFor(x => x.CheckOutDate)
                .NotEmpty();

            RuleFor(x => x)
                .Must(x => x.CheckOutDate > x.CheckInDate)
                .WithMessage("Check-out date must be after check-in date.");
        }
    }
}
