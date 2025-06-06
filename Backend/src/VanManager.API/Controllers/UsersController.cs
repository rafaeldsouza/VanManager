using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using VanManager.API.Models;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Constants;

namespace VanManager.API.Controllers;

[Authorize(Policy = Permissions.ViewUsers)]
public class UsersController : ApiControllerBase
{
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        IIdentityService identityService,
        ICurrentUserService currentUserService,
        ILogger<UsersController> logger)
    {
        _identityService = identityService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        // In a real implementation, this would fetch users from the database
        // For now, return a placeholder response
        return Ok(new { message = "This endpoint would return a list of users" });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var userName = await _identityService.GetUserNameAsync(id);

        if (userName == null)
        {
            return NotFound();
        }

        return Ok(new { id, userName });
    }

    [HttpPost]
    [Authorize(Policy = Permissions.CreateUsers)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var (result, userId) = await _identityService.CreateUserAsync(request.Email, request.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        // Update user with additional information
        var updateResult = await _identityService.UpdateUserAsync(
            userId,
            null,  // userName - we keep the email as the userName
            null,  // email - already set during creation
            request.FullName,
            request.PhoneNumber
        );

        if (!updateResult.Succeeded)
        {
            return BadRequest(updateResult.Errors);
        }

        return CreatedAtAction(nameof(GetUser), new { id = userId }, new { userId });
    }

    [HttpPut("{id}")]
    [Authorize(Policy = Permissions.EditUsers)]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
    {
        var result = await _identityService.UpdateUserAsync(
            id,
            request.Email,
            request.Email,  // email and userName are the same
            request.FullName,
            request.PhoneNumber
        );

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = Permissions.DeleteUsers)]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        // Don't allow deletion of the current user
        if (_currentUserService.UserId == id)
        {
            return BadRequest(new { error = "Você não pode excluir sua própria conta" });
        }

        var result = await _identityService.DeleteUserAsync(id);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return NoContent();
    }
}