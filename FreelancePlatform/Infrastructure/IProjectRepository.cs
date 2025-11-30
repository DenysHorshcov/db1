using FreelancePlatform.Domain;

namespace FreelancePlatform.Infrastructure;

public interface IProjectRepository
{
    Task<long> CreateAsync(CreateProjectDto dto, long currentUserId);
    Task<IEnumerable<Project>> GetActiveAsync();
    Task SoftDeleteAsync(long projectId, long currentUserId);
}
