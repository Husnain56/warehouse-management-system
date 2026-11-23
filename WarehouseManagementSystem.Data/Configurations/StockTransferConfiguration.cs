
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagementSystem.Data.Models;

namespace WarehouseManagementSystem.Data.Configurations;

public class StockTransferConfiguration : IEntityTypeConfiguration<StockTransfer>
{
    public void Configure(EntityTypeBuilder<StockTransfer> builder)
    {
        builder.HasOne(st => st.FromWarehouse)
            .WithMany()
            .HasForeignKey(st => st.FromWarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(st => st.ToWarehouse)
            .WithMany()
            .HasForeignKey(st => st.ToWarehouseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}