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

    public ItemsForm CreateItemsForm()
    {
        var unitOfWork = CreateUnitOfWork();

        return new ItemsForm(
            new ItemService(unitOfWork),
            new ItemStockQueryService(unitOfWork),
            new WarehouseService(unitOfWork));
    }

    public SupplyOrderForm CreateSupplyOrderForm()
    {
        var unitOfWork = CreateUnitOfWork();

        return new SupplyOrderForm(
            new SupplyOrderService(unitOfWork),
            new WarehouseService(unitOfWork),
            new SupplierService(unitOfWork));
    }

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