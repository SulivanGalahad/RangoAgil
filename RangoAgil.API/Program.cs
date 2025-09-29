using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RangoDbContext>(
    o => o.UseSqlite(builder.Configuration["ConnectionStrings:RangoDbConStr"])
);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/rangos", (RangoDbContext rangoDbContext) =>
{
    return rangoDbContext.Rangos;
});

app.MapGet("/rango/{id}", (RangoDbContext rangoDbContext, int id) =>
{
    return rangoDbContext.Rangos.FirstOrDefault(x => x.Id == id);
});

app.Run();
