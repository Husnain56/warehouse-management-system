using WarehouseManagementSystem.Business.Services;
using WarehouseManagementSystem.Data;
using WarehouseManagementSystem.Data.UOW;
using WarehouseManagementSystem.Data.UOW.Interfaces;
using WarehouseManagementSystem.Presenation.Forms;

namespace WarehouseManagementSystem.Presenation;

internal sealed class FormFactory
{
    public CustomerForm CreateCustomerForm() =>
        new(new CustomerService(CreateUnitOfWork()));

    public SupplierForm CreateSupplierForm() =>
        new(new SupplierService(CreateUnitOfWork()));

    public WarehouseForm CreateWarehouseForm() =>
        new(new WarehouseService(CreateUnitOfWork()));

    public StockTransferForm CreateStockTransferForm()
    {
        var unitOfWork = CreateUnitOfWork();

        return new StockTransferForm(
            new StockTransferService(unitOfWork),
            new ItemStockQueryService(unitOfWork),
            new WarehouseService(unitOfWork));
    }

    private static IUnitOfWork CreateUnitOfWork() =>
        new UnitOfWork(new WMSDbContext());
}