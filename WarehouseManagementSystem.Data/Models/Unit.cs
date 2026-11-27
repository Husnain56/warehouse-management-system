using System.Collections.Generic;

namespace WarehouseManagementSystem.Data.Models;

// Now it inherits from BaseEntity to get the id instead of creating by itself
public class Unit : BaseEntity
{
    public string Name { get; set; }
    public ICollection<ItemUnit> ItemUnits { get; set; } = new List<ItemUnit>();
}