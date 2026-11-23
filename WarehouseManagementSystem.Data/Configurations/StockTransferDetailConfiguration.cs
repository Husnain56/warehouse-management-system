
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagementSystem.Data.Models;

namespace WarehouseManagementSystem.Data.Configurations;

public class StockTransferDetailConfiguration : IEntityTypeConfiguration<StockTransferDetail>
{
    public void Configure(EntityTypeBuilder<StockTransferDetail> builder)
    {
        builder.HasOne(d => d.StockTransfer)
            .WithMany(t => t.StockTransferDetails)
            .HasForeignKey(d => d.StockTransferId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Item)
            .WithMany()
            .HasForeignKey(d => d.ItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}