using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.StudentTripLogs.Commands.CreateStudentTripLog;

public record CreateStudentTripLogCommand : IRequest<StudentTripLog>
{
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

public class CreateStudentTripLogCommandHandler : IRequestHandler<CreateStudentTripLogCommand, StudentTripLog>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CreateStudentTripLogCommandHandler> _logger;

    public CreateStudentTripLogCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<CreateStudentTripLogCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<StudentTripLog> Handle(CreateStudentTripLogCommand request, CancellationToken cancellationToken)
    {
        var studentTripLog = new StudentTripLog
        {
            Id = Guid.NewGuid(),
            StudentId = request.StudentId,
            RouteId = request.RouteId,
            VanId = request.VanId,
            BoardingTime = request.BoardingTime,
            BoardingLatitude = request.BoardingLatitude,
            BoardingLongitude = request.BoardingLongitude,
            BoardingAddress = request.BoardingAddress,
            DropoffLatitude = request.DropoffLatitude,
            DropoffLongitude = request.DropoffLongitude,
            DropoffAddress = request.DropoffAddress,
            DropoffTime = request.DropoffTime,
            Notes = request.Notes,
            Status = request.Status
        };

        await _unitOfWork.Repository<StudentTripLog>().AddAsync(studentTripLog);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Registro de viagem do estudante {StudentTripLogId} criado por {UserId}", 
            studentTripLog.Id, _currentUserService.UserId);

        return studentTripLog;
    }
} 