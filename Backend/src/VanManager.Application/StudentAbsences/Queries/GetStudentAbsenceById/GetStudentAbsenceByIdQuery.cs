using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.StudentAbsences.Queries.GetStudentAbsenceById;

public record GetStudentAbsenceByIdQuery(Guid Id) : IRequest<IList<StudentAbsence>>;

public class GetStudentAbsenceByIdQueryHandler : IRequestHandler<GetStudentAbsenceByIdQuery, IList<StudentAbsence>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetStudentAbsenceByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IList<StudentAbsence>> Handle(GetStudentAbsenceByIdQuery request, CancellationToken cancellationToken)
    {
        var studentAbsence = _unitOfWork.Repository<StudentAbsence>().GetQueryable().Where(x=>x.StudentId == request.Id).ToList();

        if (studentAbsence == null)
        {
            throw new NotFoundException("Ausência do estudante não encontrada");
        }

        return studentAbsence;
    }
}