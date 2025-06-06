using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.Fleets.Queries.GetFleets;

public record GetFleetsQuery : IRequest<IEnumerable<Fleet>>;

public class GetFleetsQueryHandler : IRequestHandler<GetFleetsQuery, IEnumerable<Fleet>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetFleetsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Fleet>> Handle(GetFleetsQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Repository<Fleet>().GetAllAsync();
    }
}