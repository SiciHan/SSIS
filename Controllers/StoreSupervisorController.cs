using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Team8ADProjectSSIS.DAO;
using Team8ADProjectSSIS.Models;
using Team8ADProjectSSIS.Report;

namespace Team8ADProjectSSIS.Controllers
{
    public class StoreSupervisorController : Controller
    {
        private readonly ItemDAO _itemDAO;

        public StoreSupervisorController() 
        {
            this._itemDAO = new ItemDAO();
        }
        // GET: StoreSupervisor
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DashBoard()
        {
            return View();
        }

        public ActionResult ExportExcel()
        {

            List<Item> DownloadableData = _itemDAO.GetDownloadableData();
            ExcelReport excelReport = new ExcelReport();
            byte[] ExcelData = excelReport.GenerateExcelReport(DownloadableData);

            return File(ExcelData, "application/xlsx", "Ordered Data.xlsx");
        }   
        public ActionResult PrintPDF()
        {
            return View();
        }
    }
}