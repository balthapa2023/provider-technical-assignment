using System.ComponentModel.DataAnnotations;
namespace ProviderAssignmentStarter.Models;

public class License
{
    public int LicenseId { get; set; }
    public int ProviderId { get; set; }

    [Required]
    [StringLength(100)]
    public string LicenseNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Status { get; set; } = "Active";

    [Required]
    [DataType(DataType.Date)]
    public DateTime ExpirationDate { get; set; }


    public Provider? Provider { get; set; } = null!;

}