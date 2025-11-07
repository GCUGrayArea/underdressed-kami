using FluentValidation;
using SmartScheduler.Application.Commands;

namespace SmartScheduler.Application.Validators;

/// <summary>
/// Validates AssignContractorCommand inputs.
/// Business logic validation (contractor exists, job type matches) is done in handler.
/// </summary>
public class AssignContractorCommandValidator : AbstractValidator<AssignContractorCommand>
{
    public AssignContractorCommandValidator()
    {
        RuleFor(x => x.JobId)
            .NotEmpty()
            .WithMessage("Job ID is required");

        RuleFor(x => x.ContractorId)
            .NotEmpty()
            .WithMessage("Contractor ID is required");

        RuleFor(x => x.ScheduledStartTime)
            .GreaterThanOrEqualTo(DateTime.Today)
            .WithMessage("Scheduled start time must be today or in the future");
    }
}
