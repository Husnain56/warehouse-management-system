namespace WarehouseManagementSystem.Presenation;

internal static class Dialogs
{
    public static void ShowError(Exception ex) =>
        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

    public static void ShowSuccess(string message) =>
        MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

    public static bool Confirm(string question) =>
        MessageBox.Show(question, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
}