using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.Schools.Queries.GetSchoolById;

public record GetSchoolByIdQuery(Guid Id) : IRequest<School>;

public class GetSchoolByIdQueryHandler : IRequestHandler<GetSchoolByIdQuery, School>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetSchoolByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<School> Handle(GetSchoolByIdQuery request, CancellationToken cancellationToken)
    {
        var school = await _unitOfWork.Repository<School>().GetByIdAsync(request.Id);

        if (school == null)
        {
            throw new NotFoundException("Escola não encontrada");
        }

        return school;
    }
}