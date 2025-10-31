using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;

using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;

using RangoAgil.API.Models;

namespace RangoAgil.API.EndpointHandlers;

public static class IngredientesHandlers
{
  /*   public static async Task<Results<NoContent, Ok<IEnumerable<IngredienteDTO>>>> GetIngredientesRangoAsync
        (RangoDbContext rangoDbContext,
        IMapper mapper,
        int rangoId)
    {
        var ingredientesEntity = (await rangoDbContext.Rangos
                                       .Include(rango => rango.Ingredientes)
                                       .FirstOrDefaultAsync(rango => rango.Id == rangoId))?.Ingredientes;

        if (ingredientesEntity == null || ingredientesEntity.Count <= 0)
        {
            return TypedResults.NoContent();
        }
        else
        {
            return TypedResults.Ok(mapper.Map<IEnumerable<IngredienteDTO>>(ingredientesEntity));
        }

    } */

    public static async Task<Results<NotFound, Ok<IEnumerable<IngredienteDTO>>>> GetIngredientesRangoAsync
        (RangoDbContext rangoDbContext,
        IMapper mapper,
        int rangoId) 
    {
        var rangosEntity = await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == rangoId);
        if (rangosEntity == null)
        {
            return TypedResults.NotFound();            
        }

        return TypedResults.Ok(mapper.Map<IEnumerable<IngredienteDTO>>((await rangoDbContext.Rangos
                                           .Include(rango => rango.Ingredientes)
                                           .FirstOrDefaultAsync(rango => rango.Id == rangoId))?.Ingredientes));
       
    }
}
