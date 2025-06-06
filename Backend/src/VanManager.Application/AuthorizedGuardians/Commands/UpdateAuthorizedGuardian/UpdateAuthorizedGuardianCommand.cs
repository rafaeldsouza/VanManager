using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.AuthorizedGuardians.Commands.UpdateAuthorizedGuardian;

public record UpdateAuthorizedGuardianCommand : IRequest
{
    public Guid Id { get; init; }
    public Guid StudentId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Relationship { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string DocumentId { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}

public class UpdateAuthorizedGuardianCommandHandler : IRequestHandler<UpdateAuthorizedGuardianCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UpdateAuthorizedGuardianCommandHandler> _logger;

    public UpdateAuthorizedGuardianCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<UpdateAuthorizedGuardianCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task Handle(UpdateAuthorizedGuardianCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<AuthorizedGuardian>();
        var authorizedGuardian = await repository.GetByIdAsync(request.Id);

        if (authorizedGuardian == null)
        {
            _logger.LogWarning("Authorized guardian {AuthorizedGuardianId} not found for user {UserId}",
                               request.Id, _currentUserService.UserId);
            throw new NotFoundException("O responsável não foi encontrado.");
        }

        authorizedGuardian.StudentId = request.StudentId;
        authorizedGuardian.FullName = request.FullName;
        authorizedGuardian.Relationship = request.Relationship;
        authorizedGuardian.PhoneNumber = request.PhoneNumber;
        authorizedGuardian.DocumentId = request.DocumentId;
        authorizedGuardian.Description = request.Description;

        await repository.UpdateAsync(authorizedGuardian);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Authorized guardian {AuthorizedGuardianId} updated by user {UserId}",
            request.Id, _currentUserService.UserId);
    }
}
