using Business.Common.Constants;
using Business.Dtos.Request;
using FluentValidation;

namespace Business.Validators
{
    public class CustomerCreateDtoValidator : AbstractValidator<CustomerCreate>
    {
        public CustomerCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                 .NotEmpty()
                 .WithMessage(AppMessages.CustomerNameRequired)

                 .MaximumLength(AppConstants.CustomerNameMaxLength)
                 .WithMessage(AppMessages.CustomerNameMaxLength)

                 .Matches(AppConstants.RegexOnlyLetters)
                 .WithMessage(AppMessages.CustomerNameOnlyLetters);
        }
    }
}
