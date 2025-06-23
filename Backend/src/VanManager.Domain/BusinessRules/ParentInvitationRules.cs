using VanManager.Domain.Constants;
using VanManager.Domain.Entities;
using VanManager.Domain.Specifications;

namespace VanManager.Domain.BusinessRules;

public static class ParentInvitationRules
{
    public static bool CanCreateInvitation(IEnumerable<string> roles, AppUser user, Student student)
    {
        if (AccessControlRules.IsAdmin(roles)) return true;
        if (AccessControlRules.IsFleetOwner(roles) && student.FleetId == user.FleetId) return true;
        return false;
    }
    
    public static bool CanAcceptInvitation(AppUser user, ParentInvitation invitation)
    {
        if (!invitation.IsValid) return false;
        if (user.Email != invitation.Email) return false;
        return true;
    }
    
    public static bool CanResendInvitation(IEnumerable<string> roles, AppUser user, ParentInvitation invitation)
    {
        if (!CanCreateInvitation(roles, user, invitation.Student)) return false;
        
        // Só pode reenviar convites expirados nos últimos 7 dias
        var daysSinceExpiration = (DateTime.UtcNow - invitation.ExpiresAt).TotalDays;
        return daysSinceExpiration <= 7;
    }
    
    public static bool HasReachedMaxGuardians(Student student)
    {
        // Limite de 4 responsáveis por estudante
        return student.Guardians.Count >= 4;
    }
    
    public static bool IsAlreadyGuardian(AppUser user, Student student)
    {
        return student.Guardians.Any(g => g.Id == user.Id);
    }
    
    public static bool HasPendingInvitation(string email, Student student)
    {
        return student.ParentInvitations.Any(i => 
            i.Email == email && 
            !i.IsAccepted && 
            i.ExpiresAt > DateTime.UtcNow);
    }
    
    public static bool CanRemoveParent(IEnumerable<string> roles, AppUser user, Student student, AppUser parentToRemove)
    {
        // Verifica se o usuário tem permissão para remover pais
        if (!AccessControlRules.CanAccessStudent(roles, user, student))
            return false;
            
        // Verifica se o estudante tem mais de um responsável
        if (student.Guardians.Count <= 1)
            return false;
            
        // Verifica se o responsável a ser removido está realmente associado ao estudante
        if (!student.Guardians.Any(g => g.Id == parentToRemove.Id))
            return false;
            
        return true;
    }
}