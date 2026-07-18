using System.Text.Json;
using FluentValidation;
using Quartz;
using QuartzScheduler.Shared.DTOs.Jobs;

namespace QuartzScheduler.API.Validators;
public class CreateHttpJobRequestValidator : AbstractValidator<CreateHttpJobRequest>
{
    public CreateHttpJobRequestValidator()
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

        RuleFor(x => x.Url)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("URL is required.")
            .MaximumLength(2048).WithMessage("URL must not exceed 2048 characters.")
            .Must(BeAValidHttpUrl)
            .WithMessage("URL must be a valid absolute http or https address.");

        RuleFor(x => x.HttpMethod)
            .IsInEnum().WithMessage("Invalid HTTP method.");

        RuleFor(x => x.Headers)
            .Must(BeValidJson)
            .WithMessage("Headers must be a valid JSON object")
            .When(x => !string.IsNullOrWhiteSpace(x.Headers));
    }

    private static bool BeAValidCronExpression(string cron) =>
        CronExpression.IsValidExpression(cron);

    // must parse AND be http/https
    private static bool BeAValidHttpUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uri)
        && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }

    private static bool BeValidJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return true;

        try
        {
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.ValueKind == JsonValueKind.Object;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}