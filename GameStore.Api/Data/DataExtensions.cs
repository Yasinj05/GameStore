using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data;

public static class DataExtensions
{
    public static void MigrateDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GameStoreContext>();
        dbContext.Database.Migrate();
    }
    public static void AddGameStoreDb(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("GameStore");

        builder.Services.AddDbContext<GameStoreContext>(options =>
        {
            options.UseSqlServer(connectionString);

            options.UseSeeding((context, _) =>
            {
                if (!context.Set<Genre>().Any())
                {
                    context.Set<Genre>().AddRange(
                        new Genre { Name = "Action" },
                        new Genre { Name = "Adventure" },
                        new Genre { Name = "RPG" },
                        new Genre { Name = "Simulation" },
                        new Genre { Name = "Strategy" }
                    );
                    context.SaveChanges();
                }
            });
        });
    }
}
