using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.StudentTripLogs.Commands.UpdateStudentTripLog;

public record UpdateStudentTripLogCommand : IRequest
{
    public Guid Id { get; init; }
    public Guid StudentId { get; init; }
    public Guid RouteId { get; init; }
    public Guid VanId { get; init; }
    public DateTime BoardingTime { get; init; }
    public double BoardingLatitude { get; init; }
    public double BoardingLongitude { get; init; }
    public string? BoardingAddress { get; init; }
    public double DropoffLatitude { get; init; }
    public double DropoffLongitude { get; init; }
    public string? DropoffAddress { get; init; }
    public DateTime DropoffTime { get; init; }
    public string? Notes { get; init; }
    public TripStatus Status { get; init; }
}

public class UpdateStudentTripLogCommandHandler : IRequestHandler<UpdateStudentTripLogCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UpdateStudentTripLogCommandHandler> _logger;

    public UpdateStudentTripLogCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<UpdateStudentTripLogCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task Handle(UpdateStudentTripLogCommand request, CancellationToken cancellationToken)
    {
        var studentTripLog = await _unitOfWork.Repository<StudentTripLog>().GetByIdAsync(request.Id);

        if (studentTripLog == null)
        {
            throw new NotFoundException("Registro de viagem do estudante não encontrado");
        }

        studentTripLog.StudentId = request.StudentId;
        studentTripLog.RouteId = request.RouteId;
        studentTripLog.VanId = request.VanId;
        studentTripLog.BoardingTime = request.BoardingTime;
        studentTripLog.BoardingLatitude = request.BoardingLatitude;
        studentTripLog.BoardingLongitude = request.BoardingLongitude;
        studentTripLog.BoardingAddress = request.BoardingAddress;
        studentTripLog.DropoffLatitude = request.DropoffLatitude;
        studentTripLog.DropoffLongitude = request.DropoffLongitude;
        studentTripLog.DropoffAddress = request.DropoffAddress;
        studentTripLog.DropoffTime = request.DropoffTime;
        studentTripLog.Notes = request.Notes;
        studentTripLog.Status = request.Status;

        await _unitOfWork.Repository<StudentTripLog>().UpdateAsync(studentTripLog);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Registro de viagem do estudante {StudentTripLogId} atualizado por {UserId}", 
            request.Id, _currentUserService.UserId);
    }
} 