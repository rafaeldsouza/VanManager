using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.Plans.Commands.DeletePlan;

public record DeletePlanCommand(Guid Id) : IRequest;

public class DeletePlanCommandHandler : IRequestHandler<DeletePlanCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<DeletePlanCommandHandler> _logger;

    public DeletePlanCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<DeletePlanCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task Handle(DeletePlanCommand request, CancellationToken cancellationToken)
    {
        var plan = await _unitOfWork.Repository<Plan>().GetByIdAsync(request.Id);

        if (plan == null)
        {
            throw new NotFoundException("Plan not found");
        }

        await _unitOfWork.Repository<Plan>().DeleteAsync(plan);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Plan {PlanId} deleted by {UserId}", request.Id, _currentUserService.UserId);
    }
}