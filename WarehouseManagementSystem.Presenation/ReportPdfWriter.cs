using AspNetCore.Reporting;
using System.Data;

namespace WarehouseManagementSystem.Presenation;

internal static class ReportPdfWriter
{
    private const string ReportsFolder = "Reports";

    public static string Write(string rdlcFileName, string dataSetName, DataTable data, string pdfFileName)
    {
        var report = new LocalReport(Path.Combine(Application.StartupPath, ReportsFolder, rdlcFileName));
        report.AddDataSource(dataSetName, data);

        var pdfPath = Path.Combine(Application.StartupPath, pdfFileName);
        File.WriteAllBytes(pdfPath, report.Execute(RenderType.Pdf).MainStream);

        return pdfPath;
    }
}