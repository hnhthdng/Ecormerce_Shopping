using DataObject.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Service.IService
{
    public interface IOrderExportService
    {
        Task<byte[]> ExportToExcel(IEnumerable<Order> orders);
        //IActionResult ExportToPdf(IEnumerable<Order> orders);
    }
}
