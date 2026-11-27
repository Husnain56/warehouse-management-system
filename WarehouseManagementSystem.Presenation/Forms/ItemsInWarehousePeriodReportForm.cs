using WarehouseManagementSystem.Business.Interfaces;
using WarehouseManagementSystem.Data.Models;

namespace WarehouseManagementSystem.Presenation.Forms;

public partial class ItemsInWarehousePeriodReportForm : Form
{
    private readonly IWarehouseService _warehouseService;
    private readonly IWarehouseReportService _warehouseReportService;

    public ItemsInWarehousePeriodReportForm(IWarehouseService warehouseService, IWarehouseReportService warehouseReportService)
    {
        InitializeComponent();
        _warehouseService = warehouseService;
        _warehouseReportService = warehouseReportService;
    }

    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        await LoadWarehouses();
    }

    private async Task LoadWarehouses()
    {
        cmbWarehouses.DataSource = await _warehouseService.GetAllWarehousesAsync();
        cmbWarehouses.DisplayMember = "Name";
        cmbWarehouses.ValueMember = "Id";
    }

    private async void btnGenerateReport_Click(object sender, EventArgs e)
    {
        try
        {
            if (cmbWarehouses.SelectedItem is not Warehouse selectedWarehouse)
            {
                Dialogs.ShowWarning("Please select a warehouse.");
                return;
            }

            var dataTable = await _warehouseReportService.GetItemsInWarehouseForPeriod(
                selectedWarehouse.Id, dtpFromDate.Value, dtpToDate.Value);

            if (dataTable.Rows.Count == 0)
            {
                Dialogs.ShowInfo("No items found for the selected warehouse and period.");
                return;
            }

            var pdfPath = ReportPdfWriter.Write(
                "ItemsInWarehousePeriodReport.rdlc",
                "ItemsInWarehousePeriodDataSet",
                dataTable,
                $"ItemsInWarehouseForPeriodReport_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

            Dialogs.ReportGenerated(pdfPath);
        }
        catch (Exception ex)
        {
            Dialogs.ReportFailed(ex);
        }
    }
}