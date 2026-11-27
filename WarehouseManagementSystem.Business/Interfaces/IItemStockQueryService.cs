using System.Collections.Generic;
using System.Data;
using WarehouseManagementSystem.Core.DTOs;

namespace WarehouseManagementSystem.Business.Interfaces;


public interface IItemStockQueryService
{
    Task<List<ItemWarehouseDto>> GetItemsInWarehousesAsync();
    Task<List<ItemWarehouseDto>> GetItemsByWarehousesAsync(List<int> warehouseIds);
    Task<List<ItemWarehouseDto>> GetItemsByWarehouseIdAsync(int warehouseId);
    Task<DataTable> GetItemsCloseToExpirationAsync(int daysThreshold);
}