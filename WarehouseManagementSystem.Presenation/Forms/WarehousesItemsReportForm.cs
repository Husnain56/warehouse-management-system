using WarehouseManagementSystem.Business.Interfaces;
using WarehouseManagementSystem.Data.Models;

namespace WarehouseManagementSystem.Presenation.Forms;

public partial class WarehousesItemsReportForm : Form
{
    private readonly IWarehouseService _warehouseService;
    private readonly IWarehouseReportService _warehouseReportService;

    public WarehousesItemsReportForm(IWarehouseService warehouseService, IWarehouseReportService warehouseReportService)
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
        clbWarehouses.DataSource = await _warehouseService.GetAllWarehousesAsync();
        clbWarehouses.DisplayMember = "Name";
        clbWarehouses.ValueMember = "Id";
    }

    private async void btnGenerateReport_Click(object sender, EventArgs e)
    {
        try
        {
            var selectedWarehouseIds = clbWarehouses.CheckedItems
                 .Cast<Warehouse>()
                 .Select(warehouse => warehouse.Id)
                 .ToList();

            if (selectedWarehouseIds.Count == 0)
            {
                Dialogs.ShowWarning("Please select at least one warehouse.");
                return;
            }

            var pdfPath = ReportPdfWriter.Write(
                "WarehousesItemsReport.rdlc",
                "WarehousesItemsDataSet",
                await _warehouseReportService.GetItemsReport(selectedWarehouseIds),
                $"WarehouseItemsReport_{string.Join("-", selectedWarehouseIds)}.pdf");

            Dialogs.ReportGenerated(pdfPath);
        }
        catch (Exception ex)
        {
            Dialogs.ReportFailed(ex);
        }
    }
}