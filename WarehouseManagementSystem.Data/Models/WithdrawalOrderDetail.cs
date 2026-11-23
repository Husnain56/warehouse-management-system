// Models/WithdrawalOrderDetail.cs (Implements IAuditableEntity without unused Id)
using System;

namespace WarehouseManagementSystem.Data.Models;

public class WithdrawalOrderDetail : IAuditableEntity
{
    public int WithdrawalOrderId { get; set; }
    public WithdrawalOrder WithdrawalOrder { get; set; }

    public int ItemId { get; set; }
    public Item Item { get; set; }

    public int Quantity { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}