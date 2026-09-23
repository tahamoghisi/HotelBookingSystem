using FluentValidation;
using FluentValidation.Validators;
using HotelBooking.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Application.Validator.Pagging
{
    public class PaginationRequestValidator : AbstractValidator<PaginationRequest>
    {
           public PaginationRequestValidator()
        {
            RuleFor(x =>x.Page)
                .GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100);
        }
    }
}
