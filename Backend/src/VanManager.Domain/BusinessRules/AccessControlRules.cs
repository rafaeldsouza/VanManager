using Microsoft.AspNetCore.Identity;
using VanManager.Domain.Constants;
using VanManager.Domain.Entities;
using VanManager.Domain.Specifications;

namespace VanManager.Domain.BusinessRules;

public static class AccessControlRules
{
    #region Role Checks
    
    public static bool IsAdmin(IEnumerable<string> roles) => roles.Contains(Roles.Admin);
    public static bool IsFleetOwner(IEnumerable<string> roles) => roles.Contains(Roles.FleetOwner);
    public static bool IsDriver(IEnumerable<string> roles) => roles.Contains(Roles.Driver);
    public static bool IsParent(IEnumerable<string> roles) => roles.Contains(Roles.Parent);

    public static bool IsAdminOrFleetOwner(IEnumerable<string> roles) =>
        IsAdmin(roles) || IsFleetOwner(roles);

    public static bool IsAdminOrDriver(IEnumerable<string> roles) =>
        IsAdmin(roles) || IsDriver(roles);

    public static bool IsAdminOrParent(IEnumerable<string> roles) =>
        IsAdmin(roles) || IsParent(roles);

    #endregion

    #region Fleet Access

    public static bool CanAccessFleet(IEnumerable<string> roles, AppUser user, Fleet fleet)
    {
        if (IsAdmin(roles)) return true;
        if (IsFleetOwner(roles)) return fleet.OwnerUserId == user.Id;
        if (IsDriver(roles)) return fleet.Vans.Any(v => v.DriverId == user.Id);
        return false;
    }

    public static bool CanManageFleet(IEnumerable<string> roles) =>
        IsAdminOrFleetOwner(roles);

    public static bool CanManageFleetSubscription(IEnumerable<string> roles) =>
        IsAdminOrFleetOwner(roles);

    #endregion

    #region Van Access

    public static bool CanAccessVan(IEnumerable<string> roles, AppUser user, Van van)
    {
        if (IsAdmin(roles)) return true;
        if (IsFleetOwner(roles) && van.FleetId == user.FleetId) return true;
        if (IsDriver(roles) && van.DriverId == user.Id) return true;
        return false;
    }

    public static bool CanManageVan(IEnumerable<string> roles, AppUser user, Van van)
    {
        if (IsAdmin(roles)) return true;
        if (IsFleetOwner(roles) && van.FleetId == user.FleetId) return true;
        return false;
    }

    #endregion

    #region Route Access

    public static bool CanAccessRoute(IEnumerable<string> roles, AppUser user, Route route)
    {
        if (IsAdmin(roles)) return true;
        if (IsFleetOwner(roles) && route.Van.FleetId == user.FleetId) return true;
        if (IsDriver(roles) && route.Van.DriverId == user.Id) return true;
        return false;
    }

    public static bool CanManageRoute(IEnumerable<string> roles, AppUser user, Route route)
    {
        if (IsAdmin(roles)) return true;
        if (IsFleetOwner(roles) && route.Van.FleetId == user.FleetId) return true;
        return false;
    }

    #endregion

    #region Student Access

    public static bool CanAccessStudent(IEnumerable<string> roles, AppUser user, Student student)
    {
        if (IsAdmin(roles)) return true;
        if (IsFleetOwner(roles) && student.FleetId == user.FleetId) return true;
        if (IsDriver(roles) && student.Van?.DriverId == user.VanId) return true;
        if (IsParent(roles) && student.Guardians.Any(g => g.Id == user.Id)) return true;
        return false;
    }

    public static bool CanManageStudent(IEnumerable<string> roles, AppUser user, Student student)
    {
        if (IsAdmin(roles)) return true;
        if (IsFleetOwner(roles) && student.Fleet.OwnerUserId == user.FleetId) return true;
        return false;
    }

    #endregion

    #region Trip Log Access

    public static bool CanAccessTripLog(IEnumerable<string> roles, AppUser user, StudentTripLog tripLog)
    {
        if (IsAdmin(roles)) return true;
        if (IsFleetOwner(roles) && tripLog.Van.FleetId == user.FleetId) return true;
        if (IsDriver(roles) && tripLog.Van.DriverId == user.Id) return true;
        if (IsParent(roles) && tripLog.Student.Guardians.Any(g => g.Id == user.Id)) return true;
        return false;
    }

    public static bool CanManageTripLog(IEnumerable<string> roles, AppUser user, StudentTripLog tripLog)
    {
        if (IsAdmin(roles)) return true;
        if (IsFleetOwner(roles) && tripLog.Van.FleetId == user.FleetId) return true;
        if (IsDriver(roles) && tripLog.Van.DriverId == user.Id) return true;
        return false;
    }

    #endregion

    #region Absence Access

    public static bool CanAccessAbsence(IEnumerable<string> roles, AppUser user, StudentAbsence absence)
    {
        if (IsAdmin(roles)) return true;
        if (IsFleetOwner(roles) && absence.Student.FleetId == user.FleetId) return true;
        if (IsDriver(roles) && absence.Student.VanId == user.VanId) return true;
        if (IsParent(roles) && absence.Student.Guardians.Any(g => g.Id == user.Id)) return true;
        return false;
    }

    public static bool CanManageAbsence(IEnumerable<string> roles, AppUser user, StudentAbsence absence)
    {
        if (IsAdmin(roles)) return true;
        if (IsFleetOwner(roles) && absence.Student.FleetId == user.FleetId) return true;
        if (IsDriver(roles) && absence.Student.VanId == user.VanId) return true;
        return false;
    }

    public static bool CanJustifyAbsence(IEnumerable<string> roles, AppUser user, StudentAbsence absence)
    {
        if (IsAdmin(roles)) return true;
        if (IsFleetOwner(roles) && absence.Student.FleetId == user.FleetId) return true;
        if (IsParent(roles) && absence.Student.Guardians.Any(g => g.Id == user.Id)) return true;
        return false;
    }

    public static bool CanApproveJustification(IEnumerable<string> roles, AppUser user, StudentAbsence absence)
    {
        if (IsAdmin(roles)) return true;
        if (IsFleetOwner(roles) && absence.Student.FleetId == user.FleetId) return true;
        return false;
    }

    #endregion

    #region User Management

    // public static bool CanManageUsers(AppUser user) =>
    //     IsAdminOrFleetOwner(user);

    // public static bool CanManageDrivers(AppUser user) =>
    //     IsAdminOrFleetOwner(user);

    // public static bool CanManageParents(AppUser user) =>
    //     IsAdminOrFleetOwner(user);

    #endregion
}