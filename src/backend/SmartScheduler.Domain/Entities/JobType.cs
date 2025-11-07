namespace SmartScheduler.Domain.Entities;

/// <summary>
/// Represents a type of flooring job that can be assigned to contractors.
/// Extensible via database seeding - new types can be added without code deployment.
/// </summary>
public class JobType
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    // EF Core constructor
    private JobType() { }

    public JobType(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Job type name cannot be empty", nameof(name));

        if (name.Length > 100)
            throw new ArgumentException("Job type name cannot exceed 100 characters", nameof(name));

        Id = Guid.NewGuid();
        Name = name.Trim();
        Description = description?.Trim();
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void UpdateDetails(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Job type name cannot be empty", nameof(name));

        if (name.Length > 100)
            throw new ArgumentException("Job type name cannot exceed 100 characters", nameof(name));

        Name = name.Trim();
        Description = description?.Trim();
    }
}
