using Candidate.API.Contracts;

namespace Candidate.API.Services;

public interface IDashboardService
{
    Task<DashboardResponse> GetAsync(CancellationToken cancellationToken);
}
