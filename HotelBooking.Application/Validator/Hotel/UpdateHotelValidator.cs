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
                .WithMessage("نام هتل الزامی است")
                .MaximumLength(200)
                .WithMessage("نام هتل نمی‌تواند بیشتر از 200 کاراکتر باشد");

            RuleFor(x => x.Address)
                .NotEmpty()
                .WithMessage("آدرس الزامی است")
                .MaximumLength(500)
                .WithMessage("آدرس نمی‌تواند بیشتر از 500 کاراکتر باشد");

            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessage("شهر الزامی است")
                .MaximumLength(100)
                .WithMessage("شهر نمی‌تواند بیشتر از 100 کاراکتر باشد");

            RuleFor(x => x.Country)
                .MaximumLength(50)
                .WithMessage("کشور نمی‌تواند بیشتر از 50 کاراکتر باشد")
                .When(x => !string.IsNullOrEmpty(x.Country));

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20)
                .WithMessage("شماره تلفن نمی‌تواند بیشتر از 20 کاراکتر باشد")
                .Matches(@"^[0-9+\-\s()]+$")
                .WithMessage("شماره تلفن معتبر نیست")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("ایمیل معتبر نیست")
                .MaximumLength(100)
                .WithMessage("ایمیل نمی‌تواند بیشتر از 100 کاراکتر باشد")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.StarRating)
                .InclusiveBetween(1, 5)
                .WithMessage("ستاره هتل باید بین 1 تا 5 باشد");

            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage("توضیحات نمی‌تواند بیشتر از 1000 کاراکتر باشد")
                .When(x => !string.IsNullOrEmpty(x.Description));
        }
    }
}
