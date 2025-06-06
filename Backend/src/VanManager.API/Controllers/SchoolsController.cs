using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VanManager.Application.Common.Interfaces;
using VanManager.Application.Schools.Commands.CreateSchool;
using VanManager.Application.Schools.Commands.DeleteSchool;
using VanManager.Application.Schools.Commands.UpdateSchool;
using VanManager.Application.Schools.Queries.GetSchoolById;
using VanManager.Application.Schools.Queries.GetSchools;
using VanManager.Domain.Constants;
using VanManager.Domain.Entities;

namespace VanManager.API.Controllers;

[Authorize(Roles = $"{Roles.Driver}, {Roles.FleetOwner}")]
public class SchoolsController : ApiControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<SchoolsController> _logger;

    public SchoolsController(
        ICurrentUserService currentUserService,
        ILogger<SchoolsController> logger)
    {
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// Lista todas as escolas
    /// </summary>
    /// <remarks>
    /// Retorna uma lista de todas as escolas cadastradas no sistema
    /// </remarks>
    /// <response code="200">Lista de escolas retornada com sucesso</response>
    /// <response code="401">Não autorizado</response>
    /// <response code="403">Acesso negado</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<School>>> GetSchools()
    {
        var schools = await Mediator.Send(new GetSchoolsQuery());
        return Ok(schools);
    }

    /// <summary>
    /// Obtém uma escola específica
    /// </summary>
    /// <param name="id">ID da escola</param>
    /// <remarks>
    /// Retorna os detalhes de uma escola específica
    /// </remarks>
    /// <response code="200">Escola encontrada com sucesso</response>
    /// <response code="404">Escola não encontrada</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<School>> GetSchool(Guid id)
    {
        var school = await Mediator.Send(new GetSchoolByIdQuery(id));
        return Ok(school);
    }

    /// <summary>
    /// Cria uma nova escola
    /// </summary>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     POST /api/schools
    ///     {
    ///         "name": "Escola Municipal João da Silva",
    ///         "address": "Rua das Flores, 123",
    ///         "locationLat": -23.550520,
    ///         "locationLng": -46.633308
    ///     }
    /// </remarks>
    /// <response code="201">Escola criada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<School>> CreateSchool([FromBody] CreateSchoolCommand command)
    {
        var school = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetSchool), new { id = school.Id }, school);
    }

    /// <summary>
    /// Atualiza uma escola existente
    /// </summary>
    /// <param name="id">ID da escola</param>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     PUT /api/schools/{id}
    ///     {
    ///         "name": "Escola Municipal João da Silva",
    ///         "address": "Rua das Flores, 123",
    ///         "locationLat": -23.550520,
    ///         "locationLng": -46.633308
    ///     }
    /// </remarks>
    /// <response code="204">Escola atualizada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Escola não encontrada</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSchool(Guid id, [FromBody] UpdateSchoolCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(new { message = "ID da escola não corresponde" });
        }

        await Mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Remove uma escola
    /// </summary>
    /// <param name="id">ID da escola</param>
    /// <response code="204">Escola removida com sucesso</response>
    /// <response code="404">Escola não encontrada</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSchool(Guid id)
    {
        await Mediator.Send(new DeleteSchoolCommand(id));
        return NoContent();
    }
}