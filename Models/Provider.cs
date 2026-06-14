using System.ComponentModel.DataAnnotations;

namespace ProviderAssignmentStarter.Models
{
    /// <summary>
    /// Represents a service provider managed by the state.
    /// Implements soft-delete for audit and compliance.
    /// </summary>
    public class Provider
    {
        /// <summary>
        /// Unique identifier for the provider (Primary Key)
        /// </summary>
        public int ProviderId { get; set; }

        /// <summary>
        /// Display name of the provider; searchable
        /// </summary>
        [Required(ErrorMessage = "Provider name is required.")]
        [StringLength(255, MinimumLength = 3, ErrorMessage = "Provider name must be between 3 and 255 characters.")]
        public string ProviderName { get; set; } = string.Empty;

        /// <summary>
        /// County in which the provider operates (e.g., Fulton, DeKalb)
        /// </summary>
        [Required(ErrorMessage = "County is required.")]
     
        public string County { get; set; } = string.Empty;

        /// <summary>
        /// Business or lifecycle status of the provider
        /// Example values: Active, Inactive, Pending
        /// </summary>
        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression(@"^(Active|Inactive|Pending)$", ErrorMessage = "Status must be Active, Inactive, or Pending.")]
        [StringLength(50)]
        public string Status { get; set; } = "Active";

        /// <summary>
        /// Date the provider record was created; used for audit and tracking
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Soft-delete flag: true if provider has been deleted (logically)
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// Timestamp when the provider was soft-deleted; null if not deleted
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Navigation property: collection of licenses associated with this provider
        /// </summary>
        public ICollection<License> Licenses { get; set; } = new List<License>();
    }
}
