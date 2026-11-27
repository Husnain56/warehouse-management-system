using System;
using System.Collections.Generic;
using System.Data;
using WarehouseManagementSystem.Business.Interfaces;
using WarehouseManagementSystem.Data.Models;
using WarehouseManagementSystem.Data.UOW.Interfaces;

namespace WarehouseManagementSystem.Business.Services;



public class WarehouseReportService : IWarehouseReportService
{
    private readonly IUnitOfWork _unitOfWork;

    public WarehouseReportService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DataTable> GetWarehouseStatusReport(int warehouseId)
    {
        var table = CreateTable("ItemName", "Quantity", "ProductionDate", "ExpirationDate");

        foreach (var stockItem in await _unitOfWork.StockItems.GetStockItemsWithItemAsync(warehouseId))
        {
            table.Rows.Add(
                stockItem.Item?.Name ?? "Unknown",
                stockItem.Quantity,
                stockItem.ProductionDate.ToShortDateString(),
                stockItem.ExpirationDate.ToShortDateString()
            );
        }

        return table;
    }

    public async Task<DataTable> GetItemsReport(List<int> warehouseIds)
    {
        var table = CreateTable("WarehouseName", "ItemName", "Quantity", "ProductionDate", "ExpirationDate");

        foreach (var stockItem in await GetAllStockItemsWithDetailsAsync())
        {
            table.Rows.Add(
                stockItem.Warehouse?.Name ?? "Unknown",
                stockItem.Item?.Name ?? "Unknown",
                stockItem.Quantity,
                stockItem.ProductionDate.ToShortDateString(),
                stockItem.ExpirationDate.ToShortDateString()
            );
        }

        return table;
    }

    public async Task<DataTable> GetItemsInWarehouseForPeriod(int warehouseId, DateTime fromDate, DateTime toDate)
    {
        var table = CreateTable("WarehouseName", "ItemName", "Quantity", "ProductionDate", "ExpirationDate", "DaysInWarehouse");

        var stockItems = (await GetAllStockItemsWithDetailsAsync())
            .Where(si => si.WarehouseId == warehouseId);

        foreach (var stockItem in stockItems)
        {
            var orderDate = await GetSupplyOrderDateAsync(stockItem);

            if (!IsWithinPeriod(orderDate, fromDate, toDate))
                continue;

            table.Rows.Add(
                stockItem.Warehouse.Name,
                stockItem.Item.Name,
                stockItem.Quantity,
                stockItem.ProductionDate.ToShortDateString(),
                stockItem.ExpirationDate.ToShortDateString(),
                DaysSince(orderDate)
            );
        }

        return table;
    }

    private async Task<List<StockItem>> GetAllStockItemsWithDetailsAsync() =>
        await _unitOfWork.StockItems.GetAllWithIncludesAsync(si => si.Item, si => si.Warehouse);

    private async Task<DateTime> GetSupplyOrderDateAsync(StockItem stockItem)
    {
        return (await _unitOfWork.SupplyOrderDetails
                .FindAsync(sod => sod.ItemId == stockItem.ItemId && sod.ProductionDate == stockItem.ProductionDate))
            .FirstOrDefault()?.SupplyOrder?.OrderDate ?? DateTime.MinValue;
    }

    private static bool IsWithinPeriod(DateTime date, DateTime fromDate, DateTime toDate) =>
        date >= fromDate && date <= toDate;

    private static int DaysSince(DateTime date) =>
        (DateTime.Now - date).Days;

    private static DataTable CreateTable(params string[] columnNames)
    {
        var table = new DataTable();
        foreach (var columnName in columnNames)
            table.Columns.Add(columnName);
        return table;
    }
}