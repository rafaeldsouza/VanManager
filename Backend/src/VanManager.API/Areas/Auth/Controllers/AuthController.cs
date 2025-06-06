using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VanManager.API.Areas.Auth.Models;
using VanManager.API.Controllers;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Constants;

namespace VanManager.API.Areas.Auth.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ApiControllerBase
{
    private readonly IIdentityService _identityService;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IIdentityService identityService,
        ITokenService tokenService,
        IEmailService emailService,
        ILogger<AuthController> logger)
    {
        _identityService = identityService;
        _tokenService = tokenService;
        _emailService = emailService;
        _logger = logger;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
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

        // Send welcome email
        await _emailService.SendWelcomeEmailAsync(request.Email, request.FullName);

        // Generate tokens
        var (accessToken, refreshToken) = await _tokenService.GenerateTokensAsync(userId);

        return Ok(new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = 3600  // 1 hour in seconds
        });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var userId = await _identityService.ValidateUserAsync(request.Email, request.Password);

        if (userId == null || userId == Guid.Empty)
        {
            return Unauthorized(new { message = "E-mail ou senha inválidos." });
        }



        var (accessToken, refreshToken) = await _tokenService.GenerateTokensAsync(userId.Value);

        return Ok(new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = 3600  // 1 hour in seconds
        });
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var (accessToken, refreshToken) = await _tokenService.RefreshTokenAsync(request.RefreshToken);

            return Ok(new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = 3600  // 1 hour in seconds
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return Unauthorized();
        }
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        // In a real implementation, generate a token and send email
        var resetToken = Guid.NewGuid().ToString();
        await _emailService.SendPasswordResetEmailAsync(request.Email, resetToken);

        return Ok(new { message = "Se uma conta estiver vinculada a este e-mail, um link de redefinição de senha será enviado." });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
    {
        await _tokenService.RevokeTokenAsync(request.RefreshToken);
        return Ok(new { message = "Logout realizado com sucesso" });
    }
}