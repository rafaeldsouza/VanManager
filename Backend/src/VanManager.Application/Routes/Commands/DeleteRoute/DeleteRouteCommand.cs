using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.Routes.Commands.DeleteRoute;

public record DeleteRouteCommand(Guid Id) : IRequest;

public class DeleteRouteCommandHandler : IRequestHandler<DeleteRouteCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<DeleteRouteCommandHandler> _logger;

    public DeleteRouteCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<DeleteRouteCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task Handle(DeleteRouteCommand request, CancellationToken cancellationToken)
    {
        var route = await _unitOfWork.Repository<Route>().GetByIdAsync(request.Id);

        if (route == null)
        {
            throw new NotFoundException("Route not found");
        }

        await _unitOfWork.Repository<Route>().DeleteAsync(route);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Route {RouteId} deleted by {UserId}", request.Id, _currentUserService.UserId);
    }
}