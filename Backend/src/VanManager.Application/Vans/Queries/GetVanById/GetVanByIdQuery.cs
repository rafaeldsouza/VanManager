using System;
using System.Threading;
using System.Threading.Tasks;
using VanManager.Domain.Entities; // Para a classe Van
using VanManager.Application.Common.Exceptions; // Para NotFoundException
using VanManager.Application.Common.Interfaces; // Para IUnitOfWork
using MediatR;
using VanManager.Domain.Repositories;

namespace VanManager.Application.Vans.Queries.GetVanById;

public record GetVanByIdQuery(Guid Id) : IRequest<Van>;

public class GetVanByIdQueryHandler : IRequestHandler<GetVanByIdQuery, Van>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetVanByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Van> Handle(GetVanByIdQuery request, CancellationToken cancellationToken)
    {
        var van = await _unitOfWork.Repository<Van>().GetByIdAsync(request.Id);

        if (van == null)
        {
            throw new NotFoundException("Van não encontrada");
        }

        return van;
    }
}