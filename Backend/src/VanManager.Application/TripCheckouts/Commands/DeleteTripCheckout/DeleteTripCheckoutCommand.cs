using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.TripCheckouts.Commands.DeleteTripCheckout;

public record DeleteTripCheckoutCommand(Guid Id) : IRequest;

public class DeleteTripCheckoutCommandHandler : IRequestHandler<DeleteTripCheckoutCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<DeleteTripCheckoutCommandHandler> _logger;

    public DeleteTripCheckoutCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<DeleteTripCheckoutCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task Handle(DeleteTripCheckoutCommand request, CancellationToken cancellationToken)
    {
        var tripCheckout = await _unitOfWork.Repository<TripCheckout>().GetByIdAsync(request.Id);

        if (tripCheckout == null)
        {
            throw new NotFoundException("Trip checkout not found");
        }

        await _unitOfWork.Repository<TripCheckout>().DeleteAsync(tripCheckout);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Trip checkout {TripCheckoutId} deleted by {UserId}", request.Id, _currentUserService.UserId);
    }
}