using System.Linq.Expressions;
using VanManager.Domain.Entities;
using VanManager.Domain.Constants;

namespace VanManager.Domain.Specifications;

public static class AccessControlSpecifications
{
    // Especificações para Van
    public static Expression<Func<Van, bool>> CanBeAccessedByDriverForVan(AppUser driver) =>
        van => van.DriverId == driver.Id && van.IsActive;

    // Especificações para Route
    public static Expression<Func<Route, bool>> CanBeAccessedByDriverForRoute(AppUser driver) =>
        route => route.Van.DriverId == driver.Id && route.IsActive;

    // Especificações para Student
    public static Expression<Func<Student, bool>> CanBeAccessedByDriverForStudent(AppUser driver) =>
        student => student.VanId == driver.VanId && student.IsActive;

    // Especificações para StudentTripLog
    public static Expression<Func<StudentTripLog, bool>> CanBeAccessedByDriverForTripLog(AppUser driver) =>
        log => log.Van.DriverId == driver.Id;

    // Especificações para StudentAbsence
    public static Expression<Func<StudentAbsence, bool>> CanBeAccessedByDriverForAbsence(AppUser driver) =>
        absence => absence.Student.VanId == driver.VanId;

    // Especificações para Parent
    public static Expression<Func<Student, bool>> CanBeAccessedByParentForStudent(AppUser parent) =>
        student => student.Guardians.Any(g => g.Id == parent.Id) && student.IsActive;

    public static Expression<Func<StudentTripLog, bool>> CanBeAccessedByParentForTripLog(AppUser parent) =>
        log => log.Student.Guardians.Any(g => g.Id == parent.Id);

    public static Expression<Func<StudentAbsence, bool>> CanBeAccessedByParentForAbsence(AppUser parent) =>
        absence => absence.Student.Guardians.Any(g => g.Id == parent.Id);

    // Especificações para FleetOwner
    public static Expression<Func<Van, bool>> CanBeAccessedByFleetOwnerForVan(AppUser fleetOwner) =>
        van => van.FleetId == fleetOwner.FleetId && van.IsActive;

    public static Expression<Func<Route, bool>> CanBeAccessedByFleetOwnerForRoute(AppUser fleetOwner) =>
        route => route.Van.FleetId == fleetOwner.FleetId && route.IsActive;

    public static Expression<Func<Student, bool>> CanBeAccessedByFleetOwnerForStudent(AppUser fleetOwner) =>
        student => student.FleetId == fleetOwner.FleetId && student.IsActive;

    public static Expression<Func<StudentTripLog, bool>> CanBeAccessedByFleetOwnerForTripLog(AppUser fleetOwner) =>
        log => log.Van.FleetId == fleetOwner.FleetId;

    public static Expression<Func<StudentAbsence, bool>> CanBeAccessedByFleetOwnerForAbsence(AppUser fleetOwner) =>
        absence => absence.Student.FleetId == fleetOwner.FleetId;
} 