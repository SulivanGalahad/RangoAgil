using RangoAgil.API.EndpointHandlers;

namespace RangoAgil.API.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static void RegisterRangosEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var rangosEndpoints = endpointRouteBuilder.MapGroup("/rangos");
        var rangosWithIdEndpoints = rangosEndpoints.MapGroup("/{rangoId:int}");

        rangosEndpoints.MapGet("", RangosHandlers.GetRangosAsync);
        rangosWithIdEndpoints.MapGet("", RangosHandlers.GetRangosWithIdAsync).WithName("GetRangos");
        rangosEndpoints.MapPost("", RangosHandlers.PostRangosAsync);
        rangosWithIdEndpoints.MapPut("", RangosHandlers.PutRangosAsync);
        rangosWithIdEndpoints.MapDelete("", RangosHandlers.DeleteRangosAsync);
    }

    public static void RegisterIngredientesEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    { 
        var ingredientesRangoEndpoints = endpointRouteBuilder.MapGroup("/rangos/{rangoId:int}/ingredientes");
        ingredientesRangoEndpoints.MapGet("", IngredientesHandlers.GetIngredientesRangoAsync);
    }
}