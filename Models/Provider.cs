using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProviderAssignmentStarter.Models
{
    public class Provider
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Provider Name")]
        public string ProviderName { get; set; } = null!;

        public string? County { get; set; }

        [Required]
        public string Status { get; set; } = "Active";

        // Audit
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Soft delete
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        public ICollection<ProviderLicense> Licenses { get; set; } = new List<ProviderLicense>();
    }
}
