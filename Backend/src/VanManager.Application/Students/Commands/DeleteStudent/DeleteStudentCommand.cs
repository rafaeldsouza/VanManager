using MediatR;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;

namespace VanManager.Application.Students.Commands.DeleteStudent;

public record DeleteStudentCommand(Guid Id) : IRequest;

public class DeleteStudentCommandHandler : IRequestHandler<DeleteStudentCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<DeleteStudentCommandHandler> _logger;

    public DeleteStudentCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<DeleteStudentCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await _unitOfWork.Repository<Student>().GetByIdAsync(request.Id);

        if (student == null)
        {
            throw new NotFoundException("Estudante não encontrado");
        }

        await _unitOfWork.Repository<Student>().DeleteAsync(student);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Estudante {StudentId} excluído por {UserId}", request.Id, _currentUserService.UserId);
    }
}