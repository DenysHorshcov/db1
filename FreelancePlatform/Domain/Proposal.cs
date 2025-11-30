namespace FreelancePlatform.Domain;

public class Proposal
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    public long FreelancerId { get; set; }
    public string? CoverLetter { get; set; }
    public decimal? ProposedBudget { get; set; }
    public string Status { get; set; } = "";
}
