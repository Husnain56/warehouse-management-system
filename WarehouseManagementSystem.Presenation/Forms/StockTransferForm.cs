using WarehouseManagementSystem.Business.Interfaces;
using WarehouseManagementSystem.Core.DTOs;
using WarehouseManagementSystem.Data.Models;

namespace WarehouseManagementSystem.Presenation.Forms;

public partial class StockTransferForm : Form
{
    private const int DefaultTransferQuantity = 0;

    private static class Column
    {
        public const string Select = "Select";
        public const string ItemId = "ItemId";
        public const string ItemName = "ItemName";
        public const string AvailableQuantity = "AvailableQuantity";
        public const string ProductionDate = "ProductionDate";
        public const string ExpirationDate = "ExpirationDate";
        public const string TransferQuantity = "TransferQuantity";
    }

    private readonly IStockTransferService _stockTransferService;
    private readonly IItemStockQueryService _itemStockQueryService;
    private readonly IWarehouseService _warehouseService;

    public StockTransferForm(
        IStockTransferService stockTransferService,
        IItemStockQueryService itemStockQueryService,
        IWarehouseService warehouseService)
    {
        InitializeComponent();
        _stockTransferService = stockTransferService;
        _itemStockQueryService = itemStockQueryService;
        _warehouseService = warehouseService;
    }

    private void InitializeDataGridView()
    {
        dgvStockItems.Columns.Clear();
        dgvStockItems.AutoGenerateColumns = false;
        dgvStockItems.AllowUserToDeleteRows = false;

        dgvStockItems.Columns.Add(new DataGridViewCheckBoxColumn { Name = Column.Select, HeaderText = "✔", Width = 40 });
        AddReadOnlyColumn(Column.ItemId, "Item Id", "ItemId", 150, visible: false);
        AddReadOnlyColumn(Column.ItemName, "Item Name", "ItemName", 150);
        AddReadOnlyColumn(Column.AvailableQuantity, "Available Qty", "Quantity", 120);
        AddReadOnlyColumn(Column.ProductionDate, "Production Date", "ProductionDate", 200);
        AddReadOnlyColumn(Column.ExpirationDate, "Expiration Date", "ExpirationDate", 200);
        dgvStockItems.Columns.Add(new DataGridViewTextBoxColumn { Name = Column.TransferQuantity, HeaderText = "Transfer Qty", Width = 120 });
    }

