using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.StudentAbsences.Commands.DeleteStudentAbsence;

public record DeleteStudentAbsenceCommand(Guid Id) : IRequest;

public class DeleteStudentAbsenceCommandHandler : IRequestHandler<DeleteStudentAbsenceCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<DeleteStudentAbsenceCommandHandler> _logger;

    public DeleteStudentAbsenceCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<DeleteStudentAbsenceCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task Handle(DeleteStudentAbsenceCommand request, CancellationToken cancellationToken)
    {
        var studentAbsence = await _unitOfWork.Repository<StudentAbsence>().GetByIdAsync(request.Id);

        if (studentAbsence == null)
        {
            throw new NotFoundException("Ausência do estudante não encontrada");
        }

        await _unitOfWork.Repository<StudentAbsence>().DeleteAsync(studentAbsence);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Ausência do estudante {StudentAbsenceId} excluída por {UserId}", request.Id, _currentUserService.UserId);
    }
}