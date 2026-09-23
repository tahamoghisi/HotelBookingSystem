using FluentValidation;
using HotelBooking.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Application.Validator.Sorting
{
    public class CustomerSortingValidator : AbstractValidator<SortingRequest>
    {
        public CustomerSortingValidator()
        {
            RuleFor(x => x.SortBy)
            .Must(x => new[]
            {
                "Id",
                "FullName",
                "Email"
            }.Contains(x))
            .WithMessage("Invalid sort field.");
        }
    }
}
