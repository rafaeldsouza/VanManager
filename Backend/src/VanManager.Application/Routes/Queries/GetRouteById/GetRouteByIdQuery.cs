using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.Routes.Queries.GetRouteById;

public record GetRouteByIdQuery(Guid Id) : IRequest<Route>;

public class GetRouteByIdQueryHandler : IRequestHandler<GetRouteByIdQuery, Route>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetRouteByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Route> Handle(GetRouteByIdQuery request, CancellationToken cancellationToken)
    {
        var route = await _unitOfWork.Repository<Route>().GetByIdAsync(request.Id);

        if (route == null)
        {
            throw new NotFoundException("Route not found");
        }

        return route;
    }
}