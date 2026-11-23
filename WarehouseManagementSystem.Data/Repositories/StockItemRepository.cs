using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WarehouseManagementSystem.Data.Models;
using WarehouseManagementSystem.Data.Repositories.Interfaces;

namespace WarehouseManagementSystem.Data.Repositories;

public class StockItemRepository : Repository<StockItem>, IStockItemRepository
{
    private readonly WMSDbContext _context;

    public StockItemRepository(WMSDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<StockItem>> GetItemsByWarehousesAsync(List<int> warehouseIds)
    {
        return await _context.StockItems
            .Where(si => warehouseIds.Contains(si.WarehouseId))
            .Include(si => si.Item)
            .Include(si => si.Warehouse)
            .ToListAsync();
    }

    public async Task<List<StockItem>> GetStockWithItemAndWarehouseAsync()
    {
        return await _context.StockItems
            .Include(s => s.Item)
            .Include(s => s.Warehouse)
            .ToListAsync();
    }

    public async Task<IEnumerable<StockItem>> GetStockItemsWithItemAsync(int warehouseId)
    {
        return await _context.StockItems
            .Include(si => si.Item)
            .Where(si => si.WarehouseId == warehouseId)
            .ToListAsync();
    }

    public async Task<List<StockItem>> GetStockItemsWithItemAndWarehouseAsync(int warehouseId)
    {
        return await _context.StockItems
            .Where(s => s.WarehouseId == warehouseId)
            .Include(s => s.Item)
            .Include(s => s.Warehouse)
            .ToListAsync();
    }

    public async Task<StockItem> GetStockItemAsync(int warehouseId, int itemId, DateTime productionDate, DateTime expirationDate)
    {
        return await _context.StockItems
            .FirstOrDefaultAsync(s => s.ItemId == itemId
                && s.WarehouseId == warehouseId
                && s.ProductionDate == productionDate
                && s.ExpirationDate == expirationDate);
    }
}