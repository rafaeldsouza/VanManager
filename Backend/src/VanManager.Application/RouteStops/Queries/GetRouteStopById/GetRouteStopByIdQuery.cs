using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.RouteStops.Queries.GetRouteStopById;

public record GetRouteStopByIdQuery(Guid Id) : IRequest<RouteStop>;

public class GetRouteStopByIdQueryHandler : IRequestHandler<GetRouteStopByIdQuery, RouteStop>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetRouteStopByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<RouteStop> Handle(GetRouteStopByIdQuery request, CancellationToken cancellationToken)
    {
        var routeStop = await _unitOfWork.Repository<RouteStop>().GetByIdAsync(request.Id);

        if (routeStop == null)
        {
            throw new NotFoundException("Parada de rota não encontrada");
        }

        return routeStop;
    }
}