using WarehouseManagementSystem.Business.Interfaces;

namespace WarehouseManagementSystem.Presenation.Forms;

public partial class ItemsCloseToExpirationReport : Form
{
    private readonly IItemStockQueryService _itemStockQueryService;

    public ItemsCloseToExpirationReport(IItemStockQueryService itemStockQueryService)
    {
        InitializeComponent();
        _itemStockQueryService = itemStockQueryService;
    }

    private async void btnGenerateReport_Click(object sender, EventArgs e)
    {
        try
        {
            int daysThreshold = (int)nudDaysThreshold.Value;

            if (daysThreshold <= 0)
            {
                Dialogs.ShowWarning("Please enter a valid number of days.");
                return;
            }

            var dataTable = await _itemStockQueryService.GetItemsCloseToExpirationAsync(daysThreshold);

            if (dataTable.Rows.Count == 0)
            {
                Dialogs.ShowInfo("No items found close to expiration.");
                return;
            }

            var pdfPath = ReportPdfWriter.Write(
                "ItemsCloseToExpirationReport.rdlc",
                "ItemsCloseToExpirationDataSet",
                dataTable,
                $"ItemsCloseToExpirationReport_{DateTime.Now:yyyyMMddHHmmss}.pdf");

            Dialogs.ReportGenerated(pdfPath);
        }
        catch (Exception ex)
        {
            Dialogs.ReportFailed(ex);
        }
    }
}