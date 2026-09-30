using GameStore.Api.Data;
using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Repositories;

public class GameRepository(GameStoreContext dbContext) : IGameRepository
{
    public async Task<IEnumerable<Game>> GetAllAsync(CancellationToken ct = default)
    {
        return await dbContext.Games
            .Include(game => game.Genre)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<Game?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await dbContext.Games
            .Include(game => game.Genre)
            .AsNoTracking()
            .FirstOrDefaultAsync(game => game.Id == id, ct);
    }

    public async Task CreateAsync(Game game, CancellationToken ct = default)
    {
        await dbContext.Games.AddAsync(game, ct);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Game game, CancellationToken ct = default)
    {
        dbContext.Games.Update(game);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        await dbContext.Games
            .Where(game => game.Id == id)
            .ExecuteDeleteAsync(ct);
    }
}