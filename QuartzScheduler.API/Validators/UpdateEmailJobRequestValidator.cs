using FluentValidation;
using Quartz;
using QuartzScheduler.Shared.DTOs.Jobs;

namespace QuartzScheduler.API.Validators;
public class UpdateEmailJobRequestValidator : AbstractValidator<UpdateEmailJobRequest>
{
    public UpdateEmailJobRequestValidator()
    {
        RuleFor(x => x.JobName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Job name is required.")
            .MaximumLength(100).WithMessage("Job name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

        RuleFor(x => x.CronExpression)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Cron expression is required.")
            .Must(BeAValidCronExpression)
            .WithMessage("Invalid cron expression. Example: '0 0 9 * * ?' runs daily at 9:00 AM.");

        RuleFor(x => x.ToAddress)
    .Cascade(CascadeMode.Stop)
    .NotEmpty().WithMessage("Recipient address is required.")
    .EmailAddress().WithMessage("A valid email address is required.")
    .Matches(@"^[^\s@]+@[^\s@]+\.[^\s@]+$")
    .WithMessage("Please enter a complete email address, like name@example.com.")
    .MaximumLength(256);

        RuleFor(x => x.Subject)
            .NotEmpty().WithMessage("Subject is required.")
            .MaximumLength(300).WithMessage("Subject must not exceed 300 characters.");

        RuleFor(x => x.Body)
            .NotEmpty().WithMessage("Email body is required.");
    }

    private static bool BeAValidCronExpression(string cron) =>
        CronExpression.IsValidExpression(cron);
}