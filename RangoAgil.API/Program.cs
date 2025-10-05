using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.Entities;
using RangoAgil.API.Models;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RangoDbContext>(
    o => o.UseSqlite(builder.Configuration["ConnectionStrings:RangoDbConStr"])
);

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

var rangosEndpoints = app.MapGroup("/rangos");
var rangosWithIdEndpoints = rangosEndpoints.MapGroup("/{rangoId:int}");
var ingredientesRangoEndpoints = rangosWithIdEndpoints.MapGroup("/ingredientes");

rangosEndpoints.MapGet("", async Task<Results<NoContent, Ok<IEnumerable<RangoDTO>>>>
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

ingredientesRangoEndpoints.MapGet("", async (
    RangoDbContext rangoDbContext,
    IMapper mapper,
    int rangoId) =>
{
    return mapper.Map<IEnumerable<IngredienteDTO>> ((await rangoDbContext.Rangos
                               .Include(rango => rango.Ingredientes)
                               .FirstOrDefaultAsync(rango => rango.Id == rangoId))?.Ingredientes);
});

rangosWithIdEndpoints.MapGet("", async (
    RangoDbContext rangoDbContext,
    IMapper mapper,
    int rangoId) =>
{
    return mapper.Map<RangoDTO>(await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == rangoId));
}).WithName("GetRangos");

/* app.MapPost("/rangos", async (
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


    rangosEndpoints.MapPost("", async Task<CreatedAtRoute<RangoDTO>> (
    RangoDbContext rangoDbContext,
    IMapper mapper,
    [FromBody] RangoForCreateDTO rangoForCreate) =>
    {
        var rangoEntity = mapper.Map<Rango>(rangoForCreate);
        rangoDbContext.Add(rangoEntity);
        await rangoDbContext.SaveChangesAsync();

        var rangoToReturn = mapper.Map<RangoDTO>(rangoEntity);

        return TypedResults.CreatedAtRoute(
            rangoToReturn, 
            "GetRangos", 
            new { rangoId = rangoToReturn.Id }
            );
    });


rangosWithIdEndpoints.MapPut("", async Task<Results<NotFound, Ok>> (
    RangoDbContext rangoDbContext,
    IMapper mapper,
    int rangoId,
    [FromBody] RangoForUpdateDTO rangoForUpdateDTO) =>
{
    var rangoEntity = await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == rangoId);

    if (rangoEntity == null)
    {
        return TypedResults.NotFound();
    }

    mapper.Map(rangoForUpdateDTO, rangoEntity);

    await rangoDbContext.SaveChangesAsync();

    return TypedResults.Ok();

});

rangosWithIdEndpoints.MapDelete("", async Task<Results<NotFound, NoContent>> (
    RangoDbContext rangoDbContext,
    int rangoId) =>
{
    var rangoEntity = await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == rangoId);

    if (rangoEntity == null)
    {
        return TypedResults.NotFound();
    }

    rangoDbContext.Rangos.Remove(rangoEntity);
   
    await rangoDbContext.SaveChangesAsync();

    return TypedResults.NoContent();

});


app.Run();
