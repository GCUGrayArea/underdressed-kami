using FluentValidation;
using SmartScheduler.Application.Commands;
using SmartScheduler.Domain.Interfaces;

namespace SmartScheduler.Application.Validators;

/// <summary>
/// Validates CreateJobCommand inputs including job type existence check.
/// </summary>
public class CreateJobCommandValidator : AbstractValidator<CreateJobCommand>
{
    public CreateJobCommandValidator()
    {
        RuleFor(x => x.JobTypeId)
            .NotEmpty()
            .WithMessage("Job type ID is required");

        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required")
            .MaximumLength(100)
            .WithMessage("Customer ID cannot exceed 100 characters");

        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .WithMessage("Customer name is required")
            .Length(2, 200)
            .WithMessage("Customer name must be between 2 and 200 characters");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90.0, 90.0)
            .WithMessage("Latitude must be between -90 and 90");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180.0, 180.0)
            .WithMessage("Longitude must be between -180 and 180");

        RuleFor(x => x.DesiredDate)
            .GreaterThanOrEqualTo(DateTime.Today)
            .WithMessage("Desired date must be today or in the future");

        RuleFor(x => x.EstimatedDurationHours)
            .GreaterThan(0)
            .WithMessage("Estimated duration must be positive")
            .LessThanOrEqualTo(24)
            .WithMessage("Estimated duration cannot exceed 24 hours");
    }
}
