using FinanceControl.Shared.Dtos.Request;
using FinanceControl.Shared.Enums;
using FluentValidation;

namespace FinanceControl.Services.Validations
{
    public class CreateAccountValidator : AbstractValidator<CreateAccountRequestDto>
    {
        public CreateAccountValidator()
        {
            RuleFor(a => a.Name).NotEmpty().WithMessage("Name is required.");

            RuleFor(a => a.AccountType).IsInEnum()
                .WithMessage($"AccountType must be one of: {string.Join(", ", Enum.GetNames<EnumAccountType>())}.");

            // Credit account: CreditLimit and BillingDueDay required
            RuleFor(a => a.CreditLimit)
                .NotNull().WithMessage("CreditLimit is required for Credit accounts.")
                .GreaterThan(0).WithMessage("CreditLimit must be greater than 0.")
                .When(a => a.AccountType == EnumAccountType.Credit);

            RuleFor(a => a.BillingDueDay)
                .NotNull().WithMessage("BillingDueDay is required for Credit accounts.")
                .InclusiveBetween(1, 31).WithMessage("BillingDueDay must be between 1 and 31.")
                .When(a => a.AccountType == EnumAccountType.Credit);

            // Non-credit accounts: CreditLimit and BillingDueDay not allowed
            RuleFor(a => a.CreditLimit)
                .Null().WithMessage("CreditLimit is only valid for Credit accounts.")
                .When(a => a.AccountType != EnumAccountType.Credit);

            RuleFor(a => a.BillingDueDay)
                .Null().WithMessage("BillingDueDay is only valid for Credit accounts.")
                .When(a => a.AccountType != EnumAccountType.Credit);

            // GoalAmount: only for Savings
            RuleFor(a => a.GoalAmount)
                .GreaterThan(0).WithMessage("GoalAmount must be greater than 0.")
                .When(a => a.GoalAmount.HasValue);

            RuleFor(a => a.GoalAmount)
                .Null().WithMessage("GoalAmount is only valid for Savings accounts.")
                .When(a => a.AccountType != EnumAccountType.Savings && a.GoalAmount.HasValue);
        }
    }
}
