namespace FreelancePlatform.Infrastructure;

public interface IProposalRepository
{
    Task<long> CreateAsync(CreateProposalDto dto, long currentUserId);
    Task AcceptAsync(long proposalId, long currentUserId);
}
