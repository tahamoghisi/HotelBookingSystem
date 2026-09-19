using FluentValidation;
using HotelBooking.Application.DTOs.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Application.Validator.Customer
{
    public class UpdateCustomerValidator: AbstractValidator<UpdateCustomerDTO>
    {
        public UpdateCustomerValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage("نام و نام خانوادگی الزامی است.")
                .MaximumLength(100)
                .WithMessage("نام و نام خانوادگی نمی‌تواند بیشتر از 100 کاراکتر باشد.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("ایمیل الزامی است.")
                .EmailAddress()
                .WithMessage("ایمیل معتبر نیست.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage("شماره تلفن الزامی است.")
                .Matches(@"^09\d{9}$")
                .WithMessage("شماره تلفن معتبر نیست.");

            RuleFor(x => x.NationalCode)
                .NotEmpty()
                .WithMessage("کد ملی الزامی است.")
                .Length(10)
                .WithMessage("کد ملی باید 10 رقم باشد.")
                .Matches(@"^\d{10}$")
                .WithMessage("کد ملی باید فقط شامل اعداد باشد.");
        }
    }
}
