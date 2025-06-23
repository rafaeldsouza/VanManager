using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using VanManager.Application.Common.Interfaces;
using VanManager.Application.StudentTripLogs.Commands.CreateStudentTripLog;
using VanManager.Application.StudentTripLogs.Commands.DeleteStudentTripLog;
using VanManager.Application.StudentTripLogs.Commands.UpdateStudentTripLog;
using VanManager.Application.StudentTripLogs.Queries.GetStudentTripLogById;
using VanManager.Application.StudentTripLogs.Queries.GetStudentTripLogsByStudentId;
using VanManager.Application.StudentTripLogs.Queries.GetStudentTripLogsByVanId;
using VanManager.Domain.Constants;
using VanManager.Domain.Entities;

namespace VanManager.API.Controllers;

[Authorize]
public class StudentTripLogsController : ApiControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<StudentTripLogsController> _logger;

    public StudentTripLogsController(
        ICurrentUserService currentUserService,
        ILogger<StudentTripLogsController> logger)
    {
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// Obtém um registro de viagem específico
    /// </summary>
    /// <param name="id">ID do registro de viagem</param>
    /// <response code="200">Registro de viagem encontrado com sucesso</response>
    /// <response code="404">Registro de viagem não encontrado</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentTripLog>> GetStudentTripLog(Guid id)
    {
        var userId = _currentUserService.UserId;
        var userRole = _currentUserService.GetRoles();
        
        if (userId == null)
        {
            _logger.LogWarning("Usuário não autenticado tentou acessar registro de viagem");
            return Unauthorized();
        }

        var studentTripLog = await Mediator.Send(new GetStudentTripLogByIdQuery(id));
        
        if (studentTripLog == null)
        {
            return NotFound();
        }

        // Verifica se o usuário tem permissão para ver o registro
        if (userRole.Contains("Parent") && studentTripLog.Student.ParentId != userId)
        {
            _logger.LogWarning("Parent {UserId} tentou acessar registro de viagem {TripLogId} que não é do seu estudante", 
                userId, id);
            return Forbid();
        }

        if (userRole.Contains("Driver"))
        {
            var vanTripLogs = await Mediator.Send(new GetStudentTripLogsByVanIdQuery(studentTripLog.VanId));
            if (!vanTripLogs.Any(x => x.Id == id))
            {
                _logger.LogWarning("Driver {UserId} tentou acessar registro de viagem {TripLogId} que não está associado à sua van", 
                    userId, id);
                return Forbid();
            }
        }

        return Ok(studentTripLog);
    }

    /// <summary>
    /// Obtém todos os registros de viagem de um estudante
    /// </summary>
    /// <param name="studentId">ID do estudante</param>
    /// <response code="200">Lista de registros de viagem retornada com sucesso</response>
    [HttpGet("student/{studentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<StudentTripLog>>> GetStudentTripLogsByStudentId(Guid studentId)
    {
        var userId = _currentUserService.UserId;
        var userRole = _currentUserService.GetRoles();
        
        if (userId == null)
        {
            _logger.LogWarning("Usuário não autenticado tentou acessar registros de viagem");
            return Unauthorized();
        }

        // Verifica se o usuário tem permissão para ver os registros
        if (userRole.Contains("Parent"))
        {
            var studentTripLogs = await Mediator.Send(new GetStudentTripLogsByStudentIdQuery(studentId));
            var firstLog = studentTripLogs.FirstOrDefault();
            if (firstLog != null && firstLog.Student.ParentId != userId)
            {
                _logger.LogWarning("Parent {UserId} tentou acessar registros de viagem do estudante {StudentId}", 
                    userId, studentId);
                return Forbid();
            }
        }

        var tripLogs = await Mediator.Send(new GetStudentTripLogsByStudentIdQuery(studentId));
        return Ok(tripLogs);
    }

    /// <summary>
    /// Obtém todos os registros de viagem de uma van
    /// </summary>
    /// <param name="vanId">ID da van</param>
    /// <response code="200">Lista de registros de viagem retornada com sucesso</response>
    [HttpGet("van/{vanId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<StudentTripLog>>> GetStudentTripLogsByVanId(Guid vanId)
    {
        var userId = _currentUserService.UserId;
        var userRole = _currentUserService.GetRoles();
        
        if (userId == null)
        {
            _logger.LogWarning("Usuário não autenticado tentou acessar registros de viagem");
            return Unauthorized();
        }

        // Verifica se o usuário tem permissão para ver os registros
        if (userRole.Contains("Driver"))
        {
            var vanTripLogs = await Mediator.Send(new GetStudentTripLogsByVanIdQuery(vanId));
            var firstLog = vanTripLogs.FirstOrDefault();
            if (firstLog != null && firstLog.Van.DriverId != userId)
            {
                _logger.LogWarning("Driver {UserId} tentou acessar registros de viagem da van {VanId}", 
                    userId, vanId);
                return Forbid();
            }
        }

        var tripLogs = await Mediator.Send(new GetStudentTripLogsByVanIdQuery(vanId));
        return Ok(tripLogs);
    }

    /// <summary>
    /// Cria um novo registro de viagem
    /// </summary>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     POST /api/studenttriplogs
    ///     {
    ///         "studentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "routeId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "vanId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "boardingTime": "2024-03-20T08:00:00",
    ///         "boardingLatitude": -23.550520,
    ///         "boardingLongitude": -46.633308,
    ///         "boardingAddress": "Rua Exemplo, 123",
    ///         "dropoffLatitude": -23.550520,
    ///         "dropoffLongitude": -46.633308,
    ///         "dropoffAddress": "Rua Exemplo, 456",
    ///         "dropoffTime": "2024-03-20T08:30:00",
    ///         "notes": "Observações sobre a viagem",
    ///         "status": "Completed"
    ///     }
    /// </remarks>
    /// <response code="201">Registro de viagem criado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StudentTripLog>> CreateStudentTripLog([FromBody] CreateStudentTripLogCommand command)
    {
        var userId = _currentUserService.UserId;
        var userRole = _currentUserService.GetRoles();
        
        if (userId == null)
        {
            _logger.LogWarning("Usuário não autenticado tentou criar registro de viagem");
            return Unauthorized();
        }

        // Verifica se o usuário tem permissão para criar o registro
        if (userRole.Contains("Driver"))
        {
            var vanTripLogs = await Mediator.Send(new GetStudentTripLogsByVanIdQuery(command.VanId));
            var firstLog = vanTripLogs.FirstOrDefault();
            if (firstLog != null && firstLog.Van.DriverId != userId)
            {
                _logger.LogWarning("Driver {UserId} tentou criar registro de viagem para van {VanId}", 
                    userId, command.VanId);
                return Forbid();
            }
        }

        var studentTripLog = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetStudentTripLog), new { id = studentTripLog.Id }, studentTripLog);
    }

    /// <summary>
    /// Atualiza um registro de viagem existente
    /// </summary>
    /// <param name="id">ID do registro de viagem</param>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     PUT /api/studenttriplogs/{id}
    ///     {
    ///         "studentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "routeId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "vanId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "boardingTime": "2024-03-20T08:00:00",
    ///         "boardingLatitude": -23.550520,
    ///         "boardingLongitude": -46.633308,
    ///         "boardingAddress": "Rua Exemplo, 123",
    ///         "dropoffLatitude": -23.550520,
    ///         "dropoffLongitude": -46.633308,
    ///         "dropoffAddress": "Rua Exemplo, 456",
    ///         "dropoffTime": "2024-03-20T08:30:00",
    ///         "notes": "Observações sobre a viagem",
    ///         "status": "Completed"
    ///     }
    /// </remarks>
    /// <response code="204">Registro de viagem atualizado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Registro de viagem não encontrado</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStudentTripLog(Guid id, [FromBody] UpdateStudentTripLogCommand command)
    {
        var userId = _currentUserService.UserId;
        var userRole = _currentUserService.GetRoles();
        
        if (userId == null)
        {
            _logger.LogWarning("Usuário não autenticado tentou atualizar registro de viagem");
            return Unauthorized();
        }

        if (id != command.Id)
        {
            return BadRequest(new { message = "ID do registro de viagem não corresponde" });
        }

        // Verifica se o registro existe e se o usuário tem permissão para atualizá-lo
        var existingLog = await Mediator.Send(new GetStudentTripLogByIdQuery(id));
        if (existingLog == null)
        {
            return NotFound();
        }

        if (userRole.Contains("Driver") && existingLog.Van.DriverId != userId)
        {
            _logger.LogWarning("Driver {UserId} tentou atualizar registro de viagem {TripLogId} que não está associado à sua van", 
                userId, id);
            return Forbid();
        }

        await Mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Remove um registro de viagem
    /// </summary>
    /// <param name="id">ID do registro de viagem</param>
    /// <response code="204">Registro de viagem removido com sucesso</response>
    /// <response code="404">Registro de viagem não encontrado</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteStudentTripLog(Guid id)
    {
        var userId = _currentUserService.UserId;
        var userRole = _currentUserService.GetRoles();
        
        if (userId == null)
        {
            _logger.LogWarning("Usuário não autenticado tentou excluir registro de viagem");
            return Unauthorized();
        }

        // Verifica se o registro existe e se o usuário tem permissão para excluí-lo
        var existingLog = await Mediator.Send(new GetStudentTripLogByIdQuery(id));
        if (existingLog == null)
        {
            return NotFound();
        }

        if (userRole.Contains("Driver") && existingLog.Van.DriverId != userId)
        {
            _logger.LogWarning("Driver {UserId} tentou excluir registro de viagem {TripLogId} que não está associado à sua van", 
                userId, id);
            return Forbid();
        }

        await Mediator.Send(new DeleteStudentTripLogCommand(id));
        return NoContent();
    }
} 