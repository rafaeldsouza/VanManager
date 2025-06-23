using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.Vans.Commands.UpdateVan;

public record UpdateVanCommand : IRequest
{
    public Guid Id { get; init; }
    public string PlateNumber { get; init; } = string.Empty;
    public Guid FleetId { get; init; }
    public Guid AssignedDriverId { get; init; }
    public int Capacity { get; init; }
}

public class UpdateVanCommandHandler : IRequestHandler<UpdateVanCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UpdateVanCommandHandler> _logger;

    public UpdateVanCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<UpdateVanCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task Handle(UpdateVanCommand request, CancellationToken cancellationToken)
    {
        var van = await _unitOfWork.Repository<Van>().GetByIdAsync(request.Id);

        if (van == null)
        {
            throw new NotFoundException("Van não encontrada");
        }

        if (van.Fleet.OwnerUserId == _currentUserService?.UserId || van.DriverId == _currentUserService?.UserId)
        {
            _logger.LogWarning("Tentativa de atualização de van {VanId} por usuário não autorizado {UserId}",
                                               request.Id, _currentUserService.UserId);
            throw new UnauthorizedAccessException("Você não tem permissão para atualizar esta van.");
        }

        van.PlateNumber = request.PlateNumber;
        van.FleetId = request.FleetId;
        van.DriverId = request.AssignedDriverId;
        van.Capacity = request.Capacity;

        await _unitOfWork.Repository<Van>().UpdateAsync(van);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Van {VanId} atualizada por {UserId}", request.Id, _currentUserService.UserId);
    }
}