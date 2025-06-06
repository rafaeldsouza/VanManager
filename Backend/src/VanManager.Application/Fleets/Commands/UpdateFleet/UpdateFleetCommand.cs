using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.Fleets.Commands.UpdateFleet;

public record UpdateFleetCommand : IRequest
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public Guid OwnerUserId { get; init; }
}

public class UpdateFleetCommandHandler : IRequestHandler<UpdateFleetCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UpdateFleetCommandHandler> _logger;

    public UpdateFleetCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<UpdateFleetCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task Handle(UpdateFleetCommand request, CancellationToken cancellationToken)
    {
        var fleet = await _unitOfWork.Repository<Fleet>().GetByIdAsync(request.Id);

        if (fleet == null)
        {
            throw new NotFoundException("Frota não encontrada");
        }

        fleet.Name = request.Name;
        fleet.OwnerUserId = request.OwnerUserId;

        await _unitOfWork.Repository<Fleet>().UpdateAsync(fleet);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Frota {FleetId} atualizada por {UserId}", request.Id, _currentUserService.UserId);
    }
}