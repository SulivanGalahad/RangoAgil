using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.Models;

namespace RangoAgil.API.EndpointHandlers;

public static class RangosHandlers
{
   public static async Task<Results<NoContent, Ok<IEnumerable<RangoDTO>>>> GetRangosAsync
    (RangoDbContext rangoDbContext,
    IMapper mapper,
    [FromQuery(Name = "name")] string? rangoNome) 
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
    }



}
