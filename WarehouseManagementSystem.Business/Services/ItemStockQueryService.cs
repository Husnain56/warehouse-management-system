using System;
using System.Collections.Generic;
using System.Data;
using WarehouseManagementSystem.Business.Interfaces;
using WarehouseManagementSystem.Core.DTOs;
using WarehouseManagementSystem.Data.Models;
using WarehouseManagementSystem.Data.UOW.Interfaces;

namespace WarehouseManagementSystem.Business.Services;



public class ItemStockQueryService : IItemStockQueryService
{
    private readonly IUnitOfWork _unitOfWork;

    public ItemStockQueryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<ItemWarehouseDto>> GetItemsInWarehousesAsync() =>
        ToItemWarehouseDtos(await GetAllStockItemsWithDetailsAsync());

    public async Task<List<ItemWarehouseDto>> GetItemsByWarehousesAsync(List<int> warehouseIds) =>
        ToItemWarehouseDtos(await _unitOfWork.StockItems.GetItemsByWarehousesAsync(warehouseIds));

    public async Task<List<ItemWarehouseDto>> GetItemsByWarehouseIdAsync(int warehouseId) =>
        ToItemWarehouseDtos(await _unitOfWork.StockItems.GetStockItemsWithItemAndWarehouseAsync(warehouseId));

    public async Task<DataTable> GetItemsCloseToExpirationAsync(int daysThreshold)
    {
        var currentDate = DateTime.Now;
        var stockItems = (await GetAllStockItemsWithDetailsAsync())
            .Where(si => IsCloseToExpiration(si, currentDate, daysThreshold));

        return BuildExpirationTable(stockItems, currentDate);
    }

    private async Task<IEnumerable<StockItem>> GetAllStockItemsWithDetailsAsync() =>
        await _unitOfWork.StockItems.GetAllWithIncludesAsync(si => si.Item, si => si.Warehouse);

    private static List<ItemWarehouseDto> ToItemWarehouseDtos(IEnumerable<StockItem> stockItems) =>
        stockItems.Select(ToItemWarehouseDto).ToList();

    private static ItemWarehouseDto ToItemWarehouseDto(StockItem stockItem)
    {
        return new ItemWarehouseDto
        {
            ItemId = stockItem.Item.Id,
            ItemName = stockItem.Item.Name,
            WarehouseName = stockItem.Warehouse.Name,
            Quantity = stockItem.Quantity,
            ItemCode = stockItem.Item.Code,
            ProductionDate = stockItem.ProductionDate,
            ExpirationDate = stockItem.ExpirationDate,
            Units = stockItem.Item.ItemUnits.Select(iu => new ItemUnitDto
            {
                UnitId = iu.Unit.Id,
                UnitName = iu.Unit.Name,
                ConversionFactor = iu.ConversionFactor
            }).ToList()
        };
    }

    private static bool IsCloseToExpiration(StockItem stockItem, DateTime currentDate, int daysThreshold) =>
        (stockItem.ExpirationDate - currentDate).TotalDays <= daysThreshold;

    private static int DaysUntilExpiration(StockItem stockItem, DateTime currentDate) =>
        (stockItem.ExpirationDate - currentDate).Days;

    private static DataTable BuildExpirationTable(IEnumerable<StockItem> stockItems, DateTime currentDate)
    {
        var table = new DataTable();
        table.Columns.Add("ItemName");
        table.Columns.Add("WarehouseName");
        table.Columns.Add("Quantity");
        table.Columns.Add("ProductionDate");
        table.Columns.Add("ExpirationDate");
        table.Columns.Add("DaysUntilExpiration");

        foreach (var stockItem in stockItems)
        {
            table.Rows.Add(
                stockItem.Item.Name,
                stockItem.Warehouse.Name,
                stockItem.Quantity,
                stockItem.ProductionDate.ToShortDateString(),
                stockItem.ExpirationDate.ToShortDateString(),
                DaysUntilExpiration(stockItem, currentDate)
            );
        }

        return table;
    }
}