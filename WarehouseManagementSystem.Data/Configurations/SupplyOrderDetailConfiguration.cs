
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagementSystem.Data.Models;

namespace WarehouseManagementSystem.Data.Configurations;

public class SupplyOrderDetailConfiguration : IEntityTypeConfiguration<SupplyOrderDetail>
{
    public void Configure(EntityTypeBuilder<SupplyOrderDetail> builder)
    {
        builder.HasKey(sod => new { sod.SupplyOrderId, sod.ItemId });
    }
}