using FluentValidation;
using SmartScheduler.Application.Commands;

namespace SmartScheduler.Application.Validators;

/// <summary>
/// Validates UpdateJobCommand inputs.
/// </summary>
public class UpdateJobCommandValidator : AbstractValidator<UpdateJobCommand>
{
    public UpdateJobCommandValidator()
    {
        RuleFor(x => x.JobId)
            .NotEmpty()
            .WithMessage("Job ID is required");

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
