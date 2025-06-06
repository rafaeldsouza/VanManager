using MediatR;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.Vans.Queries.GetVans;

public record GetVansQuery : IRequest<IEnumerable<Van>>;

public class GetVansQueryHandler : IRequestHandler<GetVansQuery, IEnumerable<Van>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetVansQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Van>> Handle(GetVansQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Repository<Van>().GetAllAsync();
    }
}