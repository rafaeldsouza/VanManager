using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VanManager.Application.Common.Interfaces;
using VanManager.Application.AuthorizedGuardians.Commands.CreateAuthorizedGuardian;
using VanManager.Application.AuthorizedGuardians.Commands.DeleteAuthorizedGuardian;
using VanManager.Application.AuthorizedGuardians.Commands.UpdateAuthorizedGuardian;
using VanManager.Application.AuthorizedGuardians.Queries.GetAuthorizedGuardianById;
using VanManager.Application.AuthorizedGuardians.Queries.GetAuthorizedGuardians;
using VanManager.Domain.Constants;
using VanManager.Domain.Entities;

namespace VanManager.API.Controllers;

[Authorize(Roles = $"{Roles.Driver}, {Roles.FleetOwner}, {Roles.Admin}")]
public class AuthorizedGuardiansController : ApiControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<AuthorizedGuardiansController> _logger;

    public AuthorizedGuardiansController(
        ICurrentUserService currentUserService,
        ILogger<AuthorizedGuardiansController> logger)
    {
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// Lists all authorized guardians
    /// </summary>
    /// <remarks>
    /// Returns a list of all authorized guardians in the system
    /// </remarks>
    /// <response code="200">List of authorized guardians returned successfully</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Access denied</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<AuthorizedGuardian>>> GetAuthorizedGuardians(Guid studentId)
    {
        var authorizedGuardians = await Mediator.Send(new GetAuthorizedGuardiansQuery(studentId));
        return Ok(authorizedGuardians);
    }

    /// <summary>
    /// Gets a specific authorized guardian
    /// </summary>
    /// <param name="id">Authorized guardian ID</param>
    /// <remarks>
    /// Returns the details of a specific authorized guardian
    /// </remarks>
    /// <response code="200">Authorized guardian found successfully</response>
    /// <response code="404">Authorized guardian not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AuthorizedGuardian>> GetAuthorizedGuardian(Guid id)
    {
        var authorizedGuardian = await Mediator.Send(new GetAuthorizedGuardianByIdQuery(id));
        return Ok(authorizedGuardian);
    }

    /// <summary>
    /// Creates a new authorized guardian
    /// </summary>
    /// <remarks>
    /// Example request:
    /// 
    ///     POST /api/authorizedguardians
    ///     {
    ///         "studentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "guardianId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "relationship": "Parent"
    ///     }
    /// </remarks>
    /// <response code="201">Authorized guardian created successfully</response>
    /// <response code="400">Invalid data</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthorizedGuardian>> CreateAuthorizedGuardian([FromBody] CreateAuthorizedGuardianCommand command)
    {
        var authorizedGuardian = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetAuthorizedGuardian), new { id = authorizedGuardian.Id }, authorizedGuardian);
    }

    /// <summary>
    /// Updates an existing authorized guardian
    /// </summary>
    /// <param name="id">Authorized guardian ID</param>
    /// <remarks>
    /// Example request:
    /// 
    ///     PUT /api/authorizedguardians/{id}
    ///     {
    ///         "studentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "guardianId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "relationship": "Parent"
    ///     }
    /// </remarks>
    /// <response code="204">Authorized guardian updated successfully</response>
    /// <response code="400">Invalid data</response>
    /// <response code="404">Authorized guardian not found</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAuthorizedGuardian(Guid id, [FromBody] UpdateAuthorizedGuardianCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(new { message = "Authorized guardian ID does not match" });
        }

        await Mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Removes an authorized guardian
    /// </summary>
    /// <param name="id">Authorized guardian ID</param>
    /// <response code="204">Authorized guardian removed successfully</response>
    /// <response code="404">Authorized guardian not found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAuthorizedGuardian(Guid id)
    {
        await Mediator.Send(new DeleteAuthorizedGuardianCommand(id));
        return NoContent();
    }
}