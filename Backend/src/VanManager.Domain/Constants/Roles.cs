namespace VanManager.Domain.Constants;

public static class Roles
{
    public const string Admin = "Admin";
    public const string FleetOwner = "FleetOwner";
    public const string Driver = "Driver";
    public const string Parent = "Parent";
    
    public static readonly string[] All = new[] { Admin, FleetOwner, Driver, Parent };
}