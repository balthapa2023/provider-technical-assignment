using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ProviderAssignmentStarter.Models
{
    
    public class ProviderLicense
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public string LicenseNumber { get; set; } = null!;
        public string LicenseStatus { get; set; } = "Active";
        public DateTime? ExpirationDate { get; set; }
        public Provider Provider { get; set; } = null!;
    }
}
