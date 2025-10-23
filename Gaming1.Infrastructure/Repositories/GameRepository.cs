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

    public async Task<Game?> GetAsync(Guid id)
        => await _context.Games.FirstOrDefaultAsync(g => g.Id == id);

    public async Task AddAsync(Game game)
    {
        _context.Games.Add(game);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Game game)
    {
        _context.Games.Update(game);
        await _context.SaveChangesAsync();
    }
}