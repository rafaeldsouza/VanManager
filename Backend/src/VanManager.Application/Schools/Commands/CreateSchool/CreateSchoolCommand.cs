using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

using VanManager.Domain.Entities;

namespace VanManager.Application.Schools.Commands.CreateSchool;

public record CreateSchoolCommand : IRequest<School>
{
    public string Name { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public decimal LocationLat { get; init; }
    public decimal LocationLng { get; init; }
}

public class CreateSchoolCommandHandler : IRequestHandler<CreateSchoolCommand, School>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CreateSchoolCommandHandler> _logger;

    public CreateSchoolCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<CreateSchoolCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<School> Handle(CreateSchoolCommand request, CancellationToken cancellationToken)
    {
        var school = new School
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Address = request.Address,
            LocationLat = request.LocationLat,
            LocationLng = request.LocationLng
        };

        await _unitOfWork.Repository<School>().AddAsync(school);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Escola {SchoolId} criada por {UserId}", school.Id, _currentUserService.UserId);

        return school;
    }
}