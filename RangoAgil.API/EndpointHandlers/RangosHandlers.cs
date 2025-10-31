using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RangoAgil.API.DbContexts;
using RangoAgil.API.Entities;
using RangoAgil.API.Models;

namespace RangoAgil.API.EndpointHandlers;

public static class RangosHandlers
{
   public static async Task<Results<NoContent, Ok<IEnumerable<RangoDTO>>>> GetRangosAsync
        (RangoDbContext rangoDbContext,
        IMapper mapper,
        ILogger<RangoDTO> logger,
        [FromQuery(Name = "name")] string? rangoNome) 
        {
            var rangosEntity = await rangoDbContext.Rangos
                                       .Where(x => rangoNome == null || x.Nome.ToLower().Contains(rangoNome.ToLower()))
                                       .ToListAsync();

            if (rangosEntity.Count <= 0 || rangosEntity == null)
            {
                logger.LogInformation($"GetRangosAsync registro não encontrado. Parametro: {rangoNome}");
                return TypedResults.NoContent();
            }
            else
            {
                logger.LogInformation("GetRangosAsync executado com sucesso!");
                return TypedResults.Ok(mapper.Map<IEnumerable<RangoDTO>>(rangosEntity));
            }
        }


    public static async Task<Results<NoContent, Ok<RangoDTO>>> GetRangosWithIdAsync
        (RangoDbContext rangoDbContext,
        IMapper mapper,
        ILogger<RangoDTO> logger,
        int rangoId)
        {
            var rangosEntity = await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == rangoId);

            if (rangosEntity == null) 
            {
                logger.LogInformation($"GetRangosWithIdAsync registro não encontrado. Parametro: {rangoId}");
                return TypedResults.NoContent(); 
            }
            else
            {
                logger.LogInformation("GetRangosWithIdAsync executado com sucesso!");
                return TypedResults.Ok(mapper.Map<RangoDTO>(rangosEntity));
            }
            
        }

    public static async Task<CreatedAtRoute<RangoDTO>> PostRangosAsync
        (RangoDbContext rangoDbContext,
        IMapper mapper,
         ILogger<RangoDTO> logger,
        [FromBody] RangoForCreateDTO rangoForCreate)
        {
            var rangoEntity = mapper.Map<Rango>(rangoForCreate);
             rangoDbContext.Add(rangoEntity);
            await rangoDbContext.SaveChangesAsync();

            var rangoToReturn = mapper.Map<RangoDTO>(rangoEntity);

            logger.LogInformation($"PostRangosAsync executado com sucesso! Id: {rangoToReturn.Id}");

            return TypedResults.CreatedAtRoute(
                rangoToReturn,
                "GetRangos", 
                new { rangoId = rangoToReturn.Id });
        }


    public static async Task<Results<NotFound, Ok>> PutRangosAsync
        (RangoDbContext rangoDbContext,
        IMapper mapper,
        ILogger<RangoDTO> logger,
        int rangoId,
        [FromBody] RangoForUpdateDTO rangoForUpdateDTO)
        {
            var rangoEntity = await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == rangoId);

            if (rangoEntity == null)
            {
                logger.LogInformation($"PutRangosAsync registro não encontrado. Parametro: {rangoId}");
                return TypedResults.NotFound();
            }

            mapper.Map(rangoForUpdateDTO, rangoEntity);

            await rangoDbContext.SaveChangesAsync();

            logger.LogInformation($"PutRangosAsync registro alterado com sucesso! Parametro: {rangoId}");

            return TypedResults.Ok();

        }

    public static async Task<Results<NotFound, NoContent>> DeleteRangosAsync
        (RangoDbContext rangoDbContext,
         ILogger<RangoDTO> logger,
        int rangoId) 
        {
            var rangoEntity = await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == rangoId);

            if (rangoEntity == null)
            {
                logger.LogInformation($"DeleteRangosAsync registro não encontrado. Parametro: {rangoId}");
                return TypedResults.NotFound();
            }

            rangoDbContext.Rangos.Remove(rangoEntity);
   
            await rangoDbContext.SaveChangesAsync();

            logger.LogInformation($"DeleteRangosAsync registro excluído com sucesso! Parametro: {rangoId}");

            return TypedResults.NoContent();

        }

}
