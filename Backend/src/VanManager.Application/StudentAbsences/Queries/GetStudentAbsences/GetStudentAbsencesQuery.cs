using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.StudentAbsences.Queries.GetStudentAbsences;

public record GetStudentAbsencesQuery : IRequest<IEnumerable<StudentAbsence>>;

public class GetStudentAbsencesQueryHandler : IRequestHandler<GetStudentAbsencesQuery, IEnumerable<StudentAbsence>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetStudentAbsencesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<StudentAbsence>> Handle(GetStudentAbsencesQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Repository<StudentAbsence>().GetAllAsync();
    }
}