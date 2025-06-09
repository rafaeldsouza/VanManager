using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.StudentTripLogs.Queries.GetStudentTripLogById;

public record GetStudentTripLogByIdQuery(Guid Id) : IRequest<StudentTripLog>;

public class GetStudentTripLogByIdQueryHandler : IRequestHandler<GetStudentTripLogByIdQuery, StudentTripLog>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetStudentTripLogByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<StudentTripLog> Handle(GetStudentTripLogByIdQuery request, CancellationToken cancellationToken)
    {
        var studentTripLog = await _unitOfWork.Repository<StudentTripLog>().GetByIdAsync(request.Id);

        if (studentTripLog == null)
        {
            throw new NotFoundException("Registro de viagem do estudante não encontrado");
        }

        return studentTripLog;
    }
} 