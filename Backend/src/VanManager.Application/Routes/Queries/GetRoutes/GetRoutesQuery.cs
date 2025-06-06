using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.Routes.Queries.GetRoutes;

public record GetRoutesQuery : IRequest<IEnumerable<Route>>;

public class GetRoutesQueryHandler : IRequestHandler<GetRoutesQuery, IEnumerable<Route>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetRoutesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Route>> Handle(GetRoutesQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Repository<Route>().GetAllAsync();
    }
}