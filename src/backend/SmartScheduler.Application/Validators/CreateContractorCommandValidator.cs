using FluentValidation;
using SmartScheduler.Application.Commands;

namespace SmartScheduler.Application.Validators;

/// <summary>
/// Validator for CreateContractorCommand.
/// Validates contractor creation inputs according to business rules.
/// </summary>
public class CreateContractorCommandValidator
    : AbstractValidator<CreateContractorCommand>
{
    public CreateContractorCommandValidator()
    {
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

        RuleFor(x => x.Rating)
            .InclusiveBetween(0, 5)
            .WithMessage("Rating must be between 0.0 and 5.0");

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Invalid email address format");
    }
}
