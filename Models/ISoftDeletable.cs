namespace ProviderAssignmentStarter.Models;

/// <summary>Marker for entities that must never be physically deleted.</summary>
public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
    DateTime? DeletedDate { get; set; }
    string? DeletedBy { get; set; }
}
