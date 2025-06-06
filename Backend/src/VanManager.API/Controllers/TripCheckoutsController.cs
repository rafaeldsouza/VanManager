using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VanManager.Application.Common.Interfaces;
using VanManager.Application.TripCheckouts.Commands.CreateTripCheckout;
using VanManager.Application.TripCheckouts.Commands.DeleteTripCheckout;
using VanManager.Application.TripCheckouts.Commands.UpdateTripCheckout;
using VanManager.Application.TripCheckouts.Queries.GetTripCheckoutById;
using VanManager.Application.TripCheckouts.Queries.GetTripCheckouts;
using VanManager.Domain.Constants;
using VanManager.Domain.Entities;

namespace VanManager.API.Controllers;

[Authorize(Roles = $"{Roles.Driver}, {Roles.FleetOwner}")]
public class TripCheckoutsController : ApiControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<TripCheckoutsController> _logger;

    public TripCheckoutsController(
        ICurrentUserService currentUserService,
        ILogger<TripCheckoutsController> logger)
    {
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// Lists all trip checkouts
    /// </summary>
    /// <remarks>
    /// Returns a list of all trip checkouts in the system
    /// </remarks>
    /// <response code="200">List of trip checkouts returned successfully</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Access denied</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<TripCheckout>>> GetTripCheckouts()
    {
        var tripCheckouts = await Mediator.Send(new GetTripCheckoutsQuery());
        return Ok(tripCheckouts);
    }

    /// <summary>
    /// Gets a specific trip checkout
    /// </summary>
    /// <param name="id">Trip checkout ID</param>
    /// <remarks>
    /// Returns the details of a specific trip checkout
    /// </remarks>
    /// <response code="200">Trip checkout found successfully</response>
    /// <response code="404">Trip checkout not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TripCheckout>> GetTripCheckout(Guid id)
    {
        var tripCheckout = await Mediator.Send(new GetTripCheckoutByIdQuery(id));
        return Ok(tripCheckout);
    }

    /// <summary>
    /// Creates a new trip checkout
    /// </summary>
    /// <remarks>
    /// Example request:
    /// 
    ///     POST /api/tripcheckouts
    ///     {
    ///         "studentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "responsibleId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "routeId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "timestamp": "2023-12-20T10:00:00Z"
    ///     }
    /// </remarks>
    /// <response code="201">Trip checkout created successfully</response>
    /// <response code="400">Invalid data</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TripCheckout>> CreateTripCheckout([FromBody] CreateTripCheckoutCommand command)
    {
        var tripCheckout = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetTripCheckout), new { id = tripCheckout.Id }, tripCheckout);
    }

    /// <summary>
    /// Updates an existing trip checkout
    /// </summary>
    /// <param name="id">Trip checkout ID</param>
    /// <remarks>
    /// Example request:
    /// 
    ///     PUT /api/tripcheckouts/{id}
    ///     {
    ///         "studentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "responsibleId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "routeId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "timestamp": "2023-12-20T10:00:00Z"
    ///     }
    /// </remarks>
    /// <response code="204">Trip checkout updated successfully</response>
    /// <response code="400">Invalid data</response>
    /// <response code="404">Trip checkout not found</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTripCheckout(Guid id, [FromBody] UpdateTripCheckoutCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(new { message = "Trip checkout ID does not match" });
        }

        await Mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Removes a trip checkout
    /// </summary>
    /// <param name="id">Trip checkout ID</param>
    /// <response code="204">Trip checkout removed successfully</response>
    /// <response code="404">Trip checkout not found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTripCheckout(Guid id)
    {
        await Mediator.Send(new DeleteTripCheckoutCommand(id));
        return NoContent();
    }
}