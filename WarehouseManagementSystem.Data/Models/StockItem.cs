using System;

namespace WarehouseManagementSystem.Data.Models;

// removed Refused Bequest by removing BaseEntity since we dont need its id
public class StockItem : IAuditableEntity
{
    public int WarehouseId { get; set; }
    public virtual Warehouse Warehouse { get; set; }

    public int ItemId { get; set; }
    public virtual Item Item { get; set; }

    public int Quantity { get; set; }
    public DateTime ProductionDate { get; set; }
    public DateTime ExpirationDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}