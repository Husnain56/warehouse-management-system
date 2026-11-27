using System;
using System.Collections.Generic;
using WarehouseManagementSystem.Business.Interfaces;
using WarehouseManagementSystem.Core.DTOs;
using WarehouseManagementSystem.Data.Models;
using WarehouseManagementSystem.Data.UOW.Interfaces;

namespace WarehouseManagementSystem.Business.Services;



public class StockTransferService : IStockTransferService
{
    private readonly IUnitOfWork _unitOfWork;

    public StockTransferService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<StockTransfer>> GetAllStockTransfersAsync() =>
        await _unitOfWork.StockTransfers.GetAllAsync();

    public async Task<StockTransfer> GetStockTransferByIdAsync(int id) =>
        await _unitOfWork.StockTransfers.GetByIdAsync(id);

    public async Task<bool> TransferStockAsync(StockTransferDto transferDto)
    {
        EnsureDifferentWarehouses(transferDto);

        var sourceStockItems = await LoadSourceStockItemsAsync(transferDto);

        foreach (var transferItem in transferDto.Items)
        {
            var sourceStockItem = GetTransferableStockItem(sourceStockItems, transferItem);

            await ChangeQuantityAsync(sourceStockItem, -transferItem.Quantity);
            await AddToDestinationAsync(transferDto.DestinationWarehouseId, transferItem, sourceStockItem);
        }

        await _unitOfWork.SaveAsync();
        return true;
    }

    private static void EnsureDifferentWarehouses(StockTransferDto transferDto)
    {
        if (transferDto.SourceWarehouseId == transferDto.DestinationWarehouseId)
            throw new InvalidOperationException("Source and destination warehouses cannot be the same.");
    }

    private async Task<IEnumerable<StockItem>> LoadSourceStockItemsAsync(StockTransferDto transferDto)
    {
        return await _unitOfWork.StockItems
            .FindAsync(s => s.WarehouseId == transferDto.SourceWarehouseId &&
                            transferDto.Items.Select(i => i.ItemId).Contains(s.ItemId));
    }

    private static StockItem GetTransferableStockItem(IEnumerable<StockItem> sourceStockItems, StockTransferItemDto transferItem)
    {
        var stockItem = sourceStockItems.FirstOrDefault(s => s.ItemId == transferItem.ItemId);

        if (HasInsufficientStock(stockItem, transferItem.Quantity))
            throw new InvalidOperationException($"Insufficient stock for item ID: {transferItem.ItemId}");

        if (IsExpired(stockItem!))
            throw new InvalidOperationException($"Cannot transfer expired item ID: {transferItem.ItemId}");

        return stockItem!;
    }

    private static bool HasInsufficientStock(StockItem? stockItem, int requestedQuantity) =>
        stockItem == null || stockItem.Quantity < requestedQuantity;

    private static bool IsExpired(StockItem stockItem) =>
        stockItem.ExpirationDate <= DateTime.Now;

    private async Task AddToDestinationAsync(int destinationWarehouseId, StockTransferItemDto transferItem, StockItem sourceStockItem)
    {
        var existingStock = await FindStockInWarehouseAsync(destinationWarehouseId, transferItem.ItemId);

        if (existingStock != null)
            await ChangeQuantityAsync(existingStock, transferItem.Quantity);
        else
            await CreateStockInWarehouseAsync(destinationWarehouseId, transferItem, sourceStockItem);
    }

    private async Task<StockItem?> FindStockInWarehouseAsync(int warehouseId, int itemId) =>
        (await _unitOfWork.StockItems
            .FindAsync(s => s.ItemId == itemId && s.WarehouseId == warehouseId))
            .FirstOrDefault();

    private async Task ChangeQuantityAsync(StockItem stockItem, int quantityChange)
    {
        stockItem.Quantity += quantityChange;
        await _unitOfWork.StockItems.UpdateAsync(stockItem);
    }

    private async Task CreateStockInWarehouseAsync(int warehouseId, StockTransferItemDto transferItem, StockItem sourceStockItem)
    {
        await _unitOfWork.StockItems.AddAsync(new StockItem
        {
            ItemId = transferItem.ItemId,
            WarehouseId = warehouseId,
            Quantity = transferItem.Quantity,
            ProductionDate = sourceStockItem.ProductionDate,
            ExpirationDate = sourceStockItem.ExpirationDate,
        });
    }
}