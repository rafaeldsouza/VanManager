using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VanManager.Application.Common.Interfaces;
using VanManager.Application.Plans.Commands.CreatePlan;
using VanManager.Application.Plans.Commands.DeletePlan;
using VanManager.Application.Plans.Commands.UpdatePlan;
using VanManager.Application.Plans.Queries.GetPlanById;
using VanManager.Application.Plans.Queries.GetPlans;
using VanManager.Domain.Constants;
using VanManager.Domain.Entities;

namespace VanManager.API.Controllers;

[Authorize(Roles = Roles.Admin)]
public class PlansController : ApiControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<PlansController> _logger;

    public PlansController(
        ICurrentUserService currentUserService,
        ILogger<PlansController> logger)
    {
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// Lists all plans
    /// </summary>
    /// <remarks>
    /// Returns a list of all subscription plans in the system
    /// </remarks>
    /// <response code="200">List of plans returned successfully</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Access denied</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Plan>>> GetPlans()
    {
        var plans = await Mediator.Send(new GetPlansQuery());
        return Ok(plans);
    }

    /// <summary>
    /// Gets a specific plan
    /// </summary>
    /// <param name="id">Plan ID</param>
    /// <remarks>
    /// Returns the details of a specific subscription plan
    /// </remarks>
    /// <response code="200">Plan found successfully</response>
    /// <response code="404">Plan not found</response>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Plan>> GetPlan(Guid id)
    {
        var plan = await Mediator.Send(new GetPlanByIdQuery(id));
        return Ok(plan);
    }

    /// <summary>
    /// Creates a new plan
    /// </summary>
    /// <remarks>
    /// Example request:
    /// 
    ///     POST /api/plans
    ///     {
    ///         "name": "Basic Plan",
    ///         "price": 99.90,
    ///         "maxVans": 1,
    ///         "active": true,
    ///         "visible": true
    ///     }
    /// </remarks>
    /// <response code="201">Plan created successfully</response>
    /// <response code="400">Invalid data</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Plan>> CreatePlan([FromBody] CreatePlanCommand command)
    {
        var plan = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetPlan), new { id = plan.Id }, plan);
    }

    /// <summary>
    /// Updates an existing plan
    /// </summary>
    /// <param name="id">Plan ID</param>
    /// <remarks>
    /// Example request:
    /// 
    ///     PUT /api/plans/{id}
    ///     {
    ///         "name": "Updated Basic Plan",
    ///         "price": 129.90,
    ///         "maxVans": 2,
    ///         "active": true,
    ///         "visible": true
    ///     }
    /// </remarks>
    /// <response code="204">Plan updated successfully</response>
    /// <response code="400">Invalid data</response>
    /// <response code="404">Plan not found</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePlan(Guid id, [FromBody] UpdatePlanCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(new { message = "Plan ID does not match" });
        }

        await Mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Removes a plan
    /// </summary>
    /// <param name="id">Plan ID</param>
    /// <response code="204">Plan removed successfully</response>
    /// <response code="404">Plan not found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePlan(Guid id)
    {
        await Mediator.Send(new DeletePlanCommand(id));
        return NoContent();
    }
}