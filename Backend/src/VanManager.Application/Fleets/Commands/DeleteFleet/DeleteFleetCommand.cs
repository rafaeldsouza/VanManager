using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.Fleets.Commands.DeleteFleet;

public record DeleteFleetCommand(Guid Id) : IRequest;

public class DeleteFleetCommandHandler : IRequestHandler<DeleteFleetCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<DeleteFleetCommandHandler> _logger;

    public DeleteFleetCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<DeleteFleetCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task Handle(DeleteFleetCommand request, CancellationToken cancellationToken)
    {
        var fleet = await _unitOfWork.Repository<Fleet>().GetByIdAsync(request.Id);

        if (fleet == null)
        {
            throw new NotFoundException("Frota não encontrada");
        }

        await _unitOfWork.Repository<Fleet>().DeleteAsync(fleet);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Frota {FleetId} excluída por {UserId}", request.Id, _currentUserService.UserId);
    }
}