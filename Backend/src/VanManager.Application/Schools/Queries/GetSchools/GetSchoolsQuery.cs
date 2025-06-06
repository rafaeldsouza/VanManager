using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.Schools.Queries.GetSchools;

public record GetSchoolsQuery : IRequest<IEnumerable<School>>;

public class GetSchoolsQueryHandler : IRequestHandler<GetSchoolsQuery, IEnumerable<School>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetSchoolsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<School>> Handle(GetSchoolsQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Repository<School>().GetAllAsync();
    }
}