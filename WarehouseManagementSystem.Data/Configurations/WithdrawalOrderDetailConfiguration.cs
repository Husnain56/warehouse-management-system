
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagementSystem.Data.Models;

namespace WarehouseManagementSystem.Data.Configurations;

public class WithdrawalOrderDetailConfiguration : IEntityTypeConfiguration<WithdrawalOrderDetail>
{
    public void Configure(EntityTypeBuilder<WithdrawalOrderDetail> builder)
    {
        builder.HasKey(wod => new { wod.WithdrawalOrderId, wod.ItemId });
    }
}