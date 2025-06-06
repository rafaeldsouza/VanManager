using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.Schools.Commands.DeleteSchool;

public record DeleteSchoolCommand(Guid Id) : IRequest;

public class DeleteSchoolCommandHandler : IRequestHandler<DeleteSchoolCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<DeleteSchoolCommandHandler> _logger;

    public DeleteSchoolCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<DeleteSchoolCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task Handle(DeleteSchoolCommand request, CancellationToken cancellationToken)
    {
        var school = await _unitOfWork.Repository<School>().GetByIdAsync(request.Id);

        if (school == null)
        {
            throw new NotFoundException("Escola não encontrada");
        }

        await _unitOfWork.Repository<School>().DeleteAsync(school);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Escola {SchoolId} excluída por {UserId}", request.Id, _currentUserService.UserId);
    }
}