using FluentValidation;
using HotelBooking.Application.DTOs.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Application.Validator.Customer
{
    public class CreateCustomerValidator : AbstractValidator<CreateCustomerDTO>
    {
        public CreateCustomerValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .Matches(@"^09\d{9}$")
                .WithMessage("شماره تلفن معتبر نیست.");

            RuleFor(x => x.NationalCode)
                .NotEmpty()
                .Length(10)
                .Matches(@"^\d{10}$")
                .WithMessage("کد ملی باید 10 رقم باشد.");
        }
    }
}
