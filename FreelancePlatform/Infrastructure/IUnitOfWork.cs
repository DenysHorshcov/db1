namespace FreelancePlatform.Infrastructure;

public interface IUnitOfWork : IAsyncDisposable
{
    IProjectRepository Projects { get; }
    IProposalRepository Proposals { get; }

    Task BeginAsync();
    Task CommitAsync();
    Task RollbackAsync();
}
