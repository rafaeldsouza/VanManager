using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.BusinessRules;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.StudentAbsences.Commands.CreateStudentAbsence;

public record CreateStudentAbsenceCommand(
    Guid StudentId,
    DateTime Date,
    string? Justification = null
) : IRequest<Guid>;

public class CreateStudentAbsenceCommandHandler : IRequestHandler<CreateStudentAbsenceCommand, Guid>
{
    
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CreateStudentAbsenceCommandHandler> _logger;
    private readonly IUnitOfWork unitOfWork;

    public CreateStudentAbsenceCommandHandler(       
        ICurrentUserService currentUserService,
        ILogger<CreateStudentAbsenceCommandHandler> logger, IUnitOfWork unitOfWork)
    {
        
        _currentUserService = currentUserService;
        _logger = logger;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateStudentAbsenceCommand request, CancellationToken cancellationToken)
    {
        var student = await unitOfWork.Repository<Student>().GetByIdAsync(request.StudentId);
        if (student == null)
        {
            throw new NotFoundException($"Student with ID {request.StudentId} not found");
        }

        
        if (_currentUserService.IsAuthenticated)
        {
            throw new UnauthorizedException("User not authenticated");
        }
        if (!_currentUserService.UserId.HasValue)
        {
            throw new UnauthorizedException("User not authenticated");
        }
        var currentUser = await unitOfWork.Repository<AppUser>().GetByIdAsync(_currentUserService.UserId.Value);
        if (currentUser == null)
        {
            throw new UnauthorizedException("User not found");
        }
        // Aplica as regras de negócio
        if (!StudentAbsenceRules.CanCreateAbsence(_currentUserService.GetRoles(), currentUser, student))
        {
            throw new ForbiddenAccessException("User is not authorized to create absence records for this student");
        }

        if (!StudentAbsenceRules.IsValidAbsenceDate(request.Date))
        {
            throw new ValidationException("Invalid absence date. Date must be within the last 7 days and cannot be in the future");
        }

        if (StudentAbsenceRules.HasOverlappingAbsence(student, request.Date))
        {
            throw new ValidationException("There is already an absence record for this date");
        }

        if (StudentAbsenceRules.HasOverlappingTrip(student, request.Date))
        {
            throw new ValidationException("Cannot create absence record because there is a trip record for this date");
        }

        var absence = new StudentAbsence
        {
            Id = Guid.NewGuid(),
            StudentId = request.StudentId,
            Date = request.Date,
            Justification = request.Justification,
            IsJustified = false,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = currentUser.Id
        };
        await unitOfWork.Repository<StudentAbsence>().AddAsync( absence );        
        await unitOfWork.SaveChangesAsync();

        _logger.LogInformation(
            "Created absence record {AbsenceId} for student {StudentId} by user {UserId}",
            absence.Id,
            request.StudentId,
            currentUser.Id);

        return absence.Id;
    }
}