    private void AddReadOnlyColumn(string name, string headerText, string dataPropertyName, int width, bool visible = true)
    {
        dgvStockItems.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = name,
            HeaderText = headerText,
            DataPropertyName = dataPropertyName,
            ReadOnly = true,
            Width = width,
            Visible = visible
        });
    }

    private async Task LoadStockItemsAsync(int warehouseId)
    {
        dgvStockItems.Rows.Clear();

        foreach (var stock in await _itemStockQueryService.GetItemsByWarehouseIdAsync(warehouseId))
            AddStockRow(stock);
    }

    private void AddStockRow(ItemWarehouseDto stock)
    {
        dgvStockItems.Rows.Add(false,
                               stock.ItemId,
                               stock.ItemName,
                               stock.Quantity,
                               stock.ProductionDate.ToShortDateString(),
                               stock.ExpirationDate.ToShortDateString(),
                               DefaultTransferQuantity);
    }

    private async Task LoadWarehousesIntoCmb()
    {
        try
        {
            var warehouses = await _warehouseService.GetAllWarehousesAsync();
            if (warehouses == null || !warehouses.Any())
            {
                MessageBox.Show("No warehouses found.");
                return;
            }

            BindToOwnWarehouseList(cmbSourceWarehouse, warehouses);
            BindToOwnWarehouseList(cmbDestinationWarehouse, warehouses);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading warehouses: {ex.Message}");
        }
    }

    private static void BindToOwnWarehouseList(ComboBox comboBox, IEnumerable<Warehouse> warehouses)
    {
        comboBox.DataSource = null;
        comboBox.DataSource = warehouses.ToList();
        comboBox.DisplayMember = "Name";
        comboBox.ValueMember = "Id";
    }

    private bool TryGetSelectedWarehouseIds(out int sourceWarehouseId, out int destinationWarehouseId)
    {
        sourceWarehouseId = 0;
        destinationWarehouseId = 0;

        if (cmbSourceWarehouse.SelectedItem is not Warehouse source ||
            cmbDestinationWarehouse.SelectedItem is not Warehouse destination)
        {
            Dialogs.ShowError("Please select both source and destination warehouses.");
            return false;
        }

        sourceWarehouseId = source.Id;
        destinationWarehouseId = destination.Id;

        if (sourceWarehouseId == destinationWarehouseId)
        {
            Dialogs.ShowError("Source and destination warehouses cannot be the same.");
            return false;
        }

        return true;
    }

    private bool TryBuildTransfer(int sourceWarehouseId, int destinationWarehouseId, out StockTransferDto transferDto)
    {
        transferDto = new StockTransferDto
        {
            SourceWarehouseId = sourceWarehouseId,
            DestinationWarehouseId = destinationWarehouseId,
            TransferDate = DateTime.UtcNow
        };

        foreach (var row in dgvStockItems.Rows.Cast<DataGridViewRow>().Where(IsSelected))
        {
            if (!TryReadTransferItem(row, out var item))
                return false;

            transferDto.Items.Add(item);
        }

        if (transferDto.Items.Count == 0)
        {
            Dialogs.ShowWarning("Please select at least one item to transfer.");
            return false;
        }

        return true;
    }

    private bool TryReadTransferItem(DataGridViewRow row, out StockTransferItemDto item)
    {
        item = new StockTransferItemDto();

        if (!TryReadPositiveQuantity(row, out var quantity))
        {
            Dialogs.ShowWarning($"Invalid transfer quantity for {GetItemName(row)}.");
            return false;
        }

        if (quantity > GetAvailableQuantity(row))
        {
            Dialogs.ShowWarning($"Transfer quantity exceeds available stock for {GetItemName(row)}.");
            return false;
        }

        if (IsExpired(row))
        {
            Dialogs.ShowWarning($"Cannot transfer expired item: {GetItemName(row)}.");
            return false;
        }

        item.ItemId = (int)row.Cells[Column.ItemId].Value;
        item.Quantity = quantity;
        return true;
    }

    private static bool IsSelected(DataGridViewRow row) =>
        Convert.ToBoolean(row.Cells[Column.Select].Value);

    private static bool TryReadPositiveQuantity(DataGridViewRow row, out int quantity) =>
        int.TryParse(row.Cells[Column.TransferQuantity].Value?.ToString(), out quantity) && quantity > 0;

    private static int GetAvailableQuantity(DataGridViewRow row) =>
        (int)row.Cells[Column.AvailableQuantity].Value;

    private static bool IsExpired(DataGridViewRow row) =>
        DateTime.Parse(row.Cells[Column.ExpirationDate].Value.ToString()) < DateTime.Today;

    private static string GetItemName(DataGridViewRow row) =>
        row.Cells[Column.ItemName].Value?.ToString() ?? string.Empty;

    private async void btnLoadItems_Click(object sender, EventArgs e)
    {
        await LoadStockItemsAsync((int)cmbSourceWarehouse.SelectedValue);
    }

    private async void btnTransferStock_Click(object sender, EventArgs e)
    {
        if (!TryGetSelectedWarehouseIds(out var sourceWarehouseId, out var destinationWarehouseId))
            return;

        if (!TryBuildTransfer(sourceWarehouseId, destinationWarehouseId, out var transferDto))
            return;

        await _stockTransferService.TransferStockAsync(transferDto);

        Dialogs.ShowSuccess("Stock transferred successfully!");
        await LoadStockItemsAsync(sourceWarehouseId);
    }

    private async void StockTransferForm_Load(object sender, EventArgs e)
    {
        InitializeDataGridView();
        await LoadWarehousesIntoCmb();
    }
}