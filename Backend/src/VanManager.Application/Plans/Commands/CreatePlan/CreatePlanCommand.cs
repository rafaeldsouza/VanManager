using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.Plans.Commands.CreatePlan;

public record CreatePlanCommand : IRequest<Plan>
{
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int MaxVans { get; init; }
    public bool Active { get; init; }
    public bool Visible { get; init; }
}

public class CreatePlanCommandHandler : IRequestHandler<CreatePlanCommand, Plan>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CreatePlanCommandHandler> _logger;

    public CreatePlanCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<CreatePlanCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Plan> Handle(CreatePlanCommand request, CancellationToken cancellationToken)
    {
        var plan = new Plan
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Price = request.Price,
            MaxVans = request.MaxVans,
            Active = request.Active,
            Visible = request.Visible
        };

        await _unitOfWork.Repository<Plan>().AddAsync(plan);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Plan {PlanId} created by {UserId}", plan.Id, _currentUserService.UserId);

        return plan;
    }
}