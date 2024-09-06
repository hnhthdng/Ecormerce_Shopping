using DataAccess.Service.IService;
using DataObject.Model;

using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Service
{
    public class ExportService : IOrderExportService
    {
        public async Task<byte[]> ExportToExcel(IEnumerable<Order> orders)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Orders");

                // Add header row
                worksheet.Cells[1, 1].Value = "Order ID";
                worksheet.Cells[1, 2].Value = "Account";
                worksheet.Cells[1, 3].Value = "Customer";
                worksheet.Cells[1, 4].Value = "Order Date";
                worksheet.Cells[1, 5].Value = "Required Date";
                worksheet.Cells[1, 6].Value = "Shipped Date";
                worksheet.Cells[1, 7].Value = "Freight";
                worksheet.Cells[1, 8].Value = "Ship Address";

                // Add data rows
                int row = 2;
                foreach (var order in orders)
                {
                    worksheet.Cells[row, 1].Value = order.OrderID;
                    worksheet.Cells[row, 2].Value = order.Accounts.UserName;
                    worksheet.Cells[row, 3].Value = order.Customer.ContactName;
                    worksheet.Cells[row, 4].Value = order.OrderDate.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 5].Value = order.RequiredDate.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 6].Value = order.ShippedDate?.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 7].Value = order.Freight;
                    worksheet.Cells[row, 8].Value = order.ShipAddress;
                    row++;
                }

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Return the Excel file as a byte array
                return await Task.FromResult(package.GetAsByteArray());
            }
        }
    }
}