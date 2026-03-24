using FinanceControl.Shared.Dtos.Request;
using FluentValidation;

namespace FinanceControl.Services.Validations
{
    public class UpdateWishlistItemValidator : AbstractValidator<UpdateWishlistItemRequestDto>
    {
        public UpdateWishlistItemValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name must not be empty.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.")
                .When(x => x.Name != null);

            RuleFor(x => x.TargetPrice)
                .GreaterThan(0).WithMessage("TargetPrice must be greater than 0.")
                .When(x => x.TargetPrice.HasValue);

            RuleFor(x => x.Priority)
                .IsInEnum().WithMessage("Priority must be Low, Medium or High.")
                .When(x => x.Priority.HasValue);

            RuleFor(x => x.ImageUrl)
                .MaximumLength(500).WithMessage("ImageUrl must not exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.ImageUrl));

            RuleFor(x => x.Deadline)
                .GreaterThan(DateOnly.FromDateTime(DateTime.Today))
                .WithMessage("Deadline must be a future date.")
                .When(x => x.Deadline.HasValue);
        }
    }
}
