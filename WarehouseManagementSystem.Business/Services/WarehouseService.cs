using System;
using System.Collections.Generic;
using WarehouseManagementSystem.Business.Interfaces;
using WarehouseManagementSystem.Data.Models;
using WarehouseManagementSystem.Data.UOW.Interfaces;

namespace WarehouseManagementSystem.Business.Services;



public class WarehouseService : IWarehouseService
{
    private readonly IUnitOfWork _unitOfWork;

    public WarehouseService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Warehouse>> GetAllWarehousesAsync() =>
        await _unitOfWork.Warehouses.GetAllAsync();

    public async Task<Warehouse> GetWarehouseByIdAsync(int id) =>
        await _unitOfWork.Warehouses.GetByIdAsync(id);

    public async Task AddWarehouseAsync(string name, string address, string manager)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Warehouse name cannot be empty.");

        await _unitOfWork.Warehouses.AddAsync(new Warehouse { Name = name, Address = address, Manager = manager });
        await _unitOfWork.SaveAsync();
    }

    public async Task UpdateWarehouseAsync(int id, string name, string address, string manager)
    {
        var warehouse = await GetExistingWarehouseAsync(id);

        warehouse.Name = name;
        warehouse.Address = address;
        warehouse.Manager = manager;

        await _unitOfWork.Warehouses.UpdateAsync(warehouse);
        await _unitOfWork.SaveAsync();
    }

    public async Task DeleteWarehouseAsync(int id)
    {
        var warehouse = await GetExistingWarehouseAsync(id);

        await _unitOfWork.Warehouses.DeleteAsync(warehouse);
        await _unitOfWork.SaveAsync();
    }

    private async Task<Warehouse> GetExistingWarehouseAsync(int id)
    {
        return await _unitOfWork.Warehouses.GetByIdAsync(id)
            ?? throw new Exception("Warehouse not found.");
    }
}