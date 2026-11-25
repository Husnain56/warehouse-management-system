using System;

namespace WarehouseManagementSystem.Data.Models;

public abstract class StockBatchEntity : IAuditableEntity
{
    public int ItemId { get; set; }
    public virtual Item Item { get; set; }

    public int Quantity { get; set; }
    public DateTime ProductionDate { get; set; }
    public DateTime ExpirationDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}