using FluentValidation;
using SmartScheduler.Application.Commands;

namespace SmartScheduler.Application.Validators;

/// <summary>
/// Validator for UpdateContractorCommand.
/// Validates contractor update inputs according to business rules.
/// </summary>
public class UpdateContractorCommandValidator
    : AbstractValidator<UpdateContractorCommand>
{
    public UpdateContractorCommandValidator()
    {
        RuleFor(x => x.ContractorId)
            .NotEmpty()
            .WithMessage("Contractor ID is required");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Contractor name is required")
            .Length(2, 100)
            .WithMessage("Name must be between 2 and 100 characters");

        RuleFor(x => x.JobTypeId)
            .NotEmpty()
            .WithMessage("Job type is required");

        RuleFor(x => x.BaseLocation)
            .NotNull()
            .WithMessage("Base location is required");

        RuleFor(x => x.BaseLocation.Latitude)
            .InclusiveBetween(-90, 90)
            .When(x => x.BaseLocation != null)
            .WithMessage("Latitude must be between -90 and 90 degrees");

        RuleFor(x => x.BaseLocation.Longitude)
            .InclusiveBetween(-180, 180)
            .When(x => x.BaseLocation != null)
            .WithMessage("Longitude must be between -180 and 180 degrees");

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Invalid email address format");
    }
}
