using Gaming1.Application.Interfaces;
using Gaming1.Domain.Entities;

namespace Gaming1.Application.Commands;

public record ListGamesQuery();

public class ListGamesHandler : IQueryHandler<ListGamesQuery, IEnumerable<Game>>
{
    private readonly IGameRepository _repo;

    public ListGamesHandler(IGameRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<Game>> Handle(ListGamesQuery query, CancellationToken cancellationToken)
    {
        return await _repo.GetAllAsync(cancellationToken);
    }
}
