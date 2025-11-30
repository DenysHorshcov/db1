namespace FreelancePlatform.Infrastructure;

public record CreateProjectDto(
    long ClientId,
    int? CategoryId,
    string Title,
    string? Description,
    string BudgetType,
    decimal? BudgetMin,
    decimal? BudgetMax
);
