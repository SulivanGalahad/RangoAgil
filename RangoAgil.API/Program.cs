using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.EndpointHandlers;
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

rangosEndpoints.MapGet("", RangosHandlers.GetRangosAsync);
rangosWithIdEndpoints.MapGet("", RangosHandlers.GetRangosWithIdAsync).WithName("GetRangos");
rangosEndpoints.MapPost("", RangosHandlers.PostRangosAsync);
rangosWithIdEndpoints.MapPut("", RangosHandlers.PutRangosAsync);
rangosWithIdEndpoints.MapDelete("", RangosHandlers.DeleteRangosAsync);

ingredientesRangoEndpoints.MapGet("", IngredientesHandlers.GetIngredientesRangoAsync);


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


app.Run();
