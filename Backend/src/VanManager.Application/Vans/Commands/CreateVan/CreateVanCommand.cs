using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.Vans.Commands.CreateVan;

public record CreateVanCommand : IRequest<Van>
{
    public string PlateNumber { get; init; } = string.Empty;
    public Guid FleetId { get; init; }
    public Guid AssignedDriverId { get; init; }
    public int Capacity { get; init; }
}

public class CreateVanCommandHandler : IRequestHandler<CreateVanCommand, Van>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CreateVanCommandHandler> _logger;

    public CreateVanCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<CreateVanCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Van> Handle(CreateVanCommand request, CancellationToken cancellationToken)
    {
        var van = new Van
        {
            Id = Guid.NewGuid(),
            PlateNumber = request.PlateNumber,
            FleetId = request.FleetId,
            AssignedDriverId = request.AssignedDriverId,
            Capacity = request.Capacity
        };

        await _unitOfWork.Repository<Van>().AddAsync(van);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Van {VanId} criada por {UserId}", van.Id, _currentUserService.UserId);

        return van;
    }
}