using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.Entities;
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


    public static async Task<Results<NoContent, Ok<RangoDTO>>> GetRangosWithIdAsync
        (RangoDbContext rangoDbContext,
        IMapper mapper,
        int rangoId)
        {
            var rangosEntity = await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == rangoId);

            if (rangosEntity == null) 
            { 
                return TypedResults.NoContent(); 
            }
            else
            {
                return TypedResults.Ok(mapper.Map<RangoDTO>(rangosEntity));
            }
            
        }

    public static async Task<CreatedAtRoute<RangoDTO>> PostRangosAsync
        (RangoDbContext rangoDbContext,
        IMapper mapper,
        [FromBody] RangoForCreateDTO rangoForCreate)
        {
            var rangoEntity = mapper.Map<Rango>(rangoForCreate);
             rangoDbContext.Add(rangoEntity);
            await rangoDbContext.SaveChangesAsync();

            var rangoToReturn = mapper.Map<RangoDTO>(rangoEntity);

            return TypedResults.CreatedAtRoute(
                rangoToReturn,
                "GetRangos", 
                new { rangoId = rangoToReturn.Id });
        }


    public static async Task<Results<NotFound, Ok>> PutRangosAsync
        (RangoDbContext rangoDbContext,
        IMapper mapper,
        int rangoId,
        [FromBody] RangoForUpdateDTO rangoForUpdateDTO)
        {
            var rangoEntity = await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == rangoId);

            if (rangoEntity == null)
            {
                return TypedResults.NotFound();
            }

            mapper.Map(rangoForUpdateDTO, rangoEntity);

            await rangoDbContext.SaveChangesAsync();

            return TypedResults.Ok();

        }

    public static async Task<Results<NotFound, NoContent>> DeleteRangosAsync
        (RangoDbContext rangoDbContext,
        int rangoId) 
        {
            var rangoEntity = await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == rangoId);

            if (rangoEntity == null)
            {
                return TypedResults.NotFound();
            }

            rangoDbContext.Rangos.Remove(rangoEntity);
   
            await rangoDbContext.SaveChangesAsync();

            return TypedResults.NoContent();

        }

}
