using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.AuthorizedGuardians.Queries.GetAuthorizedGuardianById;

public record GetAuthorizedGuardianByIdQuery(Guid Id) : IRequest<AuthorizedGuardian>;

public class GetAuthorizedGuardianByIdQueryHandler : IRequestHandler<GetAuthorizedGuardianByIdQuery, AuthorizedGuardian>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAuthorizedGuardianByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthorizedGuardian> Handle(GetAuthorizedGuardianByIdQuery request, CancellationToken cancellationToken)
    {
        var authorizedGuardian = await _unitOfWork.Repository<AuthorizedGuardian>().GetByIdAsync(request.Id);

        if (authorizedGuardian == null)
        {
            throw new NotFoundException("Authorized guardian not found");
        }

        return authorizedGuardian;
    }
}