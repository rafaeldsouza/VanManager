using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.Plans.Queries.GetPlanById;

public record GetPlanByIdQuery(Guid Id) : IRequest<Plan>;

public class GetPlanByIdQueryHandler : IRequestHandler<GetPlanByIdQuery, Plan>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPlanByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Plan> Handle(GetPlanByIdQuery request, CancellationToken cancellationToken)
    {
        var plan = await _unitOfWork.Repository<Plan>().GetByIdAsync(request.Id);

        if (plan == null)
        {
            throw new NotFoundException("Plan not found");
        }

        return plan;
    }
}