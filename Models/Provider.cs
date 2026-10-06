using System.ComponentModel.DataAnnotations;
namespace ProviderAssignmentStarter.Models;

//This represents a provider in the system.
//Entity framework core will use this class to create/map the providers table

public class Provider
{
    //ProviderId is the primary key and EF Core recognizes it as the primary key
    public int ProviderId {  get; set; }

    [Required]
    [StringLength(200)] 
    public string Name { get; set; } = string.Empty;
    [Required]
    [StringLength(100)]
    public string County { get; set; } = string.Empty;
    [Required]
    [StringLength(50)] 
    public string Status { get; set; } = string.Empty;

    public bool IsDeleted { get; set; } = false;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? DeletedDate { get; set; }

    public ICollection<License> Licenses { get; set; } = new List<License>();



}