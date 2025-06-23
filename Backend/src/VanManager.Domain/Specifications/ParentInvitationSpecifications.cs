using System.Linq.Expressions;
using VanManager.Domain.Entities;

namespace VanManager.Domain.Specifications;

public static class ParentInvitationSpecifications
{
    public static Expression<Func<ParentInvitation, bool>> IsValid => 
        invitation => !invitation.IsAccepted && invitation.ExpiresAt > DateTime.UtcNow;
    
    public static Expression<Func<ParentInvitation, bool>> HasValidToken(string token) =>
        invitation => invitation.InvitationToken == token && 
                     !invitation.IsAccepted && 
                     invitation.ExpiresAt > DateTime.UtcNow;
    
    public static Expression<Func<ParentInvitation, bool>> ForStudent(Guid studentId) =>
        invitation => invitation.StudentId == studentId;
    
    public static Expression<Func<ParentInvitation, bool>> ForEmail(string email) =>
        invitation => invitation.Email == email;
    
    public static Expression<Func<ParentInvitation, bool>> InvitedBy(Guid userId) =>
        invitation => invitation.InvitedByUserId == userId;
    
    public static Expression<Func<ParentInvitation, bool>> IsPending => 
        invitation => !invitation.IsAccepted;
    
    public static Expression<Func<ParentInvitation, bool>> IsExpired => 
        invitation => invitation.ExpiresAt <= DateTime.UtcNow;
    
    public static Expression<Func<ParentInvitation, bool>> IsAccepted => 
        invitation => invitation.IsAccepted;
    
    public static Expression<Func<ParentInvitation, bool>> CanBeResent => invitation =>
        !invitation.IsAccepted && 
        invitation.ExpiresAt <= DateTime.UtcNow.AddDays(7); // Permite reenvio se expirou há menos de 7 dias
} 