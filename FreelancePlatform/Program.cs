using System;
using System.Collections.Generic;
using FreelancePlatform.Domain;
using FreelancePlatform.Infrastructure;

const string ConnectionString =
    "Host=localhost;Port=5432;Database=freelance_platform;Username=postgres;Password=Zeus8185;";

Console.WriteLine("Freelance Platform demo started.");

await using IUnitOfWork uow = new PgUnitOfWork(ConnectionString);

try
{
    await uow.BeginAsync();

    // 1. Створюємо новий проект
    var createProjectDto = new CreateProjectDto(
        ClientId: 2,              // client1 (з твоїх тестових даних)
        CategoryId: 1,            // Web Development
        Title: "New landing page",
        Description: "Landing page for marketing campaign.",
        BudgetType: "fixed",
        BudgetMin: 500m,
        BudgetMax: 1000m
    );

    long currentUserId = 2;       // клієнт, який створює проект

    long projectId = await uow.Projects.CreateAsync(createProjectDto, currentUserId);
    Console.WriteLine($"Project created with id = {projectId}");

    // 2. Пропозиція від фрілансера
    var createProposalDto = new CreateProposalDto(
        ProjectId: projectId,
        FreelancerId: 4,  // John Developer
        CoverLetter: "I can create a high-converting landing page.",
        ProposedBudget: 800m
    );

    long freelancerUserId = 4;

    long proposalId = await uow.Proposals.CreateAsync(createProposalDto, freelancerUserId);
    Console.WriteLine($"Proposal created with id = {proposalId}");

    // 3. Клієнт приймає пропозицію -> створюється контракт
    await uow.Proposals.AcceptAsync(proposalId, currentUserId);
    Console.WriteLine("Proposal accepted and contract created.");

    // 4. Виводимо всі активні проєкти (з в'ю v_active_projects)
    IEnumerable<Project> activeProjects = await uow.Projects.GetActiveAsync();
    Console.WriteLine("Active projects:");
    foreach (var p in activeProjects)
    {
        Console.WriteLine($" - [{p.Id}] {p.Title} ({p.Status})");
    }

    await uow.CommitAsync();
    Console.WriteLine("Transaction committed.");
}
catch (Exception ex)
{
    Console.WriteLine("Error: " + ex.Message);
    await uow.RollbackAsync();
}
