using MediatR;
using Microsoft.EntityFrameworkCore;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.StudentTripLogs.Queries.GetStudentTripLogsByVanId;

public record GetStudentTripLogsByVanIdQuery(Guid VanId) : IRequest<IEnumerable<StudentTripLog>>;

public class GetStudentTripLogsByVanIdQueryHandler : IRequestHandler<GetStudentTripLogsByVanIdQuery, IEnumerable<StudentTripLog>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetStudentTripLogsByVanIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<StudentTripLog>> Handle(GetStudentTripLogsByVanIdQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Repository<StudentTripLog>()
            .GetQueryable()
            .Where(x => x.VanId == request.VanId)
            .OrderByDescending(x => x.BoardingTime)
            .ToListAsync(cancellationToken);
    }
} 