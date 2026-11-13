using System.ComponentModel.DataAnnotations;
using Candidate.API.Models;

namespace Candidate.API.Contracts;

public sealed class ProviderRequest : IValidatableObject
{
    [Required]
    [StringLength(120, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(80, MinimumLength = 2)]
    public string County { get; set; } = string.Empty;

    [Required]
    public string Status { get; set; } = ProviderStatuses.Pending;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrWhiteSpace(Status) && !ProviderStatuses.IsValid(Status))
        {
            yield return new ValidationResult(
                "Status must be Active, Inactive, or Pending.",
                [nameof(Status)]);
        }
    }

    public void Normalize()
    {
        Name = Name.Trim();
        County = County.Trim();
        Status = Status.Trim();
    }
}
