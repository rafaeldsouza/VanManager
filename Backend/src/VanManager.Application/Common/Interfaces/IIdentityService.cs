using VanManager.Application.Common.Models;

namespace VanManager.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<string?> GetUserNameAsync(Guid userId);

    Task<bool> IsInRoleAsync(Guid userId, string role);

    Task<bool> AuthorizeAsync(Guid userId, string policyName);

    Task<(Result Result, Guid UserId)> CreateUserAsync(string userName, string password);

    Task<Result> DeleteUserAsync(Guid userId);

    Task<Result> UpdateUserAsync(Guid userId, string? userName = null, string? email = null, string? fullName = null, string? phoneNumber = null);
    
     /// <summary>
    /// Valida as credenciais do usuário.
    /// </summary>
    /// <param name="email">E-mail do usuário</param>
    /// <param name="password">Senha do usuário</param>
    /// <returns>Guid do usuário se válido, Guid.Empty se inválido</returns>
    Task<Guid?> ValidateUserAsync(string email, string password);

    /// <summary>
    /// Verifica se um usuário existe com base no e-mail fornecido.
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    Task<bool> UserExistsAsync(string email);
}