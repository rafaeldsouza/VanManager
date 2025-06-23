using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using VanManager.Application.Common.Exceptions;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Infrastructure.Data;

namespace VanManager.Infrastructure.Identity;

public class TokenService : ITokenService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IDateTimeProvider _dateTimeProvider;

    public TokenService(
        UserManager<AppUser> userManager,
        ApplicationDbContext context,
        IConfiguration configuration,
        IDateTimeProvider dateTimeProvider)
    {
        _userManager = userManager;
        _context = context;
        _configuration = configuration;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            throw new NotFoundException("Usu�rio n�o encontrado");
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        var accessToken = await GenerateAccessToken(user, userRoles);
        var refreshToken = GenerateRefreshToken();

        // Save refresh token
        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = _dateTimeProvider.UtcNow.AddDays(7)
        };

        await _context.RefreshTokens.AddAsync(refreshTokenEntity);
        await _context.SaveChangesAsync();

        return (accessToken, refreshToken);
    }

    public async Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string refreshToken)
    {
        var savedRefreshToken = await _context.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == refreshToken);

        if (savedRefreshToken == null || !savedRefreshToken.IsActive)
        {
            throw new ForbiddenAccessException();
        }

        var user = savedRefreshToken.User;
        var userRoles = await _userManager.GetRolesAsync(user);
        var newAccessToken = await GenerateAccessToken(user, userRoles);
        var newRefreshToken = GenerateRefreshToken();

        // Revoke old refresh token
        _context.RefreshTokens.Remove(savedRefreshToken);

        // Save new refresh token
        var refreshTokenEntity = new RefreshToken
        {
            Token = newRefreshToken,
            UserId = user.Id,
            ExpiresAt = _dateTimeProvider.UtcNow.AddDays(7)
        };

        await _context.RefreshTokens.AddAsync(refreshTokenEntity);
        await _context.SaveChangesAsync();

        return (newAccessToken, newRefreshToken);
    }

    public async Task RevokeTokenAsync(string refreshToken)
    {
        var savedRefreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(r => r.Token == refreshToken);

        if (savedRefreshToken != null)
        {
            _context.RefreshTokens.Remove(savedRefreshToken);
            await _context.SaveChangesAsync();
        }
    }

    private async Task<string> GenerateAccessToken(AppUser user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new("fullName", user.FullName)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["JwtSettings:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured")));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = _dateTimeProvider.UtcNow.AddMinutes(
            Convert.ToDouble(_configuration["JwtSettings:ExpiryMinutes"] ?? "60"));

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}