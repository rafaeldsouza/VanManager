using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.Plans.Commands.UpdatePlan;

public record UpdatePlanCommand : IRequest
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int MaxVans { get; init; }
    public bool Active { get; init; }
    public bool Visible { get; init; }
}

public class UpdatePlanCommandHandler : IRequestHandler<UpdatePlanCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UpdatePlanCommandHandler> _logger;

    public UpdatePlanCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<UpdatePlanCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task Handle(UpdatePlanCommand request, CancellationToken cancellationToken)
    {
        var plan = await _unitOfWork.Repository<Plan>().GetByIdAsync(request.Id);

        if (plan == null)
        {
            throw new NotFoundException("Plan not found");
        }

        plan.Name = request.Name;
        plan.Price = request.Price;
        plan.MaxVans = request.MaxVans;

        await _unitOfWork.Repository<Plan>().UpdateAsync(plan);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Plan {PlanId} updated by {UserId}", request.Id, _currentUserService.UserId);
    }
}