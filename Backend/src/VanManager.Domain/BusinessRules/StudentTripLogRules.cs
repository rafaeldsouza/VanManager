using VanManager.Domain.Constants;
using VanManager.Domain.Entities;
using VanManager.Domain.Specifications;

namespace VanManager.Domain.BusinessRules;

public static class StudentTripLogRules
{
    public static bool CanCreateTripLog(IEnumerable<string> roles, AppUser user, Student student)
    {
        if (AccessControlRules.IsAdmin(roles)) return true;
        if (AccessControlRules.IsFleetOwner(roles) && student.FleetId == user.FleetId) return true;
        if (AccessControlRules.IsDriver(roles) && student.Van?.DriverId == user.Id) return true;
        return false;
    }
    
    public static bool CanUpdateTripLog(IEnumerable<string> roles, AppUser user, StudentTripLog tripLog)
    {
        if (AccessControlRules.IsAdmin(roles)) return true;
        if (AccessControlRules.IsFleetOwner(roles) && tripLog.Student.FleetId == user.FleetId) return true;
        if (AccessControlRules.IsDriver(roles) && tripLog.Student.Van?.DriverId == user.Id) return true;
        return false;
    }
    
    public static bool CanDeleteTripLog(IEnumerable<string> roles, AppUser user, StudentTripLog tripLog)
    {
        if (AccessControlRules.IsAdmin(roles)) return true;
        if (AccessControlRules.IsFleetOwner(roles) && tripLog.Student.FleetId == user.FleetId) return true;
        return false;
    }
    
    public static bool CanCancelTripLog(IEnumerable<string> roles, AppUser user, StudentTripLog tripLog)
    {
        if (!CanUpdateTripLog(roles, user, tripLog)) return false;
        
        // Só pode cancelar viagens pendentes ou agendadas
        return tripLog.Status is TripStatus.Pending or TripStatus.Scheduled;
    }
    
    public static bool CanCompleteTripLog(IEnumerable<string> roles, AppUser user, StudentTripLog tripLog)
    {
        if (!CanUpdateTripLog(roles, user, tripLog)) return false;
        
        // Só pode completar viagens em andamento
        return tripLog.Status == TripStatus.InProgress;
    }
    
    public static bool CanStartTripLog(IEnumerable<string> roles, AppUser user, StudentTripLog tripLog)
    {
        if (!CanUpdateTripLog(roles, user, tripLog)) return false;
        
        // Só pode iniciar viagens agendadas
        return tripLog.Status == TripStatus.Scheduled;
    }
    
    public static bool CanMarkNoShow(IEnumerable<string> roles, AppUser user, StudentTripLog tripLog)
    {
        if (!CanUpdateTripLog(roles, user, tripLog)) return false;
        
        // Só pode marcar como no-show viagens pendentes ou agendadas
        return tripLog.Status is TripStatus.Pending or TripStatus.Scheduled;
    }
    
    public static bool IsValidBoardingTime(DateTime boardingTime)
    {
        // Verifica se o horário de embarque não é futuro
        if (boardingTime > DateTime.UtcNow)
            return false;
            
        // Verifica se o horário de embarque não é muito antigo (máximo 7 dias)
        var daysSinceBoarding = (DateTime.UtcNow - boardingTime).TotalDays;
        if (daysSinceBoarding > 7)
            return false;
            
        return true;
    }
    
    public static bool IsValidDropoffTime(DateTime boardingTime, DateTime dropoffTime)
    {
        // Verifica se o horário de desembarque é posterior ao embarque
        if (dropoffTime <= boardingTime)
            return false;
            
        // Verifica se o horário de desembarque não é futuro
        if (dropoffTime > DateTime.UtcNow)
            return false;
            
        // Verifica se o tempo entre embarque e desembarque é razoável (máximo 12 horas)
        var tripDuration = (dropoffTime - boardingTime).TotalHours;
        if (tripDuration > 12)
            return false;
            
        return true;
    }
    
    public static bool IsValidLocation(double latitude, double longitude)
    {
        // Verifica se as coordenadas estão dentro de limites razoáveis
        if (latitude < -90 || latitude > 90)
            return false;
            
        if (longitude < -180 || longitude > 180)
            return false;
            
        return true;
    }
    
    public static bool HasOverlappingTrip(Student student, DateTime boardingTime, DateTime dropoffTime)
    {
        // Verifica se existe algum registro que se sobrepõe ao período
        return student.TripLogs.Any(log =>
            (boardingTime >= log.BoardingTime && boardingTime <= log.DropoffTime) ||
            (dropoffTime >= log.BoardingTime && dropoffTime <= log.DropoffTime) ||
            (boardingTime <= log.BoardingTime && dropoffTime >= log.DropoffTime));
    }
    
    public static bool CanChangeStatus(IEnumerable<string> roles, AppUser user, StudentTripLog tripLog, TripStatus newStatus)
    {
        // Verifica se o usuário tem permissão para alterar o status
        if (!AccessControlRules.CanAccessTripLog(roles, user, tripLog))
            return false;
            
        // Verifica se a transição de status é válida
        return (tripLog.Status, newStatus) switch
        {
            // Qualquer status pode ser alterado para Cancelled
            (_, TripStatus.Cancelled) => true,
            
            // Pending pode ser alterado para InProgress
            (TripStatus.Pending, TripStatus.InProgress) => true,
            
            // InProgress pode ser alterado para Completed
            (TripStatus.InProgress, TripStatus.Completed) => true,
            
            // Completed não pode ser alterado
            (TripStatus.Completed, _) => false,
            
            // Cancelled não pode ser alterado
            (TripStatus.Cancelled, _) => false,
            
            // Outras transições não são permitidas
            _ => false
        };
    }

    public static bool IsAdminOrFleetOwner(IEnumerable<string> roles) =>
        AccessControlRules.IsAdmin(roles) || AccessControlRules.IsFleetOwner(roles);

    public static bool IsAdminOrDriver(IEnumerable<string> roles) =>
        AccessControlRules.IsAdmin(roles) || AccessControlRules.IsDriver(roles);

    public static bool IsAdminOrParent(IEnumerable<string> roles) =>
        AccessControlRules.IsAdmin(roles) || AccessControlRules.IsParent(roles);
}