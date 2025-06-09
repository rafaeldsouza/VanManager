using MediatR;
using Microsoft.EntityFrameworkCore;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.StudentTripLogs.Queries.GetStudentTripLogsByStudentId;

public record GetStudentTripLogsByStudentIdQuery(Guid StudentId) : IRequest<IEnumerable<StudentTripLog>>;

public class GetStudentTripLogsByStudentIdQueryHandler : IRequestHandler<GetStudentTripLogsByStudentIdQuery, IEnumerable<StudentTripLog>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetStudentTripLogsByStudentIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<StudentTripLog>> Handle(GetStudentTripLogsByStudentIdQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Repository<StudentTripLog>()
            .GetQueryable()
            .Where(x => x.StudentId == request.StudentId)
            .OrderByDescending(x => x.BoardingTime)
            .ToListAsync(cancellationToken);
    }
} 