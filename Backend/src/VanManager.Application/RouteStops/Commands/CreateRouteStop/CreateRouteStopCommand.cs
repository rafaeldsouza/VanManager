using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.RouteStops.Commands.CreateRouteStop;

public record CreateRouteStopCommand : IRequest<RouteStop>
{
    public Guid RouteId { get; init; }
    public Guid StudentId { get; init; }
    public DateTime Timestamp { get; init; }
    public string Type { get; init; } = string.Empty;
    public decimal LocationLat { get; init; }
    public decimal LocationLng { get; init; }
}

public class CreateRouteStopCommandHandler : IRequestHandler<CreateRouteStopCommand, RouteStop>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CreateRouteStopCommandHandler> _logger;

    public CreateRouteStopCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<CreateRouteStopCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<RouteStop> Handle(CreateRouteStopCommand request, CancellationToken cancellationToken)
    {
        var routeStop = new RouteStop
        {
            Id = Guid.NewGuid(),
            RouteId = request.RouteId,
            StudentId = request.StudentId,
            Timestamp = request.Timestamp,
            Type = request.Type,
            LocationLat = request.LocationLat,
            LocationLng = request.LocationLng
        };

        await _unitOfWork.Repository<RouteStop>().AddAsync(routeStop);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Parada de rota {RouteStopId} criada por {UserId}", routeStop.Id, _currentUserService.UserId);

        return routeStop;
    }
}