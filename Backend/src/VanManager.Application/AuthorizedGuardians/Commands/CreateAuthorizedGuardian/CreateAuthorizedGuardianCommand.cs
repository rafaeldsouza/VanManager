using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.AuthorizedGuardians.Commands.CreateAuthorizedGuardian;

public record CreateAuthorizedGuardianCommand : IRequest<AuthorizedGuardian>
{
    public Guid StudentId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Relationship { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string DocumentId { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}

public class CreateAuthorizedGuardianCommandHandler : IRequestHandler<CreateAuthorizedGuardianCommand, AuthorizedGuardian>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CreateAuthorizedGuardianCommandHandler> _logger;

    public CreateAuthorizedGuardianCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<CreateAuthorizedGuardianCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<AuthorizedGuardian> Handle(CreateAuthorizedGuardianCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // (Opcional) Verificar se o estudante existe
            var student = await _unitOfWork.Repository<Student>().GetByIdAsync(request.StudentId);
            if (student == null)
            {
                throw new NotFoundException("Student not found");
            }

            var authorizedGuardian = new AuthorizedGuardian
            {
                Id = Guid.NewGuid(),
                StudentId = request.StudentId,
                FullName = request.FullName,
                Relationship = request.Relationship,
                PhoneNumber = request.PhoneNumber,
                DocumentId = request.DocumentId,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<AuthorizedGuardian>().AddAsync(authorizedGuardian);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Authorized guardian {AuthorizedGuardianId} created for student {StudentId} by user {UserId}",
                authorizedGuardian.Id, request.StudentId, _currentUserService.UserId
            );

            return authorizedGuardian;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating authorized guardian for student {StudentId} by user {UserId}",
                request.StudentId, _currentUserService.UserId);
            throw;
        }
    }
}
