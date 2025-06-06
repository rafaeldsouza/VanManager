using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.Vans.Commands.DeleteVan;

public record DeleteVanCommand(Guid Id) : IRequest;

public class DeleteVanCommandHandler : IRequestHandler<DeleteVanCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<DeleteVanCommandHandler> _logger;

    public DeleteVanCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<DeleteVanCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task Handle(DeleteVanCommand request, CancellationToken cancellationToken)
    {
        var van = await _unitOfWork.Repository<Van>().GetByIdAsync(request.Id);

        if (van == null)
        {
            throw new NotFoundException("Van não encontrada");
        }

        await _unitOfWork.Repository<Van>().DeleteAsync(van);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Van {VanId} excluída por {UserId}", request.Id, _currentUserService.UserId);
    }
}