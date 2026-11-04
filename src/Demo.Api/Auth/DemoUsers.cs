namespace Demo.Api.Auth;

public sealed record DemoUser(string Name, string TenantId, string Role);

public static class DemoUsers
{
    public static readonly IReadOnlyDictionary<string, DemoUser> All = new Dictionary<string, DemoUser>
    {
        ["alice"] = new("alice", "acme", Roles.Billing),
        ["bob"] = new("bob", "acme", Roles.Viewer),
        ["carol"] = new("carol", "globex", Roles.Admin),
    };
}

public static class Roles
{
    public const string Viewer = "viewer";
    public const string Billing = "billing";
    public const string Admin = "admin";
}

public static class Policies
{
    public const string CanBill = nameof(CanBill);
    public const string CanAdmin = nameof(CanAdmin);
}
