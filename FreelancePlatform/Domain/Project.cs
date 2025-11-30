namespace FreelancePlatform.Domain;

public class Project
{
    public long Id { get; set; }
    public long ClientId { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public string BudgetType { get; set; } = "";
    public decimal? BudgetMin { get; set; }
    public decimal? BudgetMax { get; set; }
    public string Status { get; set; } = "";
}
