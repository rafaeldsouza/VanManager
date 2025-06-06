using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.Fleets.Queries.GetFleetById;

public record GetFleetByIdQuery(Guid Id) : IRequest<Fleet>;

public class GetFleetByIdQueryHandler : IRequestHandler<GetFleetByIdQuery, Fleet>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetFleetByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Fleet> Handle(GetFleetByIdQuery request, CancellationToken cancellationToken)
    {
        var fleet = await _unitOfWork.Repository<Fleet>().GetByIdAsync(request.Id);

        if (fleet == null)
        {
            throw new NotFoundException("Frota não encontrada");
        }

        return fleet;
    }
}