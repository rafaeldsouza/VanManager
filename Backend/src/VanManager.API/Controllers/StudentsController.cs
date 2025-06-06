using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VanManager.Application.Common.Interfaces;
using VanManager.Application.Students.Commands.CreateStudent;
using VanManager.Application.Students.Commands.DeleteStudent;
using VanManager.Application.Students.Commands.UpdateStudent;
using VanManager.Application.Students.Queries.GetStudentById;
using VanManager.Application.Students.Queries.GetStudents;
using VanManager.Domain.Constants;
using VanManager.Domain.Entities;

namespace VanManager.API.Controllers;

[Authorize(Roles = $"{Roles.Driver}, {Roles.FleetOwner}")]
public class StudentsController : ApiControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<StudentsController> _logger;

    public StudentsController(
        ICurrentUserService currentUserService,
        ILogger<StudentsController> logger)
    {
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// Lista todos os estudantes
    /// </summary>
    /// <remarks>
    /// Retorna uma lista de todos os estudantes cadastrados no sistema
    /// </remarks>
    /// <response code="200">Lista de estudantes retornada com sucesso</response>
    /// <response code="401">Não autorizado</response>
    /// <response code="403">Acesso negado</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
    {
        var students = await Mediator.Send(new GetStudentsQuery());
        return Ok(students);
    }

    /// <summary>
    /// Obtém um estudante específico
    /// </summary>
    /// <param name="id">ID do estudante</param>
    /// <remarks>
    /// Retorna os detalhes de um estudante específico
    /// </remarks>
    /// <response code="200">Estudante encontrado com sucesso</response>
    /// <response code="404">Estudante não encontrado</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Student>> GetStudent(Guid id)
    {
        var student = await Mediator.Send(new GetStudentByIdQuery(id));
        return Ok(student);
    }

    /// <summary>
    /// Cria um novo estudante
    /// </summary>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     POST /api/students
    ///     {
    ///         "fullName": "João da Silva",
    ///         "dateOfBirth": "2010-01-01",
    ///         "schoolId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "parentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    /// <response code="201">Estudante criado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Student>> CreateStudent([FromBody] CreateStudentCommand command)
    {
        var student = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, student);
    }

    /// <summary>
    /// Atualiza um estudante existente
    /// </summary>
    /// <param name="id">ID do estudante</param>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     PUT /api/students/{id}
    ///     {
    ///         "fullName": "João da Silva",
    ///         "dateOfBirth": "2010-01-01",
    ///         "schoolId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "parentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    /// <response code="204">Estudante atualizado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Estudante não encontrado</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStudent(Guid id, [FromBody] UpdateStudentCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(new { message = "ID do estudante não corresponde" });
        }

        await Mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Remove um estudante
    /// </summary>
    /// <param name="id">ID do estudante</param>
    /// <response code="204">Estudante removido com sucesso</response>
    /// <response code="404">Estudante não encontrado</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteStudent(Guid id)
    {
        await Mediator.Send(new DeleteStudentCommand(id));
        return NoContent();
    }
}