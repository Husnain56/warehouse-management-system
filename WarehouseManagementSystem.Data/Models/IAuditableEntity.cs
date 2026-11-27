// Models/IAuditableEntity.cs
namespace WarehouseManagementSystem.Data.Models;

public interface IAuditableEntity
{
    DateTime CreatedAt { get; set; }
    DateTime? UpdatedAt { get; set; }
}