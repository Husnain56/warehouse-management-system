namespace WarehouseManagementSystem.Data.Models;

public class SupplyOrderDetail : StockBatchEntity
{
    public int SupplyOrderId { get; set; }
    public virtual SupplyOrder SupplyOrder { get; set; }
}