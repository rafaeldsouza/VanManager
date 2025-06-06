using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.RouteStops.Commands.DeleteRouteStop;

public record DeleteRouteStopCommand(Guid Id) : IRequest;

public class DeleteRouteStopCommandHandler : IRequestHandler<DeleteRouteStopCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<DeleteRouteStopCommandHandler> _logger;

    public DeleteRouteStopCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<DeleteRouteStopCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task Handle(DeleteRouteStopCommand request, CancellationToken cancellationToken)
    {
        var routeStop = await _unitOfWork.Repository<RouteStop>().GetByIdAsync(request.Id);

        if (routeStop == null)
        {
            throw new NotFoundException("Parada de rota não encontrada");
        }

        await _unitOfWork.Repository<RouteStop>().DeleteAsync(routeStop);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Parada de rota {RouteStopId} excluída por {UserId}", request.Id, _currentUserService.UserId);
    }
}