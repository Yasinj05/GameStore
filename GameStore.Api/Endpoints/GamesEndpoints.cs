using GameStore.Api.Dtos;
using GameStore.Api.Exceptions;
using GameStore.Api.Mapping;
using GameStore.Api.Repositories;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    public static void MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("games");

        // Get All Games
        group.MapGet("/", async (IGameRepository repository, CancellationToken ct) =>
        {
            var games = await repository.GetAllAsync(ct);
            return Results.Ok(games.Select(g => g.ToSummaryDto()));
        });

        // Get Game by Id
        group.MapGet("/{id:int}", async (int id, IGameRepository repository, CancellationToken ct) =>
        {
            var game = await repository.GetByIdAsync(id, ct);
            return game is null ? Results.NotFound() : Results.Ok(game.ToDetailsDto());
        });

        // Create Game
        group.MapPost("/create", async (CreateGameDto newGame, IGameRepository gameRepository, IGenreRepository genreRepository, CancellationToken ct) =>
        {
            var genreExist = await genreRepository.ExistsAsync(newGame.GenreId, ct);
            if (!genreExist)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    {nameof (newGame.GenreId), [$"ژانری با شناسه {newGame.GenreId} وجود ندارد."]}
                });
            }

            var game = newGame.ToEntity();
            await gameRepository.CreateAsync(game, ct);

            return Results.Ok(game.ToDetailsDto());
        });

        // Update Game
        group.MapPut("/update/{id:int}", async (int id, UpdateGameDto updatedGame, IGameRepository gameRepository, IGenreRepository genreRepository, CancellationToken ct) =>
        {
            var existingGame = await gameRepository.GetByIdAsync(id, ct);
            if (existingGame is null)
            {
                throw new NotFoundException($"بازی با شناسه {id} پیدا نشد.");
            }

            if (existingGame.GenreId != updatedGame.GenreId)
            {
                var genreExists = await genreRepository.ExistsAsync(updatedGame.GenreId, ct);
                if (!genreExists)
                {
                    throw new ValidationException(new Dictionary<string, string[]>
            {
                { nameof(updatedGame.GenreId), [$"ژانری با شناسه {updatedGame.GenreId} وجود ندارد."] }
            });
                }
            }

            existingGame.Name = updatedGame.Name;
            existingGame.GenreId = updatedGame.GenreId;
            existingGame.Price = updatedGame.Price;
            existingGame.ReleaseDate = updatedGame.ReleaseDate;
            existingGame.Version = updatedGame.Version;

            await gameRepository.UpdateAsync(existingGame, ct);
            return Results.NoContent();
        });

        // Delete Game
        group.MapDelete("/delete/{id:int}", async (int id, IGameRepository repository, CancellationToken ct) =>
        {
            await repository.DeleteAsync(id, ct);
            return Results.NoContent();
        });
    }
}