using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.Routes.Commands.UpdateRoute;

public record UpdateRouteCommand : IRequest
{
    public Guid Id { get; init; }
    public Guid VanId { get; init; }
    public string Description { get; init; } = string.Empty;
}

public class UpdateRouteCommandHandler : IRequestHandler<UpdateRouteCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UpdateRouteCommandHandler> _logger;

    public UpdateRouteCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<UpdateRouteCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task Handle(UpdateRouteCommand request, CancellationToken cancellationToken)
    {
        var route = await _unitOfWork.Repository<Route>().GetByIdAsync(request.Id);

        if (route == null)
        {
            throw new NotFoundException("Route not found");
        }

        route.VanId = request.VanId;
        route.Description = request.Description;

        await _unitOfWork.Repository<Route>().UpdateAsync(route);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Route {RouteId} updated by {UserId}", request.Id, _currentUserService.UserId);
    }
}