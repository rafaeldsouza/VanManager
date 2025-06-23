using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VanManager.Application.Common.Interfaces;
using VanManager.Application.Fleets.Commands.CreateFleet;
using VanManager.Application.Fleets.Commands.DeleteFleet;
using VanManager.Application.Fleets.Commands.UpdateFleet;
using VanManager.Application.Fleets.Queries.GetFleetByDriverId;
using VanManager.Application.Fleets.Queries.GetFleetById;
using VanManager.Application.Fleets.Queries.GetFleets;
using VanManager.Application.Fleets.Queries.GetFleetsByOwnerId;
using VanManager.Domain.Constants;
using VanManager.Domain.Entities;

namespace VanManager.API.Controllers;

[Authorize]
public class FleetsController : ApiControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<FleetsController> _logger;

    public FleetsController(
        ICurrentUserService currentUserService,
        ILogger<FleetsController> logger)
    {
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// Lista todas as frotas
    /// </summary>
    /// <remarks>
    /// Retorna uma lista de todas as frotas cadastradas no sistema
    /// </remarks>
    /// <response code="200">Lista de frotas retornada com sucesso</response>
    /// <response code="401">Não autorizado</response>
    /// <response code="403">Acesso negado</response>
    [HttpGet]
    [Authorize(Policy = Permissions.ViewFleets)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<Fleet>>> GetFleets()
    {
        var userId = _currentUserService.UserId;
        var userRole = _currentUserService.GetRoles();
        if (userId == null)
        {
            _logger.LogWarning("Usuário não autenticado tentou acessar frotas");
            return Unauthorized(); // Usuário não autenticado
        }
        if (userRole.Contains("Parent") || userRole.Contains("Student"))
        {
            // Usuários do tipo Parent ou Student não podem ver frotas
            _logger.LogWarning("Usuário {UserId} com papel {UserRole} tentou acessar frotas", userId, userRole);
            return Forbid(); // Não pode ver nada
        }

        if (userRole.Contains("Driver"))
        {
            var fleets = await Mediator.Send(new GetFleetByDriverIdQuery(userId.Value));
            return Ok(fleets);
        }

        if (userRole.Contains("FleetOwner"))
        {
            var fleets = await Mediator.Send(new GetFleetsByOwnerIdQuery(userId.Value));
            return Ok(fleets);
        }

        // Admin ou outros tipos: retorna tudo
        var allFleets = await Mediator.Send(new GetFleetsQuery());
        return Ok(allFleets);
    }

    /// <summary>
    /// Obtém uma frota específica
    /// </summary>
    /// <param name="id">ID da frota</param>
    /// <remarks>
    /// Retorna os detalhes de uma frota específica
    /// </remarks>
    /// <response code="200">Frota encontrada com sucesso</response>
    /// <response code="404">Frota não encontrada</response>
    [HttpGet("{id}")]
    [Authorize(Policy = Permissions.ViewFleets)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Fleet>> GetFleet(Guid id)
    {
        var userId = _currentUserService.UserId;
        var userRole = _currentUserService.GetRoles();
        if (userId == null)
        {
            _logger.LogWarning("Usuário não autenticado tentou acessar frotas");
            return Unauthorized();
        }

        if (!userRole.Contains("Admin") && !userRole.Contains("FleetOwner") && !userRole.Contains("Driver"))
        {
            _logger.LogWarning("Usuário {UserId} com papel {UserRole} tentou acessar frota {FleetId}", userId, userRole, id);
            return Forbid();
        }

        var fleet = await Mediator.Send(new GetFleetByIdQuery(id));
        
        if (fleet == null)
        {
            return NotFound();
        }

        // Verifica se o usuário tem permissão para ver a frota
        if (userRole.Contains("FleetOwner") && fleet.OwnerUserId != userId)
        {
            _logger.LogWarning("FleetOwner {UserId} tentou acessar frota {FleetId} que não é sua", userId, id);
            return Forbid();
        }

        if (userRole.Contains("Driver"))
        {
            var driverFleet = await Mediator.Send(new GetFleetByDriverIdQuery(userId.Value));
            if (!driverFleet.Any(f => f.Id == id))
            {
                _logger.LogWarning("Driver {UserId} tentou acessar frota {FleetId} que não está associada a ele", userId, id);
                return Forbid();
            }
        }

        return Ok(fleet);
    }

    /// <summary>
    /// Cria uma nova frota
    /// </summary>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     POST /api/fleets
    ///     {
    ///         "name": "Frota Escolar ABC",
    ///         "ownerUserId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    /// <response code="201">Frota criada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    [HttpPost]
    [Authorize(Policy = Permissions.CreateFleets)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Fleet>> CreateFleet([FromBody] CreateFleetCommand command)
    {
        var userId = _currentUserService.UserId;
        var userRole = _currentUserService.GetRoles();
        
        if (userId == null)
        {
            _logger.LogWarning("Usuário não autenticado tentou criar frota");
            return Unauthorized();
        }

        if (!userRole.Contains("FleetOwner"))
        {
            _logger.LogWarning("Usuário {UserId} com papel {UserRole} tentou criar frota", userId, userRole);
            return Forbid();
        }

        // Garante que o FleetOwner só pode criar frotas para si mesmo
        if (command.OwnerUserId != userId)
        {
            _logger.LogWarning("FleetOwner {UserId} tentou criar frota para outro usuário {OwnerUserId}", userId, command.OwnerUserId);
            return Forbid();
        }

        var fleet = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetFleet), new { id = fleet.Id }, fleet);
    }

    /// <summary>
    /// Atualiza uma frota existente
    /// </summary>
    /// <param name="id">ID da frota</param>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     PUT /api/fleets/{id}
    ///     {
    ///         "name": "Frota Escolar ABC Atualizada",
    ///         "ownerUserId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    /// <response code="204">Frota atualizada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Frota não encontrada</response>
    [HttpPut("{id}")]
    [Authorize(Policy = Permissions.EditFleets)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateFleet(Guid id, [FromBody] UpdateFleetCommand command)
    {
        var userId = _currentUserService.UserId;
        var userRole = _currentUserService.GetRoles();
        
        if (userId == null)
        {
            _logger.LogWarning("Usuário não autenticado tentou atualizar frota");
            return Unauthorized();
        }

        if (!userRole.Contains("FleetOwner"))
        {
            _logger.LogWarning("Usuário {UserId} com papel {UserRole} tentou atualizar frota {FleetId}", userId, userRole, id);
            return Forbid();
        }

        if (id != command.Id)
        {
            return BadRequest(new { message = "ID da frota não corresponde" });
        }

        // Verifica se a frota existe e pertence ao FleetOwner
        var fleet = await Mediator.Send(new GetFleetByIdQuery(id));
        if (fleet == null)
        {
            return NotFound();
        }

        if (fleet.OwnerUserId != userId)
        {
            _logger.LogWarning("FleetOwner {UserId} tentou atualizar frota {FleetId} que não é sua", userId, id);
            return Forbid();
        }

        // Garante que o FleetOwner não pode mudar o dono da frota
        if (command.OwnerUserId != userId)
        {
            _logger.LogWarning("FleetOwner {UserId} tentou mudar o dono da frota {FleetId}", userId, id);
            return Forbid();
        }

        await Mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Remove uma frota
    /// </summary>
    /// <param name="id">ID da frota</param>
    /// <response code="204">Frota removida com sucesso</response>
    /// <response code="404">Frota não encontrada</response>
    [HttpDelete("{id}")]
    [Authorize(Policy = Permissions.DeleteFleets)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteFleet(Guid id)
    {
        var userId = _currentUserService.UserId;
        var userRole = _currentUserService.GetRoles();
        
        if (userId == null)
        {
            _logger.LogWarning("Usuário não autenticado tentou excluir frota");
            return Unauthorized();
        }

        if (!userRole.Contains("FleetOwner"))
        {
            _logger.LogWarning("Usuário {UserId} com papel {UserRole} tentou excluir frota {FleetId}", userId, userRole, id);
            return Forbid();
        }

        // Verifica se a frota existe e pertence ao FleetOwner
        var fleet = await Mediator.Send(new GetFleetByIdQuery(id));
        if (fleet == null)
        {
            return NotFound();
        }

        if (fleet.OwnerUserId != userId)
        {
            _logger.LogWarning("FleetOwner {UserId} tentou excluir frota {FleetId} que não é sua", userId, id);
            return Forbid();
        }

        await Mediator.Send(new DeleteFleetCommand(id));
        return NoContent();
    }
}