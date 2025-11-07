using MediatR;
using SmartScheduler.Application.DTOs;

namespace SmartScheduler.Application.Queries;

/// <summary>
/// Query to retrieve a single job by its ID.
/// Returns null if job not found.
/// </summary>
public class GetJobByIdQuery : IRequest<JobDto?>
{
    public Guid JobId { get; set; }

    public GetJobByIdQuery(Guid jobId)
    {
        JobId = jobId;
    }
}
