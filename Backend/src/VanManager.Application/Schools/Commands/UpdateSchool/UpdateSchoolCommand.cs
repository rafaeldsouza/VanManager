using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.Schools.Commands.UpdateSchool;

public record UpdateSchoolCommand : IRequest
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public decimal LocationLat { get; init; }
    public decimal LocationLng { get; init; }
}

public class UpdateSchoolCommandHandler : IRequestHandler<UpdateSchoolCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UpdateSchoolCommandHandler> _logger;

    public UpdateSchoolCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<UpdateSchoolCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task Handle(UpdateSchoolCommand request, CancellationToken cancellationToken)
    {
        var school = await _unitOfWork.Repository<School>().GetByIdAsync(request.Id);

        if (school == null)
        {
            throw new NotFoundException("Escola não encontrada");
        }

        school.Name = request.Name;
        school.Address = request.Address;
        school.LocationLat = request.LocationLat;
        school.LocationLng = request.LocationLng;

        await _unitOfWork.Repository<School>().UpdateAsync(school);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Escola {SchoolId} atualizada por {UserId}", request.Id, _currentUserService.UserId);
    }
}