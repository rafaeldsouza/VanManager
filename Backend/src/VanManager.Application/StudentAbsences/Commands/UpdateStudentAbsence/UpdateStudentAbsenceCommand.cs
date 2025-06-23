using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.BusinessRules;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.StudentAbsences.Commands.UpdateStudentAbsence;

public record UpdateStudentAbsenceCommand(
    Guid Id,
    string? Reason,
    string? Justification,
    bool IsJustified,
    bool IsApproved) : IRequest;

public class UpdateStudentAbsenceCommandHandler : IRequestHandler<UpdateStudentAbsenceCommand>
{
    private readonly IRepository<StudentAbsence> _absenceRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UpdateStudentAbsenceCommandHandler> _logger;

    public UpdateStudentAbsenceCommandHandler(
        IRepository<StudentAbsence> absenceRepository,
        ICurrentUserService currentUserService,
        ILogger<UpdateStudentAbsenceCommandHandler> logger)
    {
        _absenceRepository = absenceRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task Handle(UpdateStudentAbsenceCommand request, CancellationToken cancellationToken)
    {
        var absence = await _absenceRepository.GetByIdAsync(request.Id);
        if (absence == null)
        {
            throw new NotFoundException($"Absence record with ID {request.Id} not found");
        }

        if (!StudentAbsenceRules.CanUpdateAbsence(_currentUserService.GetRoles(), _currentUserService.AppUser, absence))
        {
            throw new UnauthorizedAccessException("Você não tem permissão para atualizar esta falta.");
        }

        absence.Reason = request.Reason ?? string.Empty;
        absence.Justification = request.Justification;
        absence.IsJustified = request.IsJustified;
        absence.UpdatedAt = DateTime.UtcNow;
        absence.UpdatedByUserId = _currentUserService.UserId;

        await _absenceRepository.UpdateAsync(absence);

        _logger.LogInformation(
            "Updated absence record {AbsenceId} by user {UserId}",
            absence.Id,
            _currentUserService.UserId);
    }
}