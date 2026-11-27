using WarehouseManagementSystem.Business.Interfaces;

namespace WarehouseManagementSystem.Presenation.Forms;

public partial class WarehouseForm : Form
{
    private readonly IWarehouseService _warehouseService;

    public WarehouseForm(IWarehouseService warehouseService)
    {
        InitializeComponent();
        _warehouseService = warehouseService;
    }

    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        await LoadWarehouses();
    }

    private async Task LoadWarehouses()
    {
        dgvWarehouses.DataSource = await _warehouseService.GetAllWarehousesAsync();
        ConfigureGrid();
    }

    private void ConfigureGrid()
    {
        dgvWarehouses.Columns["Id"].Visible = false;
        dgvWarehouses.AllowUserToDeleteRows = false;
        dgvWarehouses.ReadOnly = true;
    }

    private void ResetFormInput()
    {
        txtName.Text = string.Empty;
        txtAddress.Text = string.Empty;
        txtManager.Text = string.Empty;
    }

    private async Task FillInputsFromWarehouseAsync(int id)
    {
        var warehouse = await _warehouseService.GetWarehouseByIdAsync(id);
        txtManager.Text = warehouse.Manager;
        txtName.Text = warehouse.Name;
        txtAddress.Text = warehouse.Address;
    }

    private async void btnAdd_Click(object sender, EventArgs e)
    {
        try
        {
            await _warehouseService.AddWarehouseAsync(txtName.Text, txtAddress.Text, txtManager.Text);
            await LoadWarehouses();
            ResetFormInput();
        }
        catch (Exception ex)
        {
            Dialogs.ShowError(ex);
        }
    }

    private async void btnUpdate_Click(object sender, EventArgs e)
    {
        if (dgvWarehouses.GetSelectedId() is not int id) return;

        try
        {
            await _warehouseService.UpdateWarehouseAsync(id, txtName.Text, txtAddress.Text, txtManager.Text);
            Dialogs.ShowSuccess("Warehouse updated successfully!");
            await LoadWarehouses();
            ResetFormInput();
        }
        catch (Exception ex)
        {
            Dialogs.ShowError(ex);
        }
    }

    private async void btnDelete_Click(object sender, EventArgs e)
    {
        if (dgvWarehouses.GetSelectedId() is not int id) return;
        if (!Dialogs.Confirm("Are you sure you want to delete this warehouse?")) return;

        try
        {
            await _warehouseService.DeleteWarehouseAsync(id);
            Dialogs.ShowSuccess("Warehouse deleted successfully!");
            await LoadWarehouses();
        }
        catch (Exception ex)
        {
            Dialogs.ShowError(ex);
        }
    }

    private async void dgvWarehouses_Click(object sender, EventArgs e)
    {
        if (dgvWarehouses.GetSelectedId() is not int id) return;

        await FillInputsFromWarehouseAsync(id);
    }
}