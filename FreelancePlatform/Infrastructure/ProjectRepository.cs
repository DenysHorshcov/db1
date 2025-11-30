using System.Data;
using Dapper;
using FreelancePlatform.Domain;
using Npgsql;

namespace FreelancePlatform.Infrastructure;

public class ProjectRepository : IProjectRepository
{
    private readonly NpgsqlConnection _connection;
    private readonly Func<NpgsqlTransaction?> _txAccessor;

    public ProjectRepository(
        NpgsqlConnection connection,
        Func<NpgsqlTransaction?> txAccessor)
    {
        _connection = connection;
        _txAccessor = txAccessor;
    }

    public async Task<long> CreateAsync(CreateProjectDto dto, long currentUserId)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_client_id", dto.ClientId);
        parameters.Add("p_category_id", dto.CategoryId);
        parameters.Add("p_title", dto.Title);
        parameters.Add("p_description", dto.Description);
        parameters.Add("p_budget_type", dto.BudgetType);
        parameters.Add("p_budget_min", dto.BudgetMin);
        parameters.Add("p_budget_max", dto.BudgetMax);
        parameters.Add("p_user_id", currentUserId);
        parameters.Add("p_project_id", dbType: DbType.Int64, direction: ParameterDirection.Output);

        await _connection.ExecuteAsync(
            "call sp_create_project(@p_client_id, @p_category_id, @p_title, @p_description," +
            " @p_budget_type, @p_budget_min, @p_budget_max, @p_user_id, @p_project_id)",
            parameters,
            _txAccessor());

        return parameters.Get<long>("p_project_id");
    }

    public async Task<IEnumerable<Project>> GetActiveAsync()
    {
        const string sql = @"
            select
                id,
                client_id,
                title,
                description,
                budget_type,
                budget_min,
                budget_max,
                status
            from v_active_projects";

        return await _connection.QueryAsync<Project>(sql, transaction: _txAccessor());
    }

    public async Task SoftDeleteAsync(long projectId, long currentUserId)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_project_id", projectId);
        parameters.Add("p_user_id", currentUserId);

        await _connection.ExecuteAsync(
            "call sp_soft_delete_project(@p_project_id, @p_user_id)",
            parameters,
            _txAccessor());
    }
}
