using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WarehouseManagementSystem.Data.Models;

namespace WarehouseManagementSystem.Data.Repositories.Interfaces;

public interface IStockItemRepository : IRepository<StockItem>
{
    Task<List<StockItem>> GetStockWithItemAndWarehouseAsync();
    Task<List<StockItem>> GetItemsByWarehousesAsync(List<int> warehouseIds);
    Task<IEnumerable<StockItem>> GetStockItemsWithItemAsync(int warehouseId);
    Task<List<StockItem>> GetStockItemsWithItemAndWarehouseAsync(int warehouseId);
    Task<StockItem> GetStockItemAsync(int warehouseId, int itemId, DateTime productionDate, DateTime expirationDate);
}