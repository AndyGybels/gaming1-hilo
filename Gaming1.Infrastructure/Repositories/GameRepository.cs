using Gaming1.Application.Interfaces;
using Gaming1.Domain.Entities;
using Gaming1.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gaming1.Infrastructure.Repositories;

public class GameRepository : IGameRepository
{
    private readonly GameDbContext _context;

    public GameRepository(GameDbContext context)
    {
        _context = context;
    }

    public async Task<Game?> GetAsync(Guid id, CancellationToken cancellationToken)
        => await _context.Games.SingleOrDefaultAsync(g => g.Id == id, cancellationToken);

    public async Task AddAsync(Game game, CancellationToken cancellationToken)
    {
        _context.Games.Add(game);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Game game, CancellationToken cancellationToken)
    {
        _context.Games.Update(game);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Game>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Games.AsNoTracking().ToListAsync(cancellationToken);
    }
}