namespace Candidate.API.Models;

public static class ProviderStatuses
{
    public const string Active = "Active";
    public const string Inactive = "Inactive";
    public const string Pending = "Pending";

    public static bool IsValid(string status)
    {
        return status is Active or Inactive or Pending;
    }
}
