using MediatR;
using Microsoft.EntityFrameworkCore;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;

namespace VanManager.Application.Fleets.Queries.GetFleetsByOwnerId;
public record GetFleetsByOwnerIdQuery(Guid OwnerUserId) : IRequest<IEnumerable<Fleet>>;


public class GetFleetsByOwnerIdQueryHandler : IRequestHandler<GetFleetsByOwnerIdQuery, IEnumerable<Fleet>>
{
    private readonly IApplicationDbContext _context;

    public GetFleetsByOwnerIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Fleet>> Handle(GetFleetsByOwnerIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Fleets
            .Where(f => f.OwnerUserId == request.OwnerUserId)
            .ToListAsync(cancellationToken);
    }
}