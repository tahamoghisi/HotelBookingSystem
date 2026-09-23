using FluentValidation;
using HotelBooking.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Application.Validator.Sorting
{
    public class RoomSortingValidator : AbstractValidator<SortingRequest>
    {
        public RoomSortingValidator()
        {
            RuleFor(x => x.SortBy)
            .Must(x => new[]
            {
                "Id",
                "RoomNumber",
                "Status"
            }.Contains(x))
            .WithMessage("Invalid sort field.");
        }
    }
}
