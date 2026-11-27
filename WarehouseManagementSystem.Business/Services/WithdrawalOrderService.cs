using System;
using System.Collections.Generic;
using WarehouseManagementSystem.Business.Interfaces;
using WarehouseManagementSystem.Core.DTOs;
using WarehouseManagementSystem.Data.Models;
using WarehouseManagementSystem.Data.UOW.Interfaces;

namespace WarehouseManagementSystem.Business.Services;



public class WithdrawalOrderService : IWithdrawalOrderService
{
    private readonly IUnitOfWork _unitOfWork;

    public WithdrawalOrderService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<WithdrawalOrderDTO>> GetAllWithdrawalOrdersAsync() =>
        ToDtos(await _unitOfWork.WithdrawalOrders.GetAllAsync());

    public async Task<WithdrawalOrderDTO> GetWithdrawalOrderByIdAsync(int id)
    {
        var order = await _unitOfWork.WithdrawalOrders.GetByIdAsync(id);

        return order == null ? null : ToDto(order);
    }

    public async Task CreateOrderAsync(WithdrawalOrderDTO orderDto)
    {
        foreach (var item in orderDto.Items)
            await DeductStockAsync(orderDto.WarehouseId, item);

        await _unitOfWork.WithdrawalOrders.AddAsync(new WithdrawalOrder
        {
            OrderDate = orderDto.OrderDate,
            WarehouseId = orderDto.WarehouseId,
            WithdrawalOrderDetails = orderDto.Items.Select(ToDetail).ToList()
        });
    }

    public async Task UpdateOrderAsync(WithdrawalOrderDTO orderDto)
    {
        var existingOrder = await _unitOfWork.WithdrawalOrders.GetByIdAsync(orderDto.OrderId);
        if (existingOrder == null) return;

        existingOrder.OrderDate = orderDto.OrderDate;
        existingOrder.WarehouseId = orderDto.WarehouseId;
        ReplaceDetails(existingOrder, orderDto.Items);

        await _unitOfWork.WithdrawalOrders.UpdateAsync(existingOrder);
    }

    public async Task DeleteAsync(int id) =>
        await _unitOfWork.WithdrawalOrders.DeleteAsync(await _unitOfWork.WithdrawalOrders.GetByIdAsync(id));

    public async Task<IEnumerable<WithdrawalOrderDTO>> GetOrdersByDateRange(DateTime startDate, DateTime endDate) =>
        ToDtos(await _unitOfWork.WithdrawalOrders.GetByDateRangeAsync(startDate, endDate));

    public async Task<IEnumerable<WithdrawalOrderDTO>> GetOrdersByWarehouse(int warehouseId) =>
        ToDtos(await _unitOfWork.WithdrawalOrders.GetByWarehouseAsync(warehouseId));

    private async Task DeductStockAsync(int warehouseId, WithdrawalOrderItemDTO item)
    {
        var stockItem = await _unitOfWork.StockItems.GetByIdAsync(warehouseId, item.ItemId);

        if (HasInsufficientStock(stockItem, item.Quantity))
            throw new InvalidOperationException($"Insufficient quantity for item: {item.ItemName}");

        stockItem.Quantity -= item.Quantity;
        await _unitOfWork.StockItems.UpdateAsync(stockItem);
    }

    private static bool HasInsufficientStock(StockItem? stockItem, int requestedQuantity) =>
        stockItem == null || stockItem.Quantity < requestedQuantity;

    private static void ReplaceDetails(WithdrawalOrder order, IEnumerable<WithdrawalOrderItemDTO> items)
    {
        order.WithdrawalOrderDetails.Clear();
        foreach (var item in items)
            order.WithdrawalOrderDetails.Add(ToDetail(item));
    }

    private static WithdrawalOrderDetail ToDetail(WithdrawalOrderItemDTO item)
    {
        return new WithdrawalOrderDetail
        {
            ItemId = item.ItemId,
            Quantity = item.Quantity
        };
    }

    private static IEnumerable<WithdrawalOrderDTO> ToDtos(IEnumerable<WithdrawalOrder> orders) =>
        orders.Select(ToDto);

    private static WithdrawalOrderDTO ToDto(WithdrawalOrder order)
    {
        return new WithdrawalOrderDTO
        {
            OrderId = order.Id,
            OrderDate = order.OrderDate,
            WarehouseId = order.WarehouseId,
            WarehouseName = order.Warehouse?.Name,
            Items = order.WithdrawalOrderDetails.Select(detail => new WithdrawalOrderItemDTO
            {
                ItemId = detail.ItemId,
                ItemName = detail.Item?.Name,
                Quantity = detail.Quantity
            }).ToList()
        };
    }
}