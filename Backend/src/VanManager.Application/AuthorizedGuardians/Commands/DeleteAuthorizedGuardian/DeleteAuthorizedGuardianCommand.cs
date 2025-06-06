using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.AuthorizedGuardians.Commands.DeleteAuthorizedGuardian;

public record DeleteAuthorizedGuardianCommand(Guid Id) : IRequest;

public class DeleteAuthorizedGuardianCommandHandler : IRequestHandler<DeleteAuthorizedGuardianCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<DeleteAuthorizedGuardianCommandHandler> _logger;

    public DeleteAuthorizedGuardianCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<DeleteAuthorizedGuardianCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task Handle(DeleteAuthorizedGuardianCommand request, CancellationToken cancellationToken)
    {
        var authorizedGuardian = await _unitOfWork.Repository<AuthorizedGuardian>().GetByIdAsync(request.Id);

        if (authorizedGuardian == null)
        {
            _logger.LogWarning("Authorized guardian {AuthorizedGuardianId} not found", request.Id);
            throw new NotFoundException("Authorized guardian not found");
        }

        await _unitOfWork.Repository<AuthorizedGuardian>().DeleteAsync(authorizedGuardian);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Authorized guardian {AuthorizedGuardianId} deleted by {UserId}", request.Id, _currentUserService.UserId);
    }
}