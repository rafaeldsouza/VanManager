using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.Routes.Commands.CreateRoute;

public record CreateRouteCommand : IRequest<Route>
{
    public Guid VanId { get; init; }
    public string Description { get; init; } = string.Empty;
}

public class CreateRouteCommandHandler : IRequestHandler<CreateRouteCommand, Route>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CreateRouteCommandHandler> _logger;

    public CreateRouteCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<CreateRouteCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Route> Handle(CreateRouteCommand request, CancellationToken cancellationToken)
    {
        var route = new Route
        {
            Id = Guid.NewGuid(),
            VanId = request.VanId,
            Description = request.Description
        };

        await _unitOfWork.Repository<Route>().AddAsync(route);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Route {RouteId} created by {UserId}", route.Id, _currentUserService.UserId);

        return route;
    }
}