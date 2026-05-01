using Business.Common.Constants;
using Business.Common.Dtos.Request;
using FluentValidation;

namespace Business.Common.Validators
{
    public class CustomerCreateDtoValidator : AbstractValidator<CustomerCreate>
    {
        public CustomerCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(AppMessages.CustomerNameRequired)
                .MaximumLength(100).WithMessage(AppMessages.CustomerNameMaxLength);
        }
    }
}
