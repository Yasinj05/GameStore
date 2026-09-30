using GameStore.Api.Data;
using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Repositories;

public class GenreRepository(GameStoreContext dbContext) : IGenreRepository
{
    public async Task<IEnumerable<Genre>> GetAllAsync(CancellationToken ct = default)
    {
        return await dbContext.Genres.AsNoTracking().ToListAsync(ct);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
    {
        return await dbContext.Genres.AnyAsync(g => g.Id == id, ct);
    }

}