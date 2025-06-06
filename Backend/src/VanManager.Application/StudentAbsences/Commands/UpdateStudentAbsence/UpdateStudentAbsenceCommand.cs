using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.StudentAbsences.Commands.UpdateStudentAbsence;

public record UpdateStudentAbsenceCommand : IRequest
{
    public Guid Id { get; init; }
    public Guid StudentId { get; init; }
    public Guid RouteId { get; init; }
    public DateTime Date { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Reason { get; init; } = string.Empty;
}

public class UpdateStudentAbsenceCommandHandler : IRequestHandler<UpdateStudentAbsenceCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UpdateStudentAbsenceCommandHandler> _logger;

    public UpdateStudentAbsenceCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<UpdateStudentAbsenceCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task Handle(UpdateStudentAbsenceCommand request, CancellationToken cancellationToken)
    {
        var studentAbsence = await _unitOfWork.Repository<StudentAbsence>().GetByIdAsync(request.Id);

        if (studentAbsence == null)
        {
            throw new NotFoundException("Ausência do estudante não encontrada");
        }

        studentAbsence.StudentId = request.StudentId;
        studentAbsence.RouteId = request.RouteId;
        studentAbsence.Date = request.Date;
        studentAbsence.Type = request.Type;
        studentAbsence.Reason = request.Reason;

        await _unitOfWork.Repository<StudentAbsence>().UpdateAsync(studentAbsence);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Ausência do estudante {StudentAbsenceId} atualizada por {UserId}", request.Id, _currentUserService.UserId);
    }
}