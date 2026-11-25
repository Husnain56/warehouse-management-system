using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Configuration;
using WarehouseManagementSystem.Data.Models;
using WarehouseManagementSystem.Data.SeedingData;

namespace WarehouseManagementSystem.Data;

public class WMSDbContext : DbContext
{
    public WMSDbContext() { }
    public WMSDbContext(DbContextOptions<WMSDbContext> options) : base(options)
    {
    }

    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Unit> Units { get; set; }
    public DbSet<ItemUnit> ItemUnits { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<StockItem> StockItems { get; set; }
    public DbSet<SupplyOrder> SupplyOrders { get; set; }
    public DbSet<WithdrawalOrder> WithdrawalOrders { get; set; }
    public DbSet<StockTransfer> StockTransfers { get; set; }
    public DbSet<StockTransferDetail> StockTransferDetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
            optionsBuilder.UseSqlServer(connectionString);
        }
        optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<StockBatchEntity>(); // refactoring 6
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WMSDbContext).Assembly);
        modelBuilder.SeedData();
    }
}