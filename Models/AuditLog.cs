namespace ProviderAssignmentStarter.Models
{
    /// <summary>
    /// Immutable audit trail entity tracking all create, edit, and delete operations.
    /// Never soft-deleted; maintains permanent record of all changes.
    /// </summary>
    public class AuditLog
    {
        /// <summary>
        /// Unique identifier for the audit log entry
        /// </summary>
        public int AuditLogId { get; set; }

        /// <summary>
        /// Type of entity being audited (e.g., "Provider", "License")
        /// </summary>
        public string EntityType { get; set; } = string.Empty;

        /// <summary>
        /// Primary key of the entity being audited
        /// </summary>
        public int EntityId { get; set; }

        /// <summary>
        /// Action performed (e.g., "Create", "Edit", "Delete")
        /// </summary>
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// User identifier or name who performed the action
        /// </summary>
        public string? UserId { get; set; } = "System";

        /// <summary>
        /// Timestamp when the action was performed (UTC)
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// JSON representation of old values (for Edit and Delete actions)
        /// </summary>
        public string? OldValues { get; set; }

        /// <summary>
        /// JSON representation of new values (for Create and Edit actions)
        /// </summary>
        public string? NewValues { get; set; }

        /// <summary>
        /// Optional human-readable description of the change
        /// </summary>
        public string? Description { get; set; }
    }
}
