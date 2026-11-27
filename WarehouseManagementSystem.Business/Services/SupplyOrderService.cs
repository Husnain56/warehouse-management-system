using System;
using System.Collections.Generic;
using WarehouseManagementSystem.Business.Interfaces;
using WarehouseManagementSystem.Core.DTOs;
using WarehouseManagementSystem.Data.Models;
using WarehouseManagementSystem.Data.UOW.Interfaces;

namespace WarehouseManagementSystem.Business.Services;


public class SupplyOrderService : ISupplyOrderService
{
    private readonly IUnitOfWork _unitOfWork;

    public SupplyOrderService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<SupplyOrder>> GetAllSupplyOrdersAsync() =>
        await _unitOfWork.SupplyOrders.GetAllAsync();

    public async Task<SupplyOrder> GetSupplyOrderByIdAsync(int id) =>
        await _unitOfWork.SupplyOrders.GetByIdAsync(id);

    public async Task<List<SupplyOrderDto>> GetAllSupplyOrdersDetailsAsync() =>
        ToSupplyOrderDtos(await _unitOfWork.SupplyOrders.GetAllWithDetailsAsync());

    public async Task CreateSupplyOrderAsync(int warehouseId, int supplierId, string orderNumber, DateTime orderDate, List<SupplyOrderDetail> details)
    {
        if (details == null || details.Count == 0)
            throw new ArgumentException("Supply order must have at least one item.");

        await _unitOfWork.SupplyOrders.AddAsync(new SupplyOrder
        {
            WarehouseId = warehouseId,
            SupplierId = supplierId,
            OrderNumber = orderNumber,
            OrderDate = orderDate,
            SupplyOrderDetails = details
        });
        await _unitOfWork.SaveAsync();
    }

    public async Task AddSupplyOrderAsync(SupplyOrderDto orderDto)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var itemId = await InsertItemAsync(orderDto);
            await InsertSupplyOrderAsync(orderDto, itemId);
            await InsertStockItemAsync(orderDto, itemId);

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new Exception("Error while adding supply order: " + ex.Message);
        }
    }

    private static List<SupplyOrderDto> ToSupplyOrderDtos(IEnumerable<SupplyOrder> orders) =>
        orders.SelectMany(order => order.SupplyOrderDetails.Select(detail => ToSupplyOrderDto(order, detail))).ToList();

    private static SupplyOrderDto ToSupplyOrderDto(SupplyOrder order, SupplyOrderDetail detail)
    {
        return new SupplyOrderDto
        {
            OrderId = order.Id,
            OrderNumber = order.OrderNumber,
            OrderDate = order.OrderDate,
            SupplierName = order.Supplier.Name,
            WarehouseName = order.Warehouse.Name,
            ItemName = detail.Item.Name,
            Quantity = detail.Quantity,
            ProductionDate = detail.ProductionDate,
            ExpirationDate = detail.ExpirationDate
        };
    }

    private async Task<int> InsertItemAsync(SupplyOrderDto orderDto)
    {
        var item = new Item
        {
            Code = orderDto.ItemCode,
            Name = orderDto.ItemName,
            MeasurementUnit = orderDto.MeasurementUnit
        };

        await _unitOfWork.Items.AddAsync(item);
        await _unitOfWork.SaveAsync();
        return item.Id;
    }

    private async Task InsertSupplyOrderAsync(SupplyOrderDto orderDto, int itemId)
    {
        await _unitOfWork.SupplyOrders.AddAsync(new SupplyOrder
        {
            SupplierId = orderDto.SupplierId,
            WarehouseId = orderDto.WarehouseId,
            OrderNumber = GenerateOrderNumber(),
            OrderDate = DateTime.Now,
            SupplyOrderDetails = new List<SupplyOrderDetail>
            {
                new SupplyOrderDetail
                {
                    ItemId = itemId,
                    Quantity = orderDto.Quantity,
                    ProductionDate = orderDto.ProductionDate,
                    ExpirationDate = orderDto.ExpirationDate
                }
            }
        });
        await _unitOfWork.SaveAsync();
    }

    private async Task InsertStockItemAsync(SupplyOrderDto orderDto, int itemId)
    {
        await _unitOfWork.StockItems.AddAsync(new StockItem
        {
            WarehouseId = orderDto.WarehouseId,
            ItemId = itemId,
            Quantity = orderDto.Quantity,
            ProductionDate = orderDto.ProductionDate,
            ExpirationDate = orderDto.ExpirationDate
        });
        await _unitOfWork.SaveAsync();
    }

    private static string GenerateOrderNumber() =>
        "SO-" + Guid.NewGuid().ToString().Substring(0, 6);
}