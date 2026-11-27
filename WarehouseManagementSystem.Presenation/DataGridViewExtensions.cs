namespace WarehouseManagementSystem.Presenation;

internal static class DataGridViewExtensions
{
    public static int? GetSelectedId(this DataGridView grid) =>
        grid.SelectedRows.Count > 0
            ? Convert.ToInt32(grid.SelectedRows[0].Cells["Id"].Value)
            : null;
}