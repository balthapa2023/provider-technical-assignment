using System.ComponentModel.DataAnnotations;
using Candidate.API.Models;

namespace Candidate.API.Contracts;

public sealed class LicenseRequest : IValidatableObject
{
    [Required]
    [StringLength(40, MinimumLength = 2)]
    public string Number { get; set; } = string.Empty;

    [Required]
    public string Status { get; set; } = LicenseStatuses.Active;

    [Required]
    public DateOnly? ExpirationDate { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrWhiteSpace(Status) && !LicenseStatuses.IsEditable(Status))
        {
            yield return new ValidationResult(
                "Status must be Active or Suspended. Expired status is derived from the expiration date.",
                [nameof(Status)]);
        }
    }

    public void Normalize()
    {
        Number = Number.Trim();
        Status = Status.Trim();
    }
}
