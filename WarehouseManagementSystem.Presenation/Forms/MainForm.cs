using FontAwesome.Sharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WarehouseManagementSystem.Presenation.Forms;

namespace WarehouseManagementSystem.Presenation;

public partial class MainForm : Form
{
    private readonly FormFactory _formFactory;
    private IconButton currentBtn;
    private Panel leftBorderPanal;
    private Form currentChildForm;

    internal MainForm(FormFactory formFactory)
    {
        InitializeComponent();
        _formFactory = formFactory;
        leftBorderPanal = new Panel();
        leftBorderPanal.Size = new Size(7, 60);
        SidePanal.Controls.Add(leftBorderPanal);
    }

    private void ActivateBtn(object sender)
    {
        if (sender == null) return;

        DisableBtn();
        currentBtn = (IconButton)sender;
        StyleActiveButton();
        ShowLeftBorderBeside(currentBtn);
    }

    private void StyleActiveButton()
    {
        currentBtn.BackColor = Color.FromArgb(19, 48, 48, 48);
        currentBtn.IconColor = Color.FromArgb(192, 0, 0);
        currentBtn.TextAlign = ContentAlignment.MiddleCenter;
        currentBtn.TextImageRelation = TextImageRelation.TextBeforeImage;
        currentBtn.ImageAlign = ContentAlignment.MiddleRight;
    }

    private void ShowLeftBorderBeside(IconButton button)
    {
        leftBorderPanal.BackColor = Color.FromArgb(192, 0, 0);
        leftBorderPanal.Location = new Point(0, button.Location.Y);
        leftBorderPanal.Visible = true;
        leftBorderPanal.BringToFront();
    }

    private void DisableBtn()
    {
        if (currentBtn == null) return;

        currentBtn.BackColor = Color.Black;
        currentBtn.IconColor = Color.WhiteSmoke;
        currentBtn.TextAlign = ContentAlignment.MiddleLeft;
        currentBtn.TextImageRelation = TextImageRelation.ImageBeforeText;
        currentBtn.ImageAlign = ContentAlignment.MiddleLeft;
    }

    private void OpenChildForm(Form child)
    {
        if (currentChildForm != null)
        {
            panelContainer.Controls.Clear();
        }
        child.TopLevel = false;
        child.FormBorderStyle = FormBorderStyle.None;
        child.Dock = DockStyle.Fill;
        panelContainer.Controls.Add(child);
        child.BringToFront();
        child.Show();
    }

    private void btnWarehouse_Click(object sender, EventArgs e)
    {
        ActivateBtn(sender);
        OpenChildForm(_formFactory.CreateWarehouseForm());
    }

    private void btnItems_Click(object sender, EventArgs e)
    {
        ActivateBtn(sender);
        OpenChildForm(_formFactory.CreateItemsForm());
    }

    private void btnSupplier_Click(object sender, EventArgs e)
    {
        ActivateBtn(sender);
        OpenChildForm(_formFactory.CreateSupplierForm());
    }

    private void btnCustomer_Click(object sender, EventArgs e)
    {
        ActivateBtn(sender);
        OpenChildForm(_formFactory.CreateCustomerForm());
    }

    private void btnSupplyOrder_Click(object sender, EventArgs e)
    {
        ActivateBtn(sender);
        OpenChildForm(_formFactory.CreateSupplyOrderForm());
    }

    private void btnWithdrawalOrder_Click(Object sender, EventArgs e)
    {
        ActivateBtn(sender);
    }

    private void btnStockTransfer_Click(object sender, EventArgs e)
    {
        ActivateBtn(sender);
        OpenChildForm(_formFactory.CreateStockTransferForm());
    }

    private void btnWarehousRepot_Click(object sender, EventArgs e)
    {
        ActivateBtn(sender);
        OpenChildForm(_formFactory.CreateWarehouseStateReportForm());
    }

    private void iconButton1_Click(object sender, EventArgs e)
    {
        ActivateBtn(sender);
        OpenChildForm(_formFactory.CreateWarehousesItemsReportForm());
    }

    private void btnItemsInWarehousePeriodReport_Click(object sender, EventArgs e)
    {
        ActivateBtn(sender);
        OpenChildForm(_formFactory.CreateItemsInWarehousePeriodReportForm());
    }

    private void btnItemsCloseToExpiration_Click(object sender, EventArgs e)
    {
        ActivateBtn(sender);
        OpenChildForm(_formFactory.CreateItemsCloseToExpirationReportForm());
    }
}