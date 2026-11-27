using System;
using System.Collections.Generic;
using WarehouseManagementSystem.Core.DTOs;
using WarehouseManagementSystem.Data.Models;

namespace WarehouseManagementSystem.Business.Interfaces;


public interface ISupplyOrderService
{
    Task<IEnumerable<SupplyOrder>> GetAllSupplyOrdersAsync();
    Task<SupplyOrder> GetSupplyOrderByIdAsync(int id);
    Task<List<SupplyOrderDto>> GetAllSupplyOrdersDetailsAsync();
    Task CreateSupplyOrderAsync(int warehouseId, int supplierId, string orderNumber, DateTime orderDate, List<SupplyOrderDetail> details);
    Task AddSupplyOrderAsync(SupplyOrderDto orderDto);
}