namespace SmartScheduler.Application.DTOs.Contractors;

/// <summary>
/// Lightweight DTO for contractor list display.
/// Contains essential information without nested data to optimize list queries.
/// </summary>
public class ContractorListItemDto
{
    public Guid Id { get; init; }
    public string FormattedId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string JobTypeName { get; init; } = string.Empty;
    public decimal Rating { get; init; }
    public bool IsActive { get; init; }
    public string? Phone { get; init; }
    public string? Email { get; init; }

    public ContractorListItemDto() { }

    public ContractorListItemDto(
        Guid id,
        string formattedId,
        string name,
        string jobTypeName,
        decimal rating,
        bool isActive,
        string? phone = null,
        string? email = null)
    {
        Id = id;
        FormattedId = formattedId;
        Name = name;
        JobTypeName = jobTypeName;
        Rating = rating;
        IsActive = isActive;
        Phone = phone;
        Email = email;
    }
}
