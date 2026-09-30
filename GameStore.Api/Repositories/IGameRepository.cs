using GameStore.Api.Models;

namespace GameStore.Api.Repositories;

public interface IGameRepository
{
    Task<IEnumerable<Game>> GetAllAsync(CancellationToken ct = default);
    Task<Game?> GetByIdAsync(int id, CancellationToken ct = default);
    Task CreateAsync(Game game, CancellationToken ct = default);
    Task UpdateAsync(Game game, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}