using Npgsql;

namespace FreelancePlatform.Infrastructure;

public class PgUnitOfWork : IUnitOfWork
{
    private readonly NpgsqlConnection _connection;
    private NpgsqlTransaction? _transaction;

    public IProjectRepository Projects { get; }
    public IProposalRepository Proposals { get; }

    public PgUnitOfWork(string connectionString)
    {
        _connection = new NpgsqlConnection(connectionString);
        _connection.Open();

        Projects = new ProjectRepository(_connection, () => _transaction);
        Proposals = new ProposalRepository(_connection, () => _transaction);
    }

    public async Task BeginAsync()
    {
        _transaction = await _connection.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
        }

        await _connection.DisposeAsync();
    }
}
