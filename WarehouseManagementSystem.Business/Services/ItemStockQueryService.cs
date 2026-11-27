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

    public async Task<List<ItemWarehouseDto>> GetItemsInWarehousesAsync()
    {
        var stockItems = await _unitOfWork.StockItems
            .GetAllWithIncludesAsync(s => s.Item, s => s.Warehouse);

        return stockItems.Select(ToItemWarehouseDto).ToList();
    }

    public async Task<List<ItemWarehouseDto>> GetItemsByWarehousesAsync(List<int> warehouseIds)
    {
        var stockItems = await _unitOfWork.StockItems.GetItemsByWarehousesAsync(warehouseIds);

        return stockItems.Select(ToItemWarehouseDto).ToList();
    }

    public async Task<List<ItemWarehouseDto>> GetItemsByWarehouseIdAsync(int warehouseId)
    {
        var stockItems = await _unitOfWork.StockItems.GetStockItemsWithItemAndWarehouseAsync(warehouseId);

        return stockItems.Select(ToItemWarehouseDto).ToList();
    }

    public async Task<DataTable> GetItemsCloseToExpirationAsync(int daysThreshold)
    {
        var currentDate = DateTime.Now;

        var stockItems = await _unitOfWork.StockItems
            .GetAllWithIncludesAsync(si => si.Item, si => si.Warehouse);

        var closeToExpiration = stockItems
            .Where(si => IsCloseToExpiration(si, currentDate, daysThreshold))
            .ToList();

        var expirationTable = CreateExpirationTable();

        foreach (var stockItem in closeToExpiration)
        {
            AddExpirationRow(expirationTable, stockItem, currentDate);
        }

        return expirationTable;
    }

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

    private static bool IsCloseToExpiration(StockItem stockItem, DateTime currentDate, int daysThreshold)
    {
        return (stockItem.ExpirationDate - currentDate).TotalDays <= daysThreshold;
    }

    private static DataTable CreateExpirationTable()
    {
        var table = new DataTable();
        table.Columns.Add("ItemName");
        table.Columns.Add("WarehouseName");
        table.Columns.Add("Quantity");
        table.Columns.Add("ProductionDate");
        table.Columns.Add("ExpirationDate");
        table.Columns.Add("DaysUntilExpiration");
        return table;
    }

    private static void AddExpirationRow(DataTable table, StockItem stockItem, DateTime currentDate)
    {
        var daysUntilExpiration = (stockItem.ExpirationDate - currentDate).Days;

        table.Rows.Add(
            stockItem.Item.Name,
            stockItem.Warehouse.Name,
            stockItem.Quantity,
            stockItem.ProductionDate.ToShortDateString(),
            stockItem.ExpirationDate.ToShortDateString(),
            daysUntilExpiration
        );
    }
}