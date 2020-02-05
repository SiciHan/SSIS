﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Team8ADProjectSSIS.EmailModel;
using Team8ADProjectSSIS.DAO;
using Team8ADProjectSSIS.Models;
using Team8ADProjectSSIS.Report;

namespace Team8ADProjectSSIS.Controllers
{
    public class StoreManagerController : Controller
    {
        ItemDAO _itemDAO;
        SupplierItemDAO _supplieritemDAO;
        PurchaseOrderDAO _purchaseOrderDAO;
        PurchaseOrderDetailsDAO _purchaseOrderDetailsDAO;
        DisbursementDAO _disbursementDAO;
        DisbursementItemDAO _disbursementItemDAO;

        public StoreManagerController()
        {
            _itemDAO = new ItemDAO();
            _supplieritemDAO = new SupplierItemDAO();
            _purchaseOrderDAO = new PurchaseOrderDAO();
            _purchaseOrderDetailsDAO = new PurchaseOrderDetailsDAO();
            _disbursementDAO = new DisbursementDAO();
            _disbursementItemDAO = new DisbursementItemDAO();
        }

        // GET: StoreManager
        public ActionResult Home()
        {
            
            return View();
        }

        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult ItemsForSuppliers()
        {
            List<Item> items = _itemDAO.GetAllItems();
            ViewBag.items = items;
            return View();
        }

        public ActionResult Suppliers(int itemId)
        {
            List<SupplierItem> supplierItems = _supplieritemDAO.GetSuppliersById(itemId);
            List<Supplier> suppliers = new List<Supplier>();
            foreach(SupplierItem item in supplierItems)
            {
                Supplier temp = item.Supplier;
                suppliers.Add(temp);
            }
            ViewBag.suppliers = suppliers;
            return View();
        }

        public ActionResult POHistory()
        {
            List<PurchaseOrder> AllPO = _purchaseOrderDAO.FindAllPO();
            ViewData["AllPO"] = AllPO;
            return View();
        }

        [HttpPost]
        public ActionResult PODetails(int IdPurchaseOrder)
        {
            List<PurchaseOrderDetail> DetailPO = _purchaseOrderDetailsDAO.FindDetailPO(IdPurchaseOrder);
            ViewData["DetailPO"] = DetailPO;
            return View();
        }

        public ActionResult DisbursementHistory()
        {
            List<JoinDandDI> AllDisbursement = _disbursementDAO.FindAllDisbursement();
            ViewData["AllDisbursement"] = AllDisbursement;
            return View();
        }

        [HttpPost]
        public ActionResult DisbursementDetails(int IdDisbursement)
        {
            List<JoinDandDI> DetailDisbursement = _disbursementItemDAO.FindDetailDisbursement(IdDisbursement);
            ViewData["DetailDisbursement"] = DetailDisbursement;
            return View();
        }
        [HttpPost]
        public ActionResult ExportExcel()
        {

            List<Item> DownloadableData = _itemDAO.GetDownloadableData();
            ExcelReport excelReport = new ExcelReport();
            byte[] ExcelData = excelReport.GenerateExcelReport(DownloadableData);

            return File(ExcelData, "application/xlsx", "Ordered Data.xlsx");
        }
    }
}