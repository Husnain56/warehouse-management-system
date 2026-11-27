using System;
using System.Collections.Generic;
using WarehouseManagementSystem.Business.Interfaces;
using WarehouseManagementSystem.Data.Models;
using WarehouseManagementSystem.Data.UOW.Interfaces;

namespace WarehouseManagementSystem.Business.Services;



public class ItemService : IItemService
{
    private readonly IUnitOfWork _unitOfWork;

    public ItemService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Item>> GetAllItemsAsync()
    {
        return await _unitOfWork.Items.GetAllAsync();
    }

    public async Task<Item> GetItemByIdAsync(int id) =>
        await _unitOfWork.Items.GetByIdAsync(id);

    public async Task AddItemAsync(string code, string name, string measurementUnit)
    {
        EnsureItemNameProvided(name);

        await InsertItemAsync(code, name, measurementUnit);
    }

    public async Task AddNewItemToWarehouseAsync(string itemCode, string itemName, int warehouseId, int quantity, string measurementUnit)
    {
        EnsureItemNameProvided(itemName);

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        var newItem = await InsertItemAsync(itemCode, itemName, measurementUnit);
        await InsertStockItemAsync(newItem.Id, warehouseId, quantity);
    }

    public async Task UpdateItemAsync(int id, string code, string name, string measurementUnit)
    {
        var item = await GetExistingItemAsync(id);

        item.Code = code;
        item.Name = name;
        item.MeasurementUnit = measurementUnit;

        await _unitOfWork.Items.UpdateAsync(item);
        await _unitOfWork.SaveAsync();
    }

    public async Task DeleteItemAsync(int id)
    {
        var item = await GetExistingItemAsync(id);

        await _unitOfWork.Items.DeleteAsync(item);
        await _unitOfWork.SaveAsync();
    }

    private async Task<Item> GetExistingItemAsync(int id)
    {
        return await _unitOfWork.Items.GetByIdAsync(id)
            ?? throw new Exception("Item not found.");
    }

    private static void EnsureItemNameProvided(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Item name cannot be empty.");
    }

    private async Task<Item> InsertItemAsync(string code, string name, string measurementUnit)
    {
        var item = new Item { Code = code, Name = name, MeasurementUnit = measurementUnit };
        await _unitOfWork.Items.AddAsync(item);
        await _unitOfWork.SaveAsync();
        return item;
    }

    private async Task InsertStockItemAsync(int itemId, int warehouseId, int quantity)
    {
        await _unitOfWork.StockItems.AddAsync(new StockItem
        {
            ItemId = itemId,
            WarehouseId = warehouseId,
            Quantity = quantity
        });
        await _unitOfWork.SaveAsync();
    }
}