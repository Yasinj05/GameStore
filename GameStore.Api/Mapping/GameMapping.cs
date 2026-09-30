using GameStore.Api.Dtos;
using GameStore.Api.Models;

namespace GameStore.Api.Mapping;

public static class GameMapping
{
    public static Game ToEntity(this CreateGameDto dto) => new()
    {
        Name = dto.Name,
        GenreId = dto.GenreId,
        Price = dto.Price,
        ReleaseDate = dto.ReleaseDate
    };

    public static GameSummaryDto ToSummaryDto(this Game game) => new(
        game.Id,
        game.Name,
        game.Genre!.Name,
        game.Price,
        game.ReleaseDate
    );

    public static GameDetailsDto ToDetailsDto(this Game game) => new(
        game.Id,
        game.Name,
        game.GenreId,
        game.Price,
        game.ReleaseDate
    );
}