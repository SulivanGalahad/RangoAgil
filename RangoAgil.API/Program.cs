using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using System.Xml.Linq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RangoDbContext>(
    o => o.UseSqlite(builder.Configuration["ConnectionStrings:RangoDbConStr"])
);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");


app.MapGet("/rangos", async (RangoDbContext rangoDbContext, [FromQuery(Name = "name")] string rangoNome) =>
{
    return await rangoDbContext.Rangos
                               .Where(x => x.Nome.Contains(rangoNome)).
                               ToListAsync();
});

app.MapGet("/rango/{id:int}", async (RangoDbContext rangoDbContext, int id) =>
{
    return await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == id);
});


app.Run();
