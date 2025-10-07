using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.Entities;
using RangoAgil.API.Models;

namespace RangoAgil.API.EndpointHandlers;

public static class IngredientesHandlers
{
    public static async Task<Results<NoContent, Ok<IEnumerable<IngredienteDTO>>>> GetIngredientesRangoAsync
        (RangoDbContext rangoDbContext,
        IMapper mapper,
        int rangoId) 
        {
            var ingredientesEntity = (await rangoDbContext.Rangos
                                           .Include(rango => rango.Ingredientes)
                                           .FirstOrDefaultAsync(rango => rango.Id == rangoId))?.Ingredientes;

            if (ingredientesEntity == null || ingredientesEntity.Count <= 0 )
            {
                return TypedResults.NoContent();
            }
            else
            {
                return TypedResults.Ok(mapper.Map<IEnumerable<IngredienteDTO>>(ingredientesEntity));
            }
       
        }
}
