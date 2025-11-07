using MediatR;
using SmartScheduler.Application.DTOs;
using SmartScheduler.Domain.Entities;

namespace SmartScheduler.Application.Queries;

/// <summary>
/// Query to retrieve jobs filtered by status.
/// Returns all jobs if status is not specified.
/// </summary>
public class GetJobsByStatusQuery : IRequest<IEnumerable<JobDto>>
{
    public JobStatus? Status { get; set; }

    public GetJobsByStatusQuery(JobStatus? status = null)
    {
        Status = status;
    }
}
