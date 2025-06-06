using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.RouteStops.Queries.GetRouteStops;

public record GetRouteStopsQuery : IRequest<IEnumerable<RouteStop>>;

public class GetRouteStopsQueryHandler : IRequestHandler<GetRouteStopsQuery, IEnumerable<RouteStop>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetRouteStopsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<RouteStop>> Handle(GetRouteStopsQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Repository<RouteStop>().GetAllAsync();
    }
}