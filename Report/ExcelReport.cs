using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.Report
{
    public class ExcelReport
    {
        #region Declaration
        List<Item> _downloadableData = new List<Item>();
        #endregion

        public byte[] GenerateExcelReport(List<Item> DownloadableData)
        {
            _downloadableData = DownloadableData;

            ExcelPackage excel = new ExcelPackage();
            ExcelWorksheet ws = excel.Workbook.Worksheets.Add("Data Report");

            ws.Cells["A1"].Value = "Logic University";
            ws.Cells["A2"].Value = "Data Report";
            ws.Cells["A3"].Value = "Date";
            ws.Cells["B3"].Value = string.Format("{0:dd MMMM yyyy} at {0:H: mm tt}", DateTimeOffset.Now);

            ws.Cells["A5"].Value = "Item Category";
            ws.Cells["B5"].Value = "Supplier Code";
            ws.Cells["C5"].Value = "Requested Quantity";
            ws.Cells["D5"].Value = "Supplier Price";
            ws.Cells["E5"].Value = "Approval Date";
            ws.Cells["F5"].Value = "Department";

            int RowStart = 6;
            foreach (var i in DownloadableData)
            {
                ws.Cells[string.Format("A{0}", RowStart)].Value = i.Category.Label.ToString();
                ws.Cells[string.Format("B{0}", RowStart)].Value = i.Supplier1.CodeSupplier;
                ws.Cells[string.Format("C{0}", RowStart)].Value = i.RequisitionItems.Select(x => x.Unit);
                ws.Cells[string.Format("D{0}", RowStart)].Value = i.Supplier1.SupplierItems.Select(x => x.Price);
                ws.Cells[string.Format("E{0}", RowStart)].Value = i.RequisitionItems.Select(x => x.Requisition.ApprovedDate.ToString("dd-MM-yyyy"));
                ws.Cells[string.Format("F{0}", RowStart)].Value = i.RequisitionItems.Select(x => x.Requisition.Employee.CodeDepartment);
                RowStart++;
            }

            ws.Cells["A"].AutoFitColumns();
            ws.Cells["B"].AutoFitColumns();
            ws.Cells["C"].AutoFitColumns();
            ws.Cells["D"].AutoFitColumns();
            ws.Cells["E"].AutoFitColumns();
            ws.Cells["F"].AutoFitColumns();

            return excel.GetAsByteArray();
        }
    }
}