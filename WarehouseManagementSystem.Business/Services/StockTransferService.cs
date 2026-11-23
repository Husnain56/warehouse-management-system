using Microsoft.EntityFrameworkCore;
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
        if (transferDto.SourceWarehouseId == transferDto.DestinationWarehouseId)
            throw new InvalidOperationException("Source and destination warehouses cannot be the same.");

        var sourceStockItems = await _unitOfWork.StockItems
            .FindAsync(s => s.WarehouseId == transferDto.SourceWarehouseId &&
                            transferDto.Items.Select(i => i.ItemId).Contains(s.ItemId));

        foreach (var transferItem in transferDto.Items)
        {
            var stockItem = sourceStockItems.FirstOrDefault(s => s.ItemId == transferItem.ItemId);

            if (stockItem == null || stockItem.Quantity < transferItem.Quantity)
                throw new InvalidOperationException($"Insufficient stock for item ID: {transferItem.ItemId}");

            if (stockItem.ExpirationDate <= DateTime.Now)
                throw new InvalidOperationException($"Cannot transfer expired item ID: {transferItem.ItemId}");

            // Deduct from source warehouse
            stockItem.Quantity -= transferItem.Quantity;
            await _unitOfWork.StockItems.UpdateAsync(stockItem);

            // Check if item exists in destination warehouse
            var destinationStockItem = await _unitOfWork.StockItems
                .FindAsync(s => s.ItemId == transferItem.ItemId &&
                                s.WarehouseId == transferDto.DestinationWarehouseId);

            if (destinationStockItem.Any())
            {
                var existingStock = destinationStockItem.First();
                existingStock.Quantity += transferItem.Quantity;
                await _unitOfWork.StockItems.UpdateAsync(existingStock);
            }
            else
            {
                var newStockItem = new StockItem
                {
                    ItemId = transferItem.ItemId,
                    WarehouseId = transferDto.DestinationWarehouseId,
                    Quantity = transferItem.Quantity,
                    ProductionDate = stockItem.ProductionDate,
                    ExpirationDate = stockItem.ExpirationDate,
                };
                await _unitOfWork.StockItems.AddAsync(newStockItem);
            }
        }

        await _unitOfWork.SaveAsync();
        return true;
    }

    public async Task<bool> TransferStockAsync(List<StockTransferDto> transfers)
    {
        foreach (var transfer in transfers)
        {
            // removed GetQuery from here
            var stockItem = await _unitOfWork.StockItems.GetStockItemAsync(
                transfer.SourceWarehouseId,
                transfer.ItemId,
                transfer.ProductionDate,
                transfer.ExpirationDate);
                        if (stockItem == null || stockItem.Quantity < transfer.TransferQuantity)
            return false; // Should never happen due to UI validation, but safety check

            stockItem.Quantity -= transfer.TransferQuantity;
            await _unitOfWork.StockItems.UpdateAsync(stockItem);

                // Check if item already exists in destination warehouse

                // removed getQuery from here
                var destinationStock = await _unitOfWork.StockItems.GetStockItemAsync(
                    transfer.DestinationWarehouseId,
                    transfer.ItemId,
                    transfer.ProductionDate,
                    transfer.ExpirationDate);
            if (destinationStock == null)
            {
                // Create new entry
                var newStock = new StockItem
                {
                    ItemId = transfer.ItemId,
                    WarehouseId = transfer.DestinationWarehouseId,
                    Quantity = transfer.TransferQuantity,
                    ProductionDate = transfer.ProductionDate,
                    ExpirationDate = transfer.ExpirationDate
                };
                await _unitOfWork.StockItems.AddAsync(newStock);
            }
            else
            {
                // Update existing quantity
                destinationStock.Quantity += transfer.TransferQuantity;
                await _unitOfWork.StockItems.UpdateAsync(destinationStock);
            }
        }

        await _unitOfWork.SaveAsync();
        return true;
    }

}
