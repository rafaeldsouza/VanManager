using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VanManager.Application.Common.Interfaces;
using VanManager.Application.Vans.Commands.CreateVan;
using VanManager.Application.Vans.Commands.DeleteVan;
using VanManager.Application.Vans.Commands.UpdateVan;
using VanManager.Application.Vans.Queries.GetVanById;
using VanManager.Application.Vans.Queries.GetVans;
using VanManager.Domain.Constants;
using VanManager.Domain.Entities;

namespace VanManager.API.Controllers;

[Authorize]
public class VansController : ApiControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<VansController> _logger;

    public VansController(
        ICurrentUserService currentUserService,
        ILogger<VansController> logger)
    {
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// Lista todas as vans
    /// </summary>
    /// <remarks>
    /// Retorna uma lista de todas as vans cadastradas no sistema
    /// </remarks>
    /// <response code="200">Lista de vans retornada com sucesso</response>
    /// <response code="401">Não autorizado</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<Van>>> GetVans()
    {
        var vans = await Mediator.Send(new GetVansQuery());
        return Ok(vans);
    }

    /// <summary>
    /// Obtém uma van específica
    /// </summary>
    /// <param name="id">ID da van</param>
    /// <remarks>
    /// Retorna os detalhes de uma van específica
    /// </remarks>
    /// <response code="200">Van encontrada com sucesso</response>
    /// <response code="404">Van não encontrada</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Van>> GetVan(Guid id)
    {
        var van = await Mediator.Send(new GetVanByIdQuery(id));
        return Ok(van);
    }

    /// <summary>
    /// Cria uma nova van
    /// </summary>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     POST /api/vans
    ///     {
    ///         "plateNumber": "ABC1234",
    ///         "fleetId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "assignedDriverId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "capacity": 15
    ///     }
    /// </remarks>
    /// <response code="201">Van criada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Van>> CreateVan([FromBody] CreateVanCommand command)
    {
        var van = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetVan), new { id = van.Id }, van);
    }

    /// <summary>
    /// Atualiza uma van existente
    /// </summary>
    /// <param name="id">ID da van</param>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     PUT /api/vans/{id}
    ///     {
    ///         "plateNumber": "ABC1234",
    ///         "fleetId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "assignedDriverId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "capacity": 15
    ///     }
    /// </remarks>
    /// <response code="204">Van atualizada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Van não encontrada</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateVan(Guid id, [FromBody] UpdateVanCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(new { message = "ID da van não corresponde" });
        }

        await Mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Remove uma van
    /// </summary>
    /// <param name="id">ID da van</param>
    /// <response code="204">Van removida com sucesso</response>
    /// <response code="404">Van não encontrada</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteVan(Guid id)
    {
        await Mediator.Send(new DeleteVanCommand(id));
        return NoContent();
    }
}