using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;


namespace VanManager.Application.Students.Commands.UpdateStudent;

public record UpdateStudentCommand : IRequest
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; }
    public Guid SchoolId { get; init; }
    public Guid ParentId { get; init; }
}

public class UpdateStudentCommandHandler : IRequestHandler<UpdateStudentCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UpdateStudentCommandHandler> _logger;

    public UpdateStudentCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<UpdateStudentCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await _unitOfWork.Repository<Student>().GetByIdAsync(request.Id);

        if (student == null)
        {
            throw new NotFoundException("Estudante não encontrado");
        }

        student.FullName = request.FullName;
        student.DateOfBirth = request.DateOfBirth;
        student.SchoolId = request.SchoolId;
        student.ParentId = request.ParentId;

        await _unitOfWork.Repository<Student>().UpdateAsync(student);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Estudante {StudentId} atualizado por {UserId}", request.Id, _currentUserService.UserId);
    }
}