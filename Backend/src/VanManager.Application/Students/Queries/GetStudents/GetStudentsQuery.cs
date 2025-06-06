using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.Students.Queries.GetStudents;

public record GetStudentsQuery : IRequest<IEnumerable<Student>>;

public class GetStudentsQueryHandler : IRequestHandler<GetStudentsQuery, IEnumerable<Student>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetStudentsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Student>> Handle(GetStudentsQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Repository<Student>().GetAllAsync();
    }
}