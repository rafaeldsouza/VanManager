using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.StudentTripLogs.Commands.DeleteStudentTripLog;

public record DeleteStudentTripLogCommand(Guid Id) : IRequest;

public class DeleteStudentTripLogCommandHandler : IRequestHandler<DeleteStudentTripLogCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<DeleteStudentTripLogCommandHandler> _logger;

    public DeleteStudentTripLogCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<DeleteStudentTripLogCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task Handle(DeleteStudentTripLogCommand request, CancellationToken cancellationToken)
    {
        var studentTripLog = await _unitOfWork.Repository<StudentTripLog>().GetByIdAsync(request.Id);

        if (studentTripLog == null)
        {
            throw new NotFoundException("Registro de viagem do estudante não encontrado");
        }

        await _unitOfWork.Repository<StudentTripLog>().DeleteAsync(studentTripLog);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Registro de viagem do estudante {StudentTripLogId} excluído por {UserId}", 
            request.Id, _currentUserService.UserId);
    }
} 