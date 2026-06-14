using System.ComponentModel.DataAnnotations;

namespace ProviderAssignmentStarter.Models
{
    /// <summary>
    /// Represents a license issued to a provider.
    /// Implements soft-delete for audit and compliance.
    /// </summary>
    public class License
    {
        /// <summary>
        /// Unique identifier for the license (Primary Key)
        /// </summary>
        public int LicenseId { get; set; }

        /// <summary>
        /// Foreign key to the associated provider
        /// </summary>
        [Required(ErrorMessage = "Provider ID is required.")]
        public int ProviderId { get; set; }

        /// <summary>
        /// Business-recognizable license identifier (usually unique per provider)
        /// </summary>
        [Required(ErrorMessage = "License number is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "License number must be between 3 and 100 characters.")]
        public string LicenseNumber { get; set; } = string.Empty;

        /// <summary>
        /// Current status of the license
        /// Example values: Active, Expired, Suspended
        /// </summary>
        [Required(ErrorMessage = "License status is required.")]
        [RegularExpression(@"^(Active|Expired|Suspended)$", ErrorMessage = "License status must be Active, Expired, or Suspended.")]
        [StringLength(50)]
        public string LicenseStatus { get; set; } = "Active";

        /// <summary>
        /// Date on which the license expires; used to determine license validity
        /// </summary>
        [Required(ErrorMessage = "Expiration date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Expiration date must be a valid date.")]
        [Future(ErrorMessage = "Expiration date must be in the future.")]
        public DateTime ExpirationDate { get; set; }

        /// <summary>
        /// Date the license record was created; used for audit and tracking
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Soft-delete flag: true if license has been deleted (logically)
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// Timestamp when the license was soft-deleted; null if not deleted
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Navigation property: reference to the associated provider
        /// </summary>
        public Provider? Provider { get; set; }
    }

    /// <summary>
    /// Custom validation attribute to ensure date is in the future
    /// </summary>
    public class FutureAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime date)
            {
                return date > DateTime.UtcNow;
            }
            return true;
        }
    }
}
