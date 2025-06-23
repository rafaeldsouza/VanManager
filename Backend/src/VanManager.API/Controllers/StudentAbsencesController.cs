using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VanManager.Application.Common.Interfaces;
using VanManager.Application.StudentAbsences.Commands.ApproveAbsenceJustification;
using VanManager.Application.StudentAbsences.Commands.CreateStudentAbsence;
using VanManager.Application.StudentAbsences.Commands.DeleteStudentAbsence;
using VanManager.Application.StudentAbsences.Commands.UpdateStudentAbsence;
using VanManager.Application.StudentAbsences.Queries.GetStudentAbsenceById;
using VanManager.Application.StudentAbsences.Queries.GetStudentAbsences;

using VanManager.Domain.Entities;

namespace VanManager.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StudentAbsencesController : ApiControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<StudentAbsencesController> _logger;

    public StudentAbsencesController(
        ICurrentUserService currentUserService,
        ILogger<StudentAbsencesController> logger)
    {
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// Lista todas as ausências de estudantes
    /// </summary>
    /// <remarks>
    /// Retorna uma lista de todas as ausências de estudantes cadastradas no sistema
    /// </remarks>
    /// <response code="200">Lista de ausências retornada com sucesso</response>
    /// <response code="401">Não autorizado</response>
    /// <response code="403">Acesso negado</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<StudentAbsence>>> GetStudentAbsences()
    {
        var absences = await Mediator.Send(new GetStudentAbsencesQuery());
        return Ok(absences);
    }

    /// <summary>
    /// Obtém uma ausência específica
    /// </summary>
    /// <param name="id">ID da ausência</param>
    /// <remarks>
    /// Retorna os detalhes de uma ausência específica
    /// </remarks>
    /// <response code="200">Ausência encontrada com sucesso</response>
    /// <response code="404">Ausência não encontrada</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentAbsence>> GetStudentAbsence(Guid id)
    {
        var absence = await Mediator.Send(new GetStudentAbsenceByIdQuery(id));
        return Ok(absence);
    }

    /// <summary>
    /// Registra uma nova ausência
    /// </summary>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     POST /api/studentabsences
    ///     {
    ///         "studentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "routeId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "date": "2023-12-20",
    ///         "type": "IDA",
    ///         "reason": "Consulta médica"
    ///     }
    /// </remarks>
    /// <response code="201">Ausência registrada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> Create(CreateStudentAbsenceCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// Atualiza uma ausência existente
    /// </summary>
    /// <param name="id">ID da ausência</param>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     PUT /api/studentabsences/{id}
    ///     {
    ///         "studentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "routeId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "date": "2023-12-20",
    ///         "type": "IDA",
    ///         "reason": "Consulta médica"
    ///     }
    /// </remarks>
    /// <response code="204">Ausência atualizada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Ausência não encontrada</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, UpdateStudentAbsenceCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        await Mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Remove uma ausência
    /// </summary>
    /// <param name="id">ID da ausência</param>
    /// <response code="204">Ausência removida com sucesso</response>
    /// <response code="404">Ausência não encontrada</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin,FleetOwner")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteStudentAbsenceCommand(id));
        return NoContent();
    }

    /// <summary>
    /// Aprova uma justificativa de ausência
    /// </summary>
    [HttpPut("{id}/approve-justification")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [Authorize(Roles = "Admin,FleetOwner")]
    public async Task<IActionResult> ApproveJustification(Guid id)
    {
        await Mediator.Send(new ApproveAbsenceJustificationCommand(id));
        return NoContent();
    }

    /// <summary>
    /// Gets all absence records for a student
    /// </summary>
    [HttpGet("student/{studentId}")]
    public async Task<ActionResult<IList<StudentAbsence>>> GetByStudentId(Guid studentId)
    {
        var result =  await Mediator.Send(new GetStudentAbsenceByIdQuery(studentId));
        return Ok(result);
    }
}