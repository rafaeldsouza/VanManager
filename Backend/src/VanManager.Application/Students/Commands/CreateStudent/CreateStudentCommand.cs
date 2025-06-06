using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.Students.Commands.CreateStudent;

public record CreateStudentCommand : IRequest<Student>
{
    public string FullName { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; }
    public Guid SchoolId { get; init; }
    public Guid ParentId { get; init; }
}

public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, Student>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CreateStudentCommandHandler> _logger;

    public CreateStudentCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<CreateStudentCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Student> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
    {
        var student = new Student
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            DateOfBirth = request.DateOfBirth,
            SchoolId = request.SchoolId,
            ParentId = request.ParentId
        };

        await _unitOfWork.Repository<Student>().AddAsync(student);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Estudante {StudentId} criado por {UserId}", student.Id, _currentUserService.UserId);

        return student;
    }
}