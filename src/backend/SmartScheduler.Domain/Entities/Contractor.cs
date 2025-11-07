using SmartScheduler.Domain.ValueObjects;

namespace SmartScheduler.Domain.Entities;

/// <summary>
/// Represents a flooring contractor who can be assigned to jobs.
/// </summary>
public class Contractor
{
    public Guid Id { get; private set; }
    public string FormattedId { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public Guid JobTypeId { get; private set; }
    public decimal Rating { get; private set; }
    public Location BaseLocation { get; private set; } = null!;
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public bool IsActive { get; private set; }

    // EF Core constructor
    private Contractor() { }

    public Contractor(
        string formattedId,
        string name,
        Guid jobTypeId,
        Location baseLocation,
        string? phone = null,
        string? email = null,
        decimal rating = 3.0m)
    {
        if (string.IsNullOrWhiteSpace(formattedId))
            throw new ArgumentException("Formatted ID cannot be empty", nameof(formattedId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        if (name.Length < 2 || name.Length > 100)
            throw new ArgumentException(
                "Name must be between 2 and 100 characters",
                nameof(name));

        if (jobTypeId == Guid.Empty)
            throw new ArgumentException("Job type ID cannot be empty", nameof(jobTypeId));

        if (rating < 0 || rating > 5)
            throw new ArgumentOutOfRangeException(
                nameof(rating),
                "Rating must be between 0.0 and 5.0");

        Id = Guid.NewGuid();
        FormattedId = formattedId;
        Name = name.Trim();
        JobTypeId = jobTypeId;
        BaseLocation = baseLocation;
        Phone = phone?.Trim();
        Email = email?.Trim();
        Rating = Math.Round(rating, 1); // Round to 1 decimal place
        IsActive = true;
    }

    public void UpdateDetails(
        string name,
        Guid jobTypeId,
        Location baseLocation,
        string? phone = null,
        string? email = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        if (name.Length < 2 || name.Length > 100)
            throw new ArgumentException(
                "Name must be between 2 and 100 characters",
                nameof(name));

        if (jobTypeId == Guid.Empty)
            throw new ArgumentException("Job type ID cannot be empty", nameof(jobTypeId));

        Name = name.Trim();
        JobTypeId = jobTypeId;
        BaseLocation = baseLocation;
        Phone = phone?.Trim();
        Email = email?.Trim();
    }

    public void UpdateRating(decimal newRating)
    {
        if (newRating < 0 || newRating > 5)
            throw new ArgumentOutOfRangeException(
                nameof(newRating),
                "Rating must be between 0.0 and 5.0");

        Rating = Math.Round(newRating, 1);
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}
