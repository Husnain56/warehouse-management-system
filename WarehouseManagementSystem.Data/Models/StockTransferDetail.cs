namespace WarehouseManagementSystem.Data.Models;

public class StockTransferDetail : StockBatchEntity
{
    public int Id { get; set; }
    public int StockTransferId { get; set; }
    public virtual StockTransfer StockTransfer { get; set; }
}