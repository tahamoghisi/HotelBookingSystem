using FluentValidation;
using HotelBooking.Application.DTOs.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Application.Validator.Room
{
    public class UpdateRoomValidator : AbstractValidator<UpdateRoomDTO>
    {
        public UpdateRoomValidator()
        {
            RuleFor(x => x.HotelId)
                .GreaterThan(0)
                .WithMessage("HotelId must be greater than 0.");

            RuleFor(x => x.RoomNumber)
                .GreaterThan(0)
                .WithMessage("Room number must be greater than 0.");

            RuleFor(x => x.Type)
                .IsInEnum()
                .WithMessage("Invalid room type.");

            RuleFor(x => x.PricePerNight)
                .GreaterThan(0)
                .WithMessage("Price per night must be greater than 0.");

            RuleFor(x => x.Capacity)
            .InclusiveBetween(1, 10)
            .WithMessage("Capacity must be between 1 and 10.");
        }
    }
}
