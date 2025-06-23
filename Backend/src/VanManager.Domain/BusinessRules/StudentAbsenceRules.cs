using VanManager.Domain.Constants;
using VanManager.Domain.Entities;
using VanManager.Domain.Specifications;

namespace VanManager.Domain.BusinessRules;

public static class StudentAbsenceRules
{
    public static bool CanCreateAbsence(IEnumerable<string> roles, AppUser user, Student student)
    {
        if (AccessControlRules.IsAdmin(roles)) return true;
        if (AccessControlRules.IsFleetOwner(roles) && student.FleetId == user.FleetId) return true;
        if (AccessControlRules.IsDriver(roles) && student.VanId == user.VanId) return true;
        return false;
    }
    
    public static bool CanUpdateAbsence(IEnumerable<string> roles, AppUser user, StudentAbsence absence)
    {
        if (AccessControlRules.IsAdmin(roles)) return true;
        if (AccessControlRules.IsFleetOwner(roles) && absence.Student.FleetId == user.FleetId) return true;
        if (AccessControlRules.IsDriver(roles) && absence.Student.VanId == user.VanId) return true;
        if (AccessControlRules.IsParent(roles) && absence.Student.Guardians.Any(g => g.Id == user.Id)) return true;
        return false;
    }
    
    public static bool CanDeleteAbsence(IEnumerable<string> roles, AppUser user, StudentAbsence absence)
    {
        if (AccessControlRules.IsAdmin(roles)) return true;
        if (AccessControlRules.IsFleetOwner(roles) && absence.Student.FleetId == user.FleetId) return true;
        return false;
    }
    
    public static bool IsValidAbsenceDate(DateTime date)
    {
        // Verifica se a data não é futura
        if (date > DateTime.UtcNow)
            return false;
            
        // Verifica se a data não é muito antiga (máximo 7 dias)
        var daysSinceDate = (DateTime.UtcNow - date).TotalDays;
        if (daysSinceDate > 7)
            return false;
            
        return true;
    }
    
    public static bool HasOverlappingAbsence(Student student, DateTime date)
    {
        // Verifica se já existe uma falta registrada para esta data
        return student.Absences.Any(a => a.Date.Date == date.Date);
    }
    
    public static bool HasOverlappingTrip(Student student, DateTime date)
    {
        // Verifica se existe algum registro de viagem para esta data
        return student.TripLogs.Any(log =>
            log.BoardingTime.Date == date.Date ||
            log.DropoffTime.Date == date.Date);
    }
    
    public static bool CanJustifyAbsence(IEnumerable<string> roles, AppUser user, StudentAbsence absence)
    {
        if (AccessControlRules.IsAdmin(roles)) return true;
        if (AccessControlRules.IsFleetOwner(roles) && absence.Student.FleetId == user.FleetId) return true;
        if (AccessControlRules.IsParent(roles) && absence.Student.Guardians.Any(g => g.Id == user.Id)) return true;
        return false;
    }
    
    public static bool CanApproveJustification(IEnumerable<string> roles, AppUser user, StudentAbsence absence)
    {
        if (AccessControlRules.IsAdmin(roles)) return true;
        if (AccessControlRules.IsFleetOwner(roles) && absence.Student.FleetId == user.FleetId) return true;
        return false;
    }
}