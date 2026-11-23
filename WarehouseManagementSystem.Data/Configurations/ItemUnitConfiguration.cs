
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagementSystem.Data.Models;

namespace WarehouseManagementSystem.Data.Configurations;

public class ItemUnitConfiguration : IEntityTypeConfiguration<ItemUnit>
{
    public void Configure(EntityTypeBuilder<ItemUnit> builder)
    {
        builder.HasKey(iu => new { iu.ItemId, iu.UnitId });

        builder.HasOne(iu => iu.Item)
            .WithMany(i => i.ItemUnits)
            .HasForeignKey(iu => iu.ItemId);

        builder.HasOne(iu => iu.Unit)
            .WithMany(u => u.ItemUnits)
            .HasForeignKey(iu => iu.UnitId);
    }
}