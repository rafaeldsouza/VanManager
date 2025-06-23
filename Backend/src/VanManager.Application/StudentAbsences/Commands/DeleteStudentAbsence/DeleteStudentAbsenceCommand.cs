using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.BusinessRules;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.StudentAbsences.Commands.DeleteStudentAbsence;

public record DeleteStudentAbsenceCommand(Guid Id) : IRequest;

public class DeleteStudentAbsenceCommandHandler : IRequestHandler<DeleteStudentAbsenceCommand>
{
    private readonly IRepository<StudentAbsence> _absenceRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<DeleteStudentAbsenceCommandHandler> _logger;

    public DeleteStudentAbsenceCommandHandler(
        IRepository<StudentAbsence> absenceRepository,
        ICurrentUserService currentUserService,
        ILogger<DeleteStudentAbsenceCommandHandler> logger)
    {
        _absenceRepository = absenceRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task Handle(DeleteStudentAbsenceCommand request, CancellationToken cancellationToken)
    {
        var absence = await _absenceRepository.GetByIdAsync(request.Id);
        if (absence == null)
        {
            throw new NotFoundException($"Absence record with ID {request.Id} not found");
        }

        if (!StudentAbsenceRules.CanDeleteAbsence(_currentUserService.GetRoles(), _currentUserService.AppUser,absence))
        {
            throw new UnauthorizedAccessException("Você não tem permissão para excluir esta falta.");
        }

        await _absenceRepository.DeleteAsync(absence);
        

        _logger.LogInformation(
            "Deleted absence record {AbsenceId} by user {UserId}",
            absence.Id,
            _currentUserService.UserId);
    }
}