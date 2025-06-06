using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.StudentAbsences.Queries.GetStudentAbsenceById;

public record GetStudentAbsenceByIdQuery(Guid Id) : IRequest<StudentAbsence>;

public class GetStudentAbsenceByIdQueryHandler : IRequestHandler<GetStudentAbsenceByIdQuery, StudentAbsence>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetStudentAbsenceByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<StudentAbsence> Handle(GetStudentAbsenceByIdQuery request, CancellationToken cancellationToken)
    {
        var studentAbsence = await _unitOfWork.Repository<StudentAbsence>().GetByIdAsync(request.Id);

        if (studentAbsence == null)
        {
            throw new NotFoundException("Ausência do estudante não encontrada");
        }

        return studentAbsence;
    }
}