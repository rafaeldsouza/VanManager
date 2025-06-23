using System.Security.Claims;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;

namespace VanManager.API.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            Guid userId;
            if (_httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated != true ||
                               !Guid.TryParse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), out userId))
            {
                return null;
            }
            return userId;
        }
    }

    public string? UserName => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    public string? UserRole
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
            //FindAll(ClaimTypes.Role)
        }
    }

    public string? Email => throw new NotImplementedException();

    public AppUser AppUser => throw new NotImplementedException();

    public IEnumerable<string> GetRoles()
    {
        return _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role).Select(x=>x.Value).ToList() ?? new List<string>();
    }

    public bool IsInRole(string role)
    {
        return _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role).Any(x => x.Value.Equals(role, StringComparison.OrdinalIgnoreCase)) ?? false;
    }
}