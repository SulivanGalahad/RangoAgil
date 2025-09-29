using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RangoDbContext>(
    o => o.UseSqlite(builder.Configuration["ConnectionStrings:RangoDbConStr"])
);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/rangos", async (RangoDbContext rangoDbContext) =>
{
    return await rangoDbContext.Rangos.ToListAsync();
});

app.MapGet("/rango/{nome}", async (RangoDbContext rangoDbContext, string nome) =>
{
    return await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Nome == nome);
});

app.MapGet("/rango", async (RangoDbContext rangoDbContext, [FromQuery(Name = "RangoId")] int id) =>
{
    return await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == id);
});

/*
Model Binding

FromQuery - Parâmetros na URL/Query String
app.MapGet("/rango", (RangoDbContext rangoDbContext, [FromQuery] int id) =>
{
    return rangoDbContext.Rangos.FirstOrDefault(x => x.Id == id);
});

FromHeader - Cabeçalhos HTTP
app.MapGet("/rango", (RangoDbContext rangoDbContext, [FromHeader(Name = "RangoId")] int id) =>
{
    return rangoDbContext.Rangos.FirstOrDefault(x => x.Id == id);
});

*/

app.Run();
