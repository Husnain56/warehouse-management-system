using WarehouseManagementSystem.Data.Models;
using WarehouseManagementSystem.Data.Repositories.Interfaces;

namespace WarehouseManagementSystem.Data.Repositories;

public class WarehouseRepository : Repository<Warehouse>, IWarehouseRepository
{
    private readonly WMSDbContext _context;

    public WarehouseRepository(WMSDbContext context) : base(context)
    {
        _context = context;
    }
}