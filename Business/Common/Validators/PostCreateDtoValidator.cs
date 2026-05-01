using Business.Common.Constants;
using Business.Dtos.Request;
using FluentValidation;

namespace Business.Common.Validators
{
    public class PostCreateDtoValidator : AbstractValidator<PostCreate>
    {
        public PostCreateDtoValidator()
        {
            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithMessage(AppMessages.PostCustomerRequired);

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage(AppMessages.PostTitleRequired)
                .MaximumLength(100).WithMessage(AppMessages.PostTitleMaxLength);

            RuleFor(x => x.Body)
                .NotEmpty().WithMessage(AppMessages.PostBodyRequired);

            RuleFor(x => x.Type)
                .InclusiveBetween(1, 3).WithMessage(AppMessages.PostTypeRange);
        }
    }
}
