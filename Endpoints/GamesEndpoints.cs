using System;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Dto;
using WebApplication1.Entities;
using WebApplication1.GameMapping;

namespace WebApplication1.Endpoints;

public static class GamesEndpoints
{
    const string GetGameEndpointName = "GetName";

    // Crud with a list in c#
    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("game");


        // getAll
        group.MapGet("/", async (GameStoreContext dbContext) =>
           await dbContext.Games.Include((x) => x.Genre).ToListAsync()
        );

        // getById
        group.MapGet("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            Game? game = await dbContext.Games.FindAsync(id);

            return game is null ?
                Results.NotFound("Este elemento no fue encontrado") : Results.Ok(game.ToGameDetailsDto());
        }).WithName(GetGameEndpointName);

        // Create
        group.MapPost("/", async (CreateGameDto newGame, GameStoreContext dbContext) =>
        {
            Game game = newGame.ToEntity();

            dbContext.Games.Add(game);
            await dbContext.SaveChangesAsync();

            return Results.CreatedAtRoute(GetGameEndpointName, new
            {
                id = game.Id
            }, game.ToGameDetailsDto());

        }).WithParameterValidation();


        // update
        group.MapPut("/{id}", async (int id, UpdateGameDto updateGameDto, GameStoreContext dbContext) =>
        {

            Game? existingGame = await dbContext.Games.FindAsync(id);

            if (existingGame is null)
            {
                return Results.NotFound("Este elemento no fue encontrado");
            }

            dbContext.Entry(existingGame).CurrentValues.SetValues(updateGameDto.ToEntity(id));

            await dbContext.SaveChangesAsync();

            return Results.NoContent();


        }).WithParameterValidation();

        // Delete
        group.MapDelete("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            await dbContext.Games.Where((x) => x.Id == id).ExecuteDeleteAsync();

            return Results.NoContent();
        });

        return group;
    }


}
