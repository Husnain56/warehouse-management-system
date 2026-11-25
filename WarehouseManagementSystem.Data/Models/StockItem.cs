namespace WarehouseManagementSystem.Data.Models;

public class StockItem : StockBatchEntity
{
    public int WarehouseId { get; set; }
    public virtual Warehouse Warehouse { get; set; }
}