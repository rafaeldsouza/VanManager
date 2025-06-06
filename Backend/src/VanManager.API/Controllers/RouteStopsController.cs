using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VanManager.Application.Common.Interfaces;
using VanManager.Application.RouteStops.Commands.CreateRouteStop;
using VanManager.Application.RouteStops.Commands.DeleteRouteStop;
using VanManager.Application.RouteStops.Commands.UpdateRouteStop;
using VanManager.Application.RouteStops.Queries.GetRouteStopById;
using VanManager.Application.RouteStops.Queries.GetRouteStops;
using VanManager.Domain.Constants;
using VanManager.Domain.Entities;

namespace VanManager.API.Controllers;

[Authorize(Roles = $"{Roles.Driver}, {Roles.FleetOwner}")]
public class RouteStopsController : ApiControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<RouteStopsController> _logger;

    public RouteStopsController(
        ICurrentUserService currentUserService,
        ILogger<RouteStopsController> logger)
    {
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// Lista todas as paradas de rota
    /// </summary>
    /// <remarks>
    /// Retorna uma lista de todas as paradas de rota cadastradas no sistema
    /// </remarks>
    /// <response code="200">Lista de paradas de rota retornada com sucesso</response>
    /// <response code="401">Não autorizado</response>
    /// <response code="403">Acesso negado</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<RouteStop>>> GetRouteStops()
    {
        var routeStops = await Mediator.Send(new GetRouteStopsQuery());
        return Ok(routeStops);
    }

    /// <summary>
    /// Obtém uma parada de rota específica
    /// </summary>
    /// <param name="id">ID da parada de rota</param>
    /// <remarks>
    /// Retorna os detalhes de uma parada de rota específica
    /// </remarks>
    /// <response code="200">Parada de rota encontrada com sucesso</response>
    /// <response code="404">Parada de rota não encontrada</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RouteStop>> GetRouteStop(Guid id)
    {
        var routeStop = await Mediator.Send(new GetRouteStopByIdQuery(id));
        return Ok(routeStop);
    }

    /// <summary>
    /// Cria uma nova parada de rota
    /// </summary>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     POST /api/routestops
    ///     {
    ///         "routeId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "studentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "timestamp": "2023-12-20T10:00:00Z",
    ///         "type": "EMBARQUE",
    ///         "locationLat": -23.550520,
    ///         "locationLng": -46.633308
    ///     }
    /// </remarks>
    /// <response code="201">Parada de rota criada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RouteStop>> CreateRouteStop([FromBody] CreateRouteStopCommand command)
    {
        var routeStop = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetRouteStop), new { id = routeStop.Id }, routeStop);
    }

    /// <summary>
    /// Atualiza uma parada de rota existente
    /// </summary>
    /// <param name="id">ID da parada de rota</param>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     PUT /api/routestops/{id}
    ///     {
    ///         "routeId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "studentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "timestamp": "2023-12-20T10:00:00Z",
    ///         "type": "EMBARQUE",
    ///         "locationLat": -23.550520,
    ///         "locationLng": -46.633308
    ///     }
    /// </remarks>
    /// <response code="204">Parada de rota atualizada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Parada de rota não encontrada</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRouteStop(Guid id, [FromBody] UpdateRouteStopCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(new { message = "ID da parada de rota não corresponde" });
        }

        await Mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Remove uma parada de rota
    /// </summary>
    /// <param name="id">ID da parada de rota</param>
    /// <response code="204">Parada de rota removida com sucesso</response>
    /// <response code="404">Parada de rota não encontrada</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRouteStop(Guid id)
    {
        await Mediator.Send(new DeleteRouteStopCommand(id));
        return NoContent();
    }
}