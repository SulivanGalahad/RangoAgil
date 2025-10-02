using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.Entities;
using RangoAgil.API.Models;
using System.Collections;
using System.Xml.Linq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RangoDbContext>(
    o => o.UseSqlite(builder.Configuration["ConnectionStrings:RangoDbConStr"])
);

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

app.MapGet("/", () => "Hello World!");


app.MapGet("/rangos", async Task<Results<NoContent, Ok<IEnumerable<RangoDTO>>>>
    (RangoDbContext rangoDbContext,
    IMapper mapper,
    [FromQuery(Name = "name")] string? rangoNome) =>
{
    var rangosEntity = await rangoDbContext.Rangos
                               .Where(x => rangoNome == null || x.Nome.ToLower().Contains(rangoNome.ToLower()))
                               .ToListAsync();

    if (rangosEntity.Count <= 0 || rangosEntity == null)
    {
        return TypedResults.NoContent();
    }
    else
    {
        return TypedResults.Ok(mapper.Map<IEnumerable<RangoDTO>>(rangosEntity));
    }
});

app.MapGet("/rango/{rangoId:int}/ingredientes", async (
    RangoDbContext rangoDbContext,
    IMapper mapper,
    int rangoId) =>
{
    return mapper.Map<IEnumerable<IngredienteDTO>> ((await rangoDbContext.Rangos
                               .Include(rango => rango.Ingredientes)
                               .FirstOrDefaultAsync(rango => rango.Id == rangoId))?.Ingredientes);
});

app.MapGet("/rango/{id:int}", async (
    RangoDbContext rangoDbContext,
    IMapper mapper,
    int id) =>
{
    return mapper.Map<RangoDTO>(await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == id));
}).WithName("GetRango");

/* app.MapPost("/rango", async (
    RangoDbContext rangoDbContext,
    IMapper mapper,
    [FromBody] RangoForCreateDTO rangoForCreate,
    LinkGenerator linkGenerator,
    HttpContext httpContext) =>
    {
        var rangoEntity = mapper.Map<Rango>(rangoForCreate);
        rangoDbContext.Add(rangoEntity);
        await rangoDbContext.SaveChangesAsync();

        var rangoToReturn = mapper.Map<RangoDTO>(rangoEntity);

        var linkToReturn = linkGenerator.GetUriByName(
            httpContext,
            "GetRango",
            new { id = rangoToReturn.Id }
        );

        return TypedResults.Created(
            linkToReturn, rangoToReturn);
    }); */


    app.MapPost("/rango", async Task<CreatedAtRoute<RangoDTO>> (
    RangoDbContext rangoDbContext,
    IMapper mapper,
    [FromBody] RangoForCreateDTO rangoForCreate) =>
    {
        var rangoEntity = mapper.Map<Rango>(rangoForCreate);
        rangoDbContext.Add(rangoEntity);
        await rangoDbContext.SaveChangesAsync();

        var rangoToReturn = mapper.Map<RangoDTO>(rangoEntity);

        return TypedResults.CreatedAtRoute(rangoToReturn, "GetRango", new { id = rangoToReturn.Id });
    });

app.MapPut("/rango/{id:int}/ingredientes", async Task<Results<NotFound, Ok>> (
    RangoDbContext rangoDbContext,
    IMapper mapper,
    int id,
    [FromBody] RangoForUpdateDTO rangoForUpdateDTO) =>
{
    var rangoEntity = await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == id);

    if (rangoEntity == null)
    {
        return TypedResults.NotFound();
    }

    mapper.Map(rangoForUpdateDTO, rangoEntity);

    await rangoDbContext.SaveChangesAsync();

    return TypedResults.Ok();


});


app.Run();
