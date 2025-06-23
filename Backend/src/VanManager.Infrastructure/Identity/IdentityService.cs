using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VanManager.Application.Common.Interfaces;
using VanManager.Application.Common.Models;
using VanManager.Domain.Constants;
using VanManager.Domain.Entities;

namespace VanManager.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<AppUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;
    private readonly ILogger<IdentityService> _logger;

    public IdentityService(
        UserManager<AppUser> userManager,
        IUserClaimsPrincipalFactory<AppUser> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService,
        ILogger<IdentityService> logger)
    {
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
        _logger = logger;
    }

    public async Task<string?> GetUserNameAsync(Guid userId)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
        return user?.UserName;
    }

    public async Task<(Result Result, Guid UserId)> CreateUserAsync(string userName, string password)
    {
        var user = new AppUser
        {
            UserName = userName,
            Email = userName,
            CreatedAt = DateTime.UtcNow,
            FullName = " ",
        };

        var result = await _userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, Roles.FleetOwner);
            _logger.LogInformation("Created FleetOwner user {Email}", userName);
        }
        return (result.ToApplicationResult(), user.Id);
    }

    public async Task<bool> IsInRoleAsync(Guid userId, string role)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId);
        return user != null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(Guid userId, string policyName)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return false;
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);
        var result = await _authorizationService.AuthorizeAsync(principal, policyName);
        return result.Succeeded;
    }

    public async Task<Result> DeleteUserAsync(Guid userId)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId);
        return user != null ? await DeleteUserAsync(user) : Result.Success();
    }

    public async Task<Result> UpdateUserAsync(Guid userId, string? userName = null, string? email = null, string? fullName = null, string? phoneNumber = null)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return Result.Failure($"User with ID {userId} not found.");
        }

        if (!string.IsNullOrWhiteSpace(userName) && userName != user.UserName)
        {
            user.UserName = userName;
        }

        if (!string.IsNullOrWhiteSpace(email) && email != user.Email)
        {
            user.Email = email;
        }

        if (!string.IsNullOrWhiteSpace(fullName) && fullName != user.FullName)
        {
            user.FullName = fullName;
        }

        if (!string.IsNullOrWhiteSpace(phoneNumber) && phoneNumber != user.PhoneNumber)
        {
            user.PhoneNumber = phoneNumber;
        }

        var result = await _userManager.UpdateAsync(user);
        return result.ToApplicationResult();
    }

    private async Task<Result> DeleteUserAsync(AppUser user)
    {
        var result = await _userManager.DeleteAsync(user);
        return result.ToApplicationResult();
    }

    public async Task<Guid?> ValidateUserAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return Guid.Empty;

        var isValid = await _userManager.CheckPasswordAsync(user, password);
        if (!isValid)
            return Guid.Empty;

        return user.Id;
    }

    //crete method to validate if user exists by email
    public async Task<bool> UserExistsAsync(string email)
    {
        return await _userManager.Users.AnyAsync(u => u.Email == email);
    }


}

public static class IdentityResultExtensions
{
    public static Result ToApplicationResult(this IdentityResult result)
    {
        return result.Succeeded
            ? Result.Success()
            : Result.Failure(result.Errors.Select(e => e.Description));
    }
}