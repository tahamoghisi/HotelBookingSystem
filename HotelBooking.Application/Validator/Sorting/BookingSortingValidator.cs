using FluentValidation;
using HotelBooking.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Application.Validator.Sorting
{
    public class BookingSortingValidator : AbstractValidator<SortingRequest>
    {
        public BookingSortingValidator()
        {
            RuleFor(x => x.SortBy)
            .Must(x => new[]
            {
                "Id",
                "CustomerId",
                "RoomId",
                "Status",
                "CheckInDate",
                "CheckOutDate"
            }.Contains(x))
            .WithMessage("Invalid sort field.");
        }
    }
}
