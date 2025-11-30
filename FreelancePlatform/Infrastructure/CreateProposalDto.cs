namespace FreelancePlatform.Infrastructure;

public record CreateProposalDto(
    long ProjectId,
    long FreelancerId,
    string? CoverLetter,
    decimal? ProposedBudget
);
