using WarehouseManagementSystem.Business.Interfaces;
using WarehouseManagementSystem.Data.Models;

namespace WarehouseManagementSystem.Presenation.Forms;

public partial class ItemsForm : Form
{
    private const string ItemIdColumn = "ItemId";
    private const int UiStateSettleDelayMilliseconds = 100;
    private static readonly string[] MeasurementUnits = { "", "Piece", "Kg", "Liter", "Meter" };

    private readonly IItemService _itemService;
    private readonly IItemStockQueryService _itemStockQueryService;
    private readonly IWarehouseService _warehouseService;

    public ItemsForm(
        IItemService itemService,
        IItemStockQueryService itemStockQueryService,
        IWarehouseService warehouseService)
    {
        InitializeComponent();
        _itemService = itemService;
        _itemStockQueryService = itemStockQueryService;
        _warehouseService = warehouseService;
    }

    private async Task LoadItems()
    {
        await LoadItemsGridAsync();
        await LoadWarehouseListsAsync();
    }

    private async Task LoadItemsGridAsync()
    {
        dgvItems.DataSource = await _itemStockQueryService.GetItemsInWarehousesAsync();
        dgvItems.Columns[ItemIdColumn].Visible = false;
        dgvItems.ReadOnly = true;
        dgvItems.AllowUserToDeleteRows = false;
    }

    private async Task LoadWarehouseListsAsync()
    {
        var warehouses = await _warehouseService.GetAllWarehousesAsync();

        BindWarehouses(clbWarehouses, warehouses);
        BindWarehouses(cmbWarehouse, warehouses.ToList());
    }

    private static void BindWarehouses(ListControl listControl, object dataSource)
    {
        listControl.DataSource = dataSource;
        listControl.DisplayMember = "Name";
        listControl.ValueMember = "Id";
    }

    private void LoadMeasurementUnits()
    {
        cmbUnit.Items.AddRange(MeasurementUnits);
        cmbUnit.SelectedIndex = 0;
    }

    private void ResetFormInput()
    {
        txtName.Text = string.Empty;
        txtCode.Text = string.Empty;
        txtQuantity.Text = string.Empty;
    }

    private async Task FillInputsFromItemAsync(int id)
    {
        var item = await _itemService.GetItemByIdAsync(id);
        txtCode.Text = item.Code;
        txtName.Text = item.Name;
        cmbUnit.SelectedItem = item.MeasurementUnit;
    }

    private bool TryReadNewItemInput(out int warehouseId, out int quantity)
    {
        warehouseId = 0;
        quantity = 0;

        if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtCode.Text))
        {
            Dialogs.ShowWarning("Please enter an item name and code.");
            return false;
        }

        if (cmbWarehouse.SelectedItem is not Warehouse selectedWarehouse)
        {
            Dialogs.ShowWarning("Please select a warehouse.");
            return false;
        }

        if (!int.TryParse(txtQuantity.Text, out quantity) || quantity <= 0)
        {
            Dialogs.ShowWarning("Please enter a valid quantity.");
            return false;
        }

        warehouseId = selectedWarehouse.Id;
        return true;
    }

    private List<int> GetCheckedWarehouseIdsAfter(ItemCheckEventArgs e)
    {
        var checkedIds = clbWarehouses.CheckedItems
                                      .Cast<Warehouse>()
                                      .Select(w => w.Id)
                                      .ToList();

        var changedId = ((Warehouse)clbWarehouses.Items[e.Index]).Id;

        if (e.NewValue == CheckState.Checked)
            checkedIds.Add(changedId);
        else
            checkedIds.Remove(changedId);

        return checkedIds;
    }

    private void ClearGrid()
    {
        dgvItems.DataSource = null;
        dgvItems.Rows.Clear();
        dgvItems.Columns.Clear();
        dgvItems.Refresh();
    }

    private async void btnAdd_Click(object sender, EventArgs e)
    {
        try
        {
            if (!TryReadNewItemInput(out var warehouseId, out var quantity))
                return;

            await _itemService.AddNewItemToWarehouseAsync(
                txtCode.Text,
                txtName.Text,
                warehouseId,
                quantity,
                cmbUnit.SelectedItem.ToString()
            );

            Dialogs.ShowSuccess("New item added successfully!");

            dgvItems.DataSource = await _itemStockQueryService.GetItemsByWarehousesAsync(new List<int> { warehouseId });
            ResetFormInput();
        }
        catch (Exception ex)
        {
            Dialogs.ShowError(ex);
        }
    }

    private async void btnUpdate_Click(object sender, EventArgs e)
    {
        if (dgvItems.GetSelectedId(ItemIdColumn) is not int id) return;

        await _itemService.UpdateItemAsync(id, txtCode.Text, txtName.Text, cmbUnit.SelectedItem.ToString());
        await LoadItems();
        ResetFormInput();
    }

    private async void dgvItems_Click(object sender, EventArgs e)
    {
        if (dgvItems.GetSelectedId(ItemIdColumn) is not int id) return;

        await FillInputsFromItemAsync(id);
    }

    private async void clbWarehouses_ItemCheck(object sender, ItemCheckEventArgs e)
    {
        await Task.Delay(UiStateSettleDelayMilliseconds);

        var selectedWarehouseIds = GetCheckedWarehouseIdsAfter(e);
        if (!selectedWarehouseIds.Any()) return;

        var items = await _itemStockQueryService.GetItemsByWarehousesAsync(selectedWarehouseIds);

        ClearGrid();
        dgvItems.DataSource = items;
    }

    private async void ItemsForm_Load(object sender, EventArgs e)
    {
        await LoadItems();
        LoadMeasurementUnits();
    }
}