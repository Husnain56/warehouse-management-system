using WarehouseManagementSystem.Business.Interfaces;

namespace WarehouseManagementSystem.Presenation.Forms;

public partial class CustomerForm : Form
{
    private readonly ICustomerService _customerService;

    public CustomerForm(ICustomerService customerService)
    {
        InitializeComponent();

        _customerService = customerService;
    }

    private async Task LoadCustomers()
    {
        dgvCustomers.DataSource = await _customerService.GetAllCustomersAsync();
        ConfigureGrid();
    }

    private void ConfigureGrid()
    {
        dgvCustomers.Columns["Id"].Visible = false;
        dgvCustomers.Columns["WithdrawalOrders"].Visible = false;
        dgvCustomers.AllowUserToAddRows = false;
        dgvCustomers.AllowUserToDeleteRows = false;
        dgvCustomers.ReadOnly = true;
    }

    private void ResetFormInput()
    {
        txtName.Text = string.Empty;
        txtEmail.Text = string.Empty;
        txtPhone.Text = string.Empty;
    }

    private async Task FillInputsFromCustomerAsync(int id)
    {
        var customer = await _customerService.GetCustomerByIdAsync(id);
        txtName.Text = customer.Name;
        txtPhone.Text = customer.Phone;
        txtEmail.Text = customer.Email;
    }

    private int? GetSelectedCustomerId() =>
        dgvCustomers.SelectedRows.Count > 0
            ? Convert.ToInt32(dgvCustomers.SelectedRows[0].Cells["Id"].Value)
            : null;

    private static void ShowError(Exception ex) =>
        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

    private static void ShowSuccess(string message) =>
        MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

    private static bool ConfirmDelete() =>
        MessageBox.Show("Are you sure you want to delete this customer?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;

    private async void CustomerForm_Load(object sender, EventArgs e)
    {
        await LoadCustomers();
    }

    private async void btnAdd_Click(object sender, EventArgs e)
    {
        try
        {
            await _customerService.AddCustomerAsync(txtName.Text, txtPhone.Text, txtEmail.Text);
            await LoadCustomers();
            ResetFormInput();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private async void btnUpdate_Click(object sender, EventArgs e)
    {
        if (GetSelectedCustomerId() is not int id) return;

        try
        {
            await _customerService.UpdateCustomerAsync(id, txtName.Text, txtPhone.Text, txtEmail.Text);
            ShowSuccess("Customer updated successfully!");
            await LoadCustomers();
            ResetFormInput();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private async void dgvCustomers_Click(object sender, EventArgs e)
    {
        if (GetSelectedCustomerId() is not int id) return;

        await FillInputsFromCustomerAsync(id);
    }

    private async void btnDelete_Click(object sender, EventArgs e)
    {
        if (GetSelectedCustomerId() is not int id) return;
        if (!ConfirmDelete()) return;

        try
        {
            await _customerService.DeleteCustomerAsync(id);
            ShowSuccess("customer deleted successfully!");
            await LoadCustomers();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }
}