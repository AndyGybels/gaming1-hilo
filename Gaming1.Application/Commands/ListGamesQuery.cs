using Gaming1.Application.Interfaces;
using Gaming1.Domain.Entities;

namespace Gaming1.Application.Commands;

public record ListGamesQuery();

public class ListGamesHandler
{
    private readonly IGameRepository _repo;

    public ListGamesHandler(IGameRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<Game>> Handle(ListGamesQuery query)
    {
        return await _repo.GetAllAsync();
    }
}
