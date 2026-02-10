using System.Security.Claims;

namespace HRMS.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int? GetEmployeeId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst("EmployeeId");
        if (claim != null && int.TryParse(claim.Value, out int employeeId))
        {
            return employeeId;
        }
        return null;
    }

    public static bool IsAdmin(this ClaimsPrincipal user)
    {
        return user.IsInRole("AdminRH");
    }

    public static bool IsManager(this ClaimsPrincipal user)
    {
        return user.IsInRole("Manager");
    }

    public static bool IsEmployee(this ClaimsPrincipal user)
    {
        return user.IsInRole("Employé");
    }
}
