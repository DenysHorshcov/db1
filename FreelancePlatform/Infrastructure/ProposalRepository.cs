using System.Data;
using Dapper;
using Npgsql;

namespace FreelancePlatform.Infrastructure;

public class ProposalRepository : IProposalRepository
{
    private readonly NpgsqlConnection _connection;
    private readonly Func<NpgsqlTransaction?> _txAccessor;

    public ProposalRepository(
        NpgsqlConnection connection,
        Func<NpgsqlTransaction?> txAccessor)
    {
        _connection = connection;
        _txAccessor = txAccessor;
    }

    public async Task<long> CreateAsync(CreateProposalDto dto, long currentUserId)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_project_id", dto.ProjectId);
        parameters.Add("p_freelancer_id", dto.FreelancerId);
        parameters.Add("p_cover_letter", dto.CoverLetter);
        parameters.Add("p_proposed_budget", dto.ProposedBudget);
        parameters.Add("p_user_id", currentUserId);
        parameters.Add("p_proposal_id", dbType: DbType.Int64, direction: ParameterDirection.Output);

        await _connection.ExecuteAsync(
            "call sp_create_proposal(@p_project_id, @p_freelancer_id, @p_cover_letter," +
            " @p_proposed_budget, @p_user_id, @p_proposal_id)",
            parameters,
            _txAccessor());

        return parameters.Get<long>("p_proposal_id");
    }

    public async Task AcceptAsync(long proposalId, long currentUserId)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_proposal_id", proposalId);
        parameters.Add("p_user_id", currentUserId);

        await _connection.ExecuteAsync(
            "call sp_accept_proposal(@p_proposal_id, @p_user_id)",
            parameters,
            _txAccessor());
    }
}
