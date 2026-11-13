namespace Candidate.API.Models;

public static class LicenseStatuses
{
    public const string Active = "Active";
    public const string Expired = "Expired";
    public const string Suspended = "Suspended";

    public static bool IsEditable(string status)
    {
        return status is Active or Suspended;
    }
}
