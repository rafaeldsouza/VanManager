using MediatR;
using Microsoft.EntityFrameworkCore;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.AuthorizedGuardians.Queries.GetAuthorizedGuardians;

public record GetAuthorizedGuardiansQuery(Guid StudentId) : IRequest<IEnumerable<AuthorizedGuardian>>;

public class GetAuthorizedGuardiansQueryHandler : IRequestHandler<GetAuthorizedGuardiansQuery, IEnumerable<AuthorizedGuardian>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAuthorizedGuardiansQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<AuthorizedGuardian>> Handle(GetAuthorizedGuardiansQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Repository<AuthorizedGuardian>().GetQueryable().Where(x => x.StudentId == request.StudentId).ToListAsync(cancellationToken);
    }
}