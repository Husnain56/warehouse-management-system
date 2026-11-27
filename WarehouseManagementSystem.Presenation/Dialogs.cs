namespace WarehouseManagementSystem.Presenation;

internal static class Dialogs
{
    public static void ShowError(Exception ex) =>
        ShowError(ex.Message);

    public static void ShowError(string message) =>
        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

    public static void ShowWarning(string message) =>
        MessageBox.Show(message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    public static void ShowInfo(string message) =>
        MessageBox.Show(message, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

    public static void ShowSuccess(string message) =>
        MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

    public static bool Confirm(string question) =>
        MessageBox.Show(question, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;

    public static void ReportGenerated(string pdfPath) =>
        ShowSuccess($"Report generated successfully!\nSaved at: {pdfPath}");

    public static void ReportFailed(Exception ex) =>
        ShowError($"Error generating report: {ex.Message}");
}