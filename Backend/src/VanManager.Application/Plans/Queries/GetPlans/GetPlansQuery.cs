using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.Plans.Queries.GetPlans;

public record GetPlansQuery : IRequest<IEnumerable<Plan>>;

public class GetPlansQueryHandler : IRequestHandler<GetPlansQuery, IEnumerable<Plan>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPlansQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Plan>> Handle(GetPlansQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Repository<Plan>().GetAllAsync();
    }
}