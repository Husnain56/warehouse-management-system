using WarehouseManagementSystem.Business.Interfaces;

namespace WarehouseManagementSystem.Presenation.Forms;

public partial class SupplierForm : Form
{
    private readonly ISupplierService _supplierService;

    public SupplierForm(ISupplierService supplierService)
    {
        InitializeComponent();
        _supplierService = supplierService;
    }

    private async Task LoadSuppliers()
    {
        var suppliers = await _supplierService.GetAllSuppliersAsync();
        dgvSupplier.DataSource = null;
        dgvSupplier.DataSource = suppliers;
        ConfigureGrid();
    }

    private void ConfigureGrid()
    {
        dgvSupplier.Columns["Id"].Visible = false;
        dgvSupplier.AllowUserToAddRows = false;
        dgvSupplier.AllowUserToDeleteRows = false;
        dgvSupplier.ReadOnly = true;
    }

    private void ResetInputs()
    {
        txtSupplierName.Text = string.Empty;
        txtSupplierPhone.Text = string.Empty;
        txtSupplierEmail.Text = string.Empty;
        txtSupplierTax.Text = string.Empty;
        txtSupplierWebsite.Text = string.Empty;
        txtSupplierMobile.Text = string.Empty;
    }

    private async Task FillInputsFromSupplierAsync(int id)
    {
        var supplier = await _supplierService.GetSupplierByIdAsync(id);
        txtSupplierName.Text = supplier.Name;
        txtSupplierPhone.Text = supplier.Phone;
        txtSupplierEmail.Text = supplier.Email;
        txtSupplierMobile.Text = supplier.Mobile;
        txtSupplierTax.Text = supplier.Fax;
        txtSupplierWebsite.Text = supplier.Website;
    }

    private async void SupplierForm_Load(object sender, EventArgs e)
    {
        await LoadSuppliers();
    }

    private async void btnAddSupplier_Click(object sender, EventArgs e)
    {
        try
        {
            await _supplierService.AddSupplierAsync(txtSupplierName.Text, txtSupplierPhone.Text,
                txtSupplierEmail.Text, txtSupplierTax.Text, txtSupplierMobile.Text,
                txtSupplierWebsite.Text);

            await LoadSuppliers();
            ResetInputs();
        }
        catch (Exception ex)
        {
            Dialogs.ShowError(ex);
        }
    }

    private async void btnUpdateSupplier_Click(object sender, EventArgs e)
    {
        if (dgvSupplier.GetSelectedId() is not int id) return;

        try
        {
            await _supplierService.UpdateSupplierAsync(id, txtSupplierName.Text, txtSupplierPhone.Text,
                txtSupplierEmail.Text, txtSupplierTax.Text, txtSupplierMobile.Text, txtSupplierWebsite.Text);
            Dialogs.ShowSuccess("Supplier updated successfully!");
            await LoadSuppliers();
            ResetInputs();
        }
        catch (Exception ex)
        {
            Dialogs.ShowError(ex);
        }
    }

    private async void dgvSupplier_Click(object sender, EventArgs e)
    {
        if (dgvSupplier.GetSelectedId() is not int id) return;

        await FillInputsFromSupplierAsync(id);
    }

    private async void btnDeleteSupplier_Click(object sender, EventArgs e)
    {
        if (dgvSupplier.GetSelectedId() is not int id) return;
        if (!Dialogs.Confirm("Are you sure you want to delete this supplier?")) return;

        try
        {
            await _supplierService.DeleteSupplierAsync(id);
            Dialogs.ShowSuccess("supplier deleted successfully!");
            await LoadSuppliers();
        }
        catch (Exception ex)
        {
            Dialogs.ShowError(ex);
        }
    }
}