using System;
using System.Collections.Generic;
using System.Data;

namespace WarehouseManagementSystem.Business.Interfaces;


public interface IWarehouseReportService
{
    Task<DataTable> GetWarehouseStatusReport(int warehouseId);
    Task<DataTable> GetItemsReport(List<int> warehouseIds);
    Task<DataTable> GetItemsInWarehouseForPeriod(int warehouseId, DateTime fromDate, DateTime toDate);
}