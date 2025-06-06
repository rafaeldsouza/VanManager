using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VanManager.Application.Common.Interfaces;
using VanManager.Application.Routes.Commands.CreateRoute;
using VanManager.Application.Routes.Commands.DeleteRoute;
using VanManager.Application.Routes.Commands.UpdateRoute;
using VanManager.Application.Routes.Queries.GetRouteById;
using VanManager.Application.Routes.Queries.GetRoutes;
using VanManager.Domain.Constants;
using VanManager.Domain.Entities;

namespace VanManager.API.Controllers;

[Authorize(Roles = $"{Roles.Driver}, {Roles.FleetOwner}")]
public class RoutesController : ApiControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<RoutesController> _logger;

    public RoutesController(
        ICurrentUserService currentUserService,
        ILogger<RoutesController> logger)
    {
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// Lists all routes
    /// </summary>
    /// <remarks>
    /// Returns a list of all routes in the system
    /// </remarks>
    /// <response code="200">List of routes returned successfully</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Access denied</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<Domain.Entities.Route>>> GetRoutes()
    {
        var routes = await Mediator.Send(new GetRoutesQuery());
        return Ok(routes);
    }

    /// <summary>
    /// Gets a specific route
    /// </summary>
    /// <param name="id">Route ID</param>
    /// <remarks>
    /// Returns the details of a specific route
    /// </remarks>
    /// <response code="200">Route found successfully</response>
    /// <response code="404">Route not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Domain.Entities.Route>> GetRoute(Guid id)
    {
        var route = await Mediator.Send(new GetRouteByIdQuery(id));
        return Ok(route);
    }

    /// <summary>
    /// Creates a new route
    /// </summary>
    /// <remarks>
    /// Example request:
    /// 
    ///     POST /api/routes
    ///     {
    ///         "vanId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "description": "Morning School Route"
    ///     }
    /// </remarks>
    /// <response code="201">Route created successfully</response>
    /// <response code="400">Invalid data</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Domain.Entities.Route>> CreateRoute([FromBody] CreateRouteCommand command)
    {
        var route = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetRoute), new { id = route.Id }, route);
    }

    /// <summary>
    /// Updates an existing route
    /// </summary>
    /// <param name="id">Route ID</param>
    /// <remarks>
    /// Example request:
    /// 
    ///     PUT /api/routes/{id}
    ///     {
    ///         "vanId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "description": "Updated Morning School Route"
    ///     }
    /// </remarks>
    /// <response code="204">Route updated successfully</response>
    /// <response code="400">Invalid data</response>
    /// <response code="404">Route not found</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRoute(Guid id, [FromBody] UpdateRouteCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(new { message = "Route ID does not match" });
        }

        await Mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Removes a route
    /// </summary>
    /// <param name="id">Route ID</param>
    /// <response code="204">Route removed successfully</response>
    /// <response code="404">Route not found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRoute(Guid id)
    {
        await Mediator.Send(new DeleteRouteCommand(id));
        return NoContent();
    }
}