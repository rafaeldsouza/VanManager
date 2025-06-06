using MediatR;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.AuthorizedGuardians.Queries.GetAuthorizedGuardians;

public record GetAuthorizedGuardiansQuery : IRequest<IEnumerable<AuthorizedGuardian>>;

public class GetAuthorizedGuardiansQueryHandler : IRequestHandler<GetAuthorizedGuardiansQuery, IEnumerable<AuthorizedGuardian>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAuthorizedGuardiansQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<AuthorizedGuardian>> Handle(GetAuthorizedGuardiansQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Repository<AuthorizedGuardian>().GetAllAsync();
    }
}