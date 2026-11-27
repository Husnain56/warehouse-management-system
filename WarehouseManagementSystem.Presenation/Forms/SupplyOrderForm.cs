using WarehouseManagementSystem.Business.Interfaces;
using WarehouseManagementSystem.Core.DTOs;

namespace WarehouseManagementSystem.Presenation.Forms;

public partial class SupplyOrderForm : Form
{
    private readonly ISupplyOrderService _supplyOrderService;
    private readonly IWarehouseService _warehouseService;
    private readonly ISupplierService _supplierService;

    public SupplyOrderForm(
        ISupplyOrderService supplyOrderService,
        IWarehouseService warehouseService,
        ISupplierService supplierService)
    {
        InitializeComponent();

        _supplyOrderService = supplyOrderService;
        _warehouseService = warehouseService;
        _supplierService = supplierService;
    }

    private async Task LoadData()
    {
        dgvSupplyOrder.DataSource = await _supplyOrderService.GetAllSupplyOrdersDetailsAsync();

        BindLookup(cmbWarehouses, await _warehouseService.GetAllWarehousesAsync());
        BindLookup(cmbSuppliers, await _supplierService.GetAllSuppliersAsync());
    }

    private static void BindLookup(ComboBox comboBox, object dataSource)
    {
        comboBox.DataSource = null;
        comboBox.DataSource = dataSource;
        comboBox.DisplayMember = "Name";
        comboBox.ValueMember = "Id";
    }

    private void SetupDataGridView()
    {
        dgvSupplyOrder.AutoGenerateColumns = false;

        dgvSupplyOrder.Columns.Clear();

        AddColumn("OrderNumber", "Order #");
        AddColumn("OrderDate", "Date");
        AddColumn("SupplierName", "Supplier");
        AddColumn("WarehouseName", "Warehouse");
        AddColumn("ItemName", "Item");
        AddColumn("Quantity", "Quantity");
        AddColumn("ProductionDate", "Production Date");
        AddColumn("ExpirationDate", "Expiration Date");

        dgvSupplyOrder.AllowUserToAddRows = false;
        dgvSupplyOrder.ReadOnly = true;
    }

    private void AddColumn(string dataPropertyName, string headerText)
    {
        dgvSupplyOrder.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = dataPropertyName,
            HeaderText = headerText
        });
    }

    private bool HasItemCodeAndName() =>
        !string.IsNullOrWhiteSpace(txtItemCode.Text) && !string.IsNullOrWhiteSpace(txtItemName.Text);

    private SupplyOrderDto BuildOrderFromInputs()
    {
        return new SupplyOrderDto
        {
            SupplierId = (int)cmbSuppliers.SelectedValue,
            WarehouseId = (int)cmbWarehouses.SelectedValue,
            ItemCode = txtItemCode.Text.Trim(),
            ItemName = txtItemName.Text.Trim(),
            MeasurementUnit = cmbMeasurementUnit.Text,
            Quantity = Convert.ToInt32(txtItemQty.Text),
            ProductionDate = dtpProductionDate.Value,
            ExpirationDate = dtpExpirationDate.Value,
        };
    }

    private async void SupplyOrderForm_Load(object sender, EventArgs e)
    {
        await LoadData();
        SetupDataGridView();
    }

    private async void btnAddOrder_Click(object sender, EventArgs e)
    {
        if (!HasItemCodeAndName())
        {
            Dialogs.ShowWarning("Please enter an item code and name.");
            return;
        }

        var orderDto = BuildOrderFromInputs();

        try
        {
            await _supplyOrderService.AddSupplyOrderAsync(orderDto);
            Dialogs.ShowSuccess("Supply Order added successfully!");
        }
        catch (Exception ex)
        {
            Dialogs.ShowError(ex);
        }
    }
}