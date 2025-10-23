using Gaming1.Domain.Entities;

namespace Gaming1.Application.Interfaces;

public interface IGameRepository
{
    Task<Game?> GetAsync(Guid id);
    Task AddAsync(Game game);
    Task UpdateAsync(Game game);
    Task<IEnumerable<Game>> GetAllAsync();
}