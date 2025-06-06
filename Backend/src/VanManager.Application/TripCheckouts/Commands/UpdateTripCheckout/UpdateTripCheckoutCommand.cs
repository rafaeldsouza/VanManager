using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.TripCheckouts.Commands.UpdateTripCheckout;

public record UpdateTripCheckoutCommand : IRequest
{
    public Guid Id { get; init; }
    public Guid StudentId { get; init; }
    public Guid ResponsibleId { get; init; }
    public Guid RouteId { get; init; }
    public DateTime Timestamp { get; init; }
}

public class UpdateTripCheckoutCommandHandler : IRequestHandler<UpdateTripCheckoutCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UpdateTripCheckoutCommandHandler> _logger;

    public UpdateTripCheckoutCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<UpdateTripCheckoutCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task Handle(UpdateTripCheckoutCommand request, CancellationToken cancellationToken)
    {
        var tripCheckout = await _unitOfWork.Repository<TripCheckout>().GetByIdAsync(request.Id);

        if (tripCheckout == null)
        {
            throw new NotFoundException("Trip checkout not found");
        }

        tripCheckout.StudentId = request.StudentId;
        tripCheckout.ResponsibleId = request.ResponsibleId;
        tripCheckout.RouteId = request.RouteId;
        tripCheckout.Timestamp = request.Timestamp;

        await _unitOfWork.Repository<TripCheckout>().UpdateAsync(tripCheckout);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Trip checkout {TripCheckoutId} updated by {UserId}", request.Id, _currentUserService.UserId);
    }
}