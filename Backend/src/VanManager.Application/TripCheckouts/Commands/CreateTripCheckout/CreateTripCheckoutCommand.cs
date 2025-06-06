using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

using VanManager.Domain.Entities;

namespace VanManager.Application.TripCheckouts.Commands.CreateTripCheckout;

public record CreateTripCheckoutCommand : IRequest<TripCheckout>
{
    public Guid StudentId { get; init; }
    public Guid ResponsibleId { get; init; }
    public Guid RouteId { get; init; }
    public DateTime Timestamp { get; init; }
}

public class CreateTripCheckoutCommandHandler : IRequestHandler<CreateTripCheckoutCommand, TripCheckout>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CreateTripCheckoutCommandHandler> _logger;

    public CreateTripCheckoutCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<CreateTripCheckoutCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<TripCheckout> Handle(CreateTripCheckoutCommand request, CancellationToken cancellationToken)
    {
        var tripCheckout = new TripCheckout
        {
            Id = Guid.NewGuid(),
            StudentId = request.StudentId,
            ResponsibleId = request.ResponsibleId,
            RouteId = request.RouteId,
            Timestamp = request.Timestamp
        };

        await _unitOfWork.Repository<TripCheckout>().AddAsync(tripCheckout);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Trip checkout {TripCheckoutId} created by {UserId}", tripCheckout.Id, _currentUserService.UserId);

        return tripCheckout;
    }
}