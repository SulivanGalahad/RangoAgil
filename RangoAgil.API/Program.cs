using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.Entities;
using System.Xml.Linq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RangoDbContext>(
    o => o.UseSqlite(builder.Configuration["ConnectionStrings:RangoDbConStr"])
);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");


app.MapGet("/rangos", async Task<Results<NoContent, Ok<List<Rango>>>> 
    (RangoDbContext rangoDbContext, 
    [FromQuery(Name = "name")] string? rangoNome) =>
{
    var rangosEntity = await rangoDbContext.Rangos
                               .Where(x => rangoNome == null || x.Nome.ToLower().Contains(rangoNome.ToLower())).
                               ToListAsync();
    if (rangosEntity.Count <= 0  || rangosEntity == null)
    {
        return TypedResults.NoContent();
    }
    else
    {
        return TypedResults.Ok(rangosEntity);
    }
});

app.MapGet("/rango/{id:int}", async (RangoDbContext rangoDbContext, int id) =>
{
    return await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == id);
});


app.Run();
