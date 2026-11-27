using WarehouseManagementSystem.Business.Interfaces;

namespace WarehouseManagementSystem.Presenation.Forms;

public partial class WarehouseStateReportForm : Form
{
    private readonly IWarehouseService _warehouseService;
    private readonly IWarehouseReportService _warehouseReportService;

    public WarehouseStateReportForm(IWarehouseService warehouseService, IWarehouseReportService warehouseReportService)
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
        comboBoxWarehouses.DataSource = await _warehouseService.GetAllWarehousesAsync();
        comboBoxWarehouses.DisplayMember = "Name";
        comboBoxWarehouses.ValueMember = "Id";
    }

    private async void btnGenerateReport_Click_1(object sender, EventArgs e)
    {
        try
        {
            int warehouseId = (int)comboBoxWarehouses.SelectedValue;

            var pdfPath = ReportPdfWriter.Write(
                "WarehouseStatusReport.rdlc",
                "WarehouseStatusDataSet",
                await _warehouseReportService.GetWarehouseStatusReport(warehouseId),
                $"WarehouseStatusReport_{warehouseId}.pdf");

            Dialogs.ReportGenerated(pdfPath);
        }
        catch (Exception ex)
        {
            Dialogs.ReportFailed(ex);
        }
    }
}