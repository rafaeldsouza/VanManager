using MediatR;
using Microsoft.EntityFrameworkCore;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;

namespace VanManager.Application.Fleets.Queries.GetFleetByDriverId;


public record GetFleetByDriverIdQuery(Guid DriverUserId) : IRequest<IEnumerable<Fleet>>;


public class GetFleetByDriverIdQueryHandler : IRequestHandler<GetFleetByDriverIdQuery, IEnumerable<Fleet>>
{
    private readonly IApplicationDbContext _context;

    public GetFleetByDriverIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Fleet>> Handle(GetFleetByDriverIdQuery request, CancellationToken cancellationToken)
    {
         return await _context.Fleets
        .Include(f => f.Vans)
        .Where(
            f => f.Vans.Any(v => v.AssignedDriverId == request.DriverUserId)            
        ).ToListAsync(cancellationToken);
    }
}