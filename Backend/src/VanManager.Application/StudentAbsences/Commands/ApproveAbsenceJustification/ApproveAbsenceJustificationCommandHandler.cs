using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.BusinessRules;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.StudentAbsences.Commands.ApproveAbsenceJustification;

public record ApproveAbsenceJustificationCommand(Guid Id) : IRequest;

public class ApproveAbsenceJustificationCommandHandler : IRequestHandler<ApproveAbsenceJustificationCommand>
{
    private readonly IRepository<StudentAbsence> _absenceRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ApproveAbsenceJustificationCommandHandler> _logger;

    public ApproveAbsenceJustificationCommandHandler(
        IRepository<StudentAbsence> absenceRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        ILogger<ApproveAbsenceJustificationCommandHandler> logger)
    {
        _absenceRepository = absenceRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(ApproveAbsenceJustificationCommand request, CancellationToken cancellationToken)
    {
        var absence = await _absenceRepository.GetByIdAsync(request.Id);
        if (absence == null)
        {
            throw new NotFoundException($"Absence record with ID {request.Id} not found");
        }

        if (!StudentAbsenceRules.CanApproveJustification(_currentUserService.GetRoles(), _currentUserService.AppUser,absence))
        {
            throw new UnauthorizedAccessException("Você não tem permissão para aprovar esta justificativa.");
        }
        
        absence.UpdatedAt = DateTime.UtcNow;
        absence.UpdatedByUserId = _currentUserService.UserId;

        await _absenceRepository.UpdateAsync(absence);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Approved absence justification {AbsenceId} by user {UserId}",
            absence.Id,
            _currentUserService.UserId);
    }
}