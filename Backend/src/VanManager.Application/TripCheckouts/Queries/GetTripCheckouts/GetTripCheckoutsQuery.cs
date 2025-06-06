using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.TripCheckouts.Queries.GetTripCheckouts;

public record GetTripCheckoutsQuery : IRequest<IEnumerable<TripCheckout>>;

public class GetTripCheckoutsQueryHandler : IRequestHandler<GetTripCheckoutsQuery, IEnumerable<TripCheckout>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTripCheckoutsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<TripCheckout>> Handle(GetTripCheckoutsQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Repository<TripCheckout>().GetAllAsync();
    }
}