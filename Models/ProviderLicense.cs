using System.ComponentModel.DataAnnotations;

namespace ProviderAssignmentStarter.Models
{
    public class ProviderLicense
    {
        public int Id { get; set; }

        [Required]
        public int ProviderId { get; set; }

        // Navigation property — NOT required for model binding
        public Provider? Provider { get; set; }

        [Required]
        [StringLength(50)]
        public string LicenseNumber { get; set; }

        [Required]
        public string LicenseStatus { get; set; }

        // IMPORTANT: Must be nullable for model binding to succeed
        [DataType(DataType.Date)]
        public DateTime? ExpirationDate { get; set; }

        // Soft delete fields
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }
}
