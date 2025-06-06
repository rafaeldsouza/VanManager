using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.StudentAbsences.Commands.CreateStudentAbsence;

public record CreateStudentAbsenceCommand : IRequest<StudentAbsence>
{
    public Guid StudentId { get; init; }
    public Guid RouteId { get; init; }
    public DateTime Date { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Reason { get; init; } = string.Empty;
}

public class CreateStudentAbsenceCommandHandler : IRequestHandler<CreateStudentAbsenceCommand, StudentAbsence>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CreateStudentAbsenceCommandHandler> _logger;

    public CreateStudentAbsenceCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<CreateStudentAbsenceCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<StudentAbsence> Handle(CreateStudentAbsenceCommand request, CancellationToken cancellationToken)
    {
        var studentAbsence = new StudentAbsence
        {
            Id = Guid.NewGuid(),
            StudentId = request.StudentId,
            RouteId = request.RouteId,
            Date = request.Date,
            Type = request.Type,
            Reason = request.Reason
        };

        await _unitOfWork.Repository<StudentAbsence>().AddAsync(studentAbsence);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Ausência do estudante {StudentAbsenceId} criada por {UserId}", studentAbsence.Id, _currentUserService.UserId);

        return studentAbsence;
    }
}