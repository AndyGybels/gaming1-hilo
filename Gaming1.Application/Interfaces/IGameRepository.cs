using Gaming1.Domain.Entities;

namespace Gaming1.Application.Interfaces;

public interface IGameRepository
{
    Task<Game?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Game game, CancellationToken cancellationToken);
    Task UpdateAsync(Game game, CancellationToken cancellationToken);
    Task<IEnumerable<Game>> GetAllAsync(CancellationToken cancellationToken);
}