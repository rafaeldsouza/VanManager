using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.TripCheckouts.Queries.GetTripCheckoutById;

public record GetTripCheckoutByIdQuery(Guid Id) : IRequest<TripCheckout>;

public class GetTripCheckoutByIdQueryHandler : IRequestHandler<GetTripCheckoutByIdQuery, TripCheckout>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTripCheckoutByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TripCheckout> Handle(GetTripCheckoutByIdQuery request, CancellationToken cancellationToken)
    {
        var tripCheckout = await _unitOfWork.Repository<TripCheckout>().GetByIdAsync(request.Id);

        if (tripCheckout == null)
        {
            throw new NotFoundException("Trip checkout not found");
        }

        return tripCheckout;
    }
}