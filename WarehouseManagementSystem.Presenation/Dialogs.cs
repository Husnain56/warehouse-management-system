namespace WarehouseManagementSystem.Presenation;

internal static class Dialogs
{
    public static void ShowError(Exception ex) =>
        ShowError(ex.Message);

    public static void ShowError(string message) =>
        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

    public static void ShowWarning(string message) =>
        MessageBox.Show(message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    public static void ShowSuccess(string message) =>
        MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

    public static bool Confirm(string question) =>
        MessageBox.Show(question, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
}