using FinanceControl.Shared.Dtos.Request;
using FinanceControl.Shared.Helpers;
using FluentValidation;

namespace FinanceControl.Services.Validations
{
    public class PatchUserValidator : AbstractValidator<PatchUserRequestDto>
    {
        public PatchUserValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name cannot be empty.")
                .MaximumLength(100).WithMessage("Name must be at most 100 characters.")
                .When(x => x.Name is not null);

            RuleFor(x => x.PreferredCurrency)
                .NotEmpty().WithMessage("PreferredCurrency cannot be empty.")
                .MaximumLength(10).WithMessage("PreferredCurrency must be at most 10 characters.")
                .When(x => x.PreferredCurrency is not null);

            RuleFor(x => x.PreferredLanguage)
                .NotEmpty().WithMessage("PreferredLanguage cannot be empty.")
                .MaximumLength(10).WithMessage("PreferredLanguage must be at most 10 characters.")
                .When(x => x.PreferredLanguage is not null);

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Country cannot be empty.")
                .Length(2).WithMessage("Country must be exactly 2 characters (ISO 3166-1 alpha-2).")
                .Must(c => c != null && IsoCountryCodes.Valid.Contains(c.ToUpper()))
                .WithMessage("Country must be a valid ISO 3166-1 alpha-2 code (e.g., BR, US, PT).")
                .When(x => x.Country is not null);
        }
    }
}
