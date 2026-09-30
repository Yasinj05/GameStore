using GameStore.Api.Dtos;
using GameStore.Api.Repositories;

namespace GameStore.Api.Endpoints;

public static class GenresEndpoints
{
    public static void MapGenresEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/genres");

        // Get All Genres
        group.MapGet("/", async (IGenreRepository repository, CancellationToken ct) =>
        {
            var genres = await repository.GetAllAsync(ct);

            var genreDtos = genres.Select(g => new GenreSummaryDto(g.Id, g.Name));

            return Results.Ok(genreDtos);
        });
    }
}