using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.RouteStops.Commands.UpdateRouteStop;

public record UpdateRouteStopCommand : IRequest
{
    public Guid Id { get; init; }
    public Guid RouteId { get; init; }
    public Guid StudentId { get; init; }
    public DateTime Timestamp { get; init; }
    public string Type { get; init; } = string.Empty;
    public decimal LocationLat { get; init; }
    public decimal LocationLng { get; init; }
}

public class UpdateRouteStopCommandHandler : IRequestHandler<UpdateRouteStopCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UpdateRouteStopCommandHandler> _logger;

    public UpdateRouteStopCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<UpdateRouteStopCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task Handle(UpdateRouteStopCommand request, CancellationToken cancellationToken)
    {
        var routeStop = await _unitOfWork.Repository<RouteStop>().GetByIdAsync(request.Id);

        if (routeStop == null)
        {
            throw new NotFoundException("Parada de rota não encontrada");
        }

        routeStop.RouteId = request.RouteId;
        routeStop.StudentId = request.StudentId;
        routeStop.Timestamp = request.Timestamp;
        routeStop.Type = request.Type;
        routeStop.LocationLat = request.LocationLat;
        routeStop.LocationLng = request.LocationLng;

        await _unitOfWork.Repository<RouteStop>().UpdateAsync(routeStop);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Parada de rota {RouteStopId} atualizada por {UserId}", request.Id, _currentUserService.UserId);
    }
}