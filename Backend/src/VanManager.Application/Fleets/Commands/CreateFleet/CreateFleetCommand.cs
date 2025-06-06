using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.Fleets.Commands.CreateFleet;

public record CreateFleetCommand : IRequest<Fleet>
{
    public string Name { get; init; } = string.Empty;
    public Guid OwnerUserId { get; init; }
}

public class CreateFleetCommandHandler : IRequestHandler<CreateFleetCommand, Fleet>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CreateFleetCommandHandler> _logger;

    public CreateFleetCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<CreateFleetCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Fleet> Handle(CreateFleetCommand request, CancellationToken cancellationToken)
    {
        var fleet = new Fleet
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            OwnerUserId = request.OwnerUserId
        };

        await _unitOfWork.Repository<Fleet>().AddAsync(fleet);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Frota {FleetId} criada por {UserId}", fleet.Id, _currentUserService.UserId);

        return fleet;
    }
}