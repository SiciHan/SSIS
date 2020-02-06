using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Team8ADProjectSSIS.EmailModel;
using Team8ADProjectSSIS.DAO;
using Team8ADProjectSSIS.Models;
using Team8ADProjectSSIS.Report;
using Team8ADProjectSSIS.Filters;

namespace Team8ADProjectSSIS.Controllers
{

    [AuthenticateFilter]
    [AuthorizeFilter]
      
    public class StoreSupervisorController : Controller
    {
        StockRecordDAO _stockRecordDAO;
        ItemDAO _itemDAO;
        PurchaseOrderDAO _purchaseOrderDAO;
        PurchaseOrderDetailsDAO _purchaseOrderDetailsDAO;
        NotificationChannelDAO _notificationChannelDAO;
        public StoreSupervisorController()
        {
            this._stockRecordDAO = new StockRecordDAO();
            this._itemDAO = new ItemDAO();
            this._purchaseOrderDAO = new PurchaseOrderDAO();
            this._purchaseOrderDetailsDAO = new PurchaseOrderDetailsDAO();
            _notificationChannelDAO = new NotificationChannelDAO();
        }
          
        public ActionResult Notification()
        {
            int IdReceiver = 1;
            if (Session["IdEmployee"] != null)
            {
                IdReceiver = (int)Session["IdEmployee"];
            }
            ViewData["NCs"] = _notificationChannelDAO.FindAllNotificationsByIdReceiver(IdReceiver);

            return View();
        }
          
        public ActionResult Voucher()
        {
            List<StockRecord> vouchers = _stockRecordDAO.FindVoucherForSupervisor();
            List<float> prices = new List<float>();
            foreach (StockRecord voucher in vouchers)
            {
                float price = _itemDAO.FindPriceById(voucher.IdItem);
                prices.Add(price);
            }
            ViewData["prices"] = prices;
            ViewData["vouchers"] = vouchers;
            return View();
        }
          
        public ActionResult VoucherHistory()
        {
            List<StockRecord> vouchers = _stockRecordDAO.FindJudgedVoucherForSupervisor();
            List<float> prices = new List<float>();
            List<string> status = new List<string>();
            foreach (StockRecord voucher in vouchers)
            {
                float price = _itemDAO.FindPriceById(voucher.IdItem);
                prices.Add(price);
                if (voucher.IdOperation == 7 || voucher.IdOperation == 9 || voucher.IdOperation == 12 || voucher.IdOperation == 14)
                {
                    status.Add("Approved");
                }
                else
                {
                    status.Add("Rejected");
                }
            }
            ViewData["prices"] = prices;
            ViewData["vouchers"] = vouchers;
            ViewData["status"] = status;
            return View();
        }
          
        public ActionResult PurchaseOrder()
        {
            List<PurchaseOrder> pendingPOs = _purchaseOrderDAO.FindPendingPO();
            ViewData["pengding"] = pendingPOs;
            return View();
        }
          
        public ActionResult POHistory()
        {
            List<PurchaseOrder> handledPOs = _purchaseOrderDAO.FindHandledPO();
            ViewData["handledPOs"] = handledPOs;
            return View();
        }
          
        public ActionResult PurchaseOrderDetail(int idPurchaseOrder)
        {
            List<PurchaseOrderDetail> PODetails = _purchaseOrderDetailsDAO.FindDetailPO(idPurchaseOrder);
            PurchaseOrder po = _purchaseOrderDAO.FindPOById(idPurchaseOrder);
            ViewData["POD"] = PODetails;
            ViewBag.po = po;
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
          
        [HttpPost]
        public ActionResult HandlePO(string handle, List<int> purchase_ordersId, string remarks)
        {
            List<PurchaseOrder> purchaseOrders = new List<PurchaseOrder>();
            foreach (int id in purchase_ordersId)
            {
                PurchaseOrder po = _purchaseOrderDAO.FindPOById(id);
                purchaseOrders.Add(po);
            }
            if (handle == "Approve")
            {
                _purchaseOrderDAO.UpdatePOToApproved(purchaseOrders);
            }
            else
            {
                _purchaseOrderDAO.UpdatePOToRejected(purchaseOrders, remarks);
            }
            return RedirectToAction("PurchaseOrder", "StoreSupervisor");
        }
          
        [HttpPost]
        public ActionResult Handlejustment(string handle, List<int> vouchersId)
        {
            List<StockRecord> vouchers = new List<StockRecord>();
            foreach (int id in vouchersId)
            {
                StockRecord voucher = _stockRecordDAO.FindById(id);
                vouchers.Add(voucher);
            }
            if (handle == "Approve")
            {
                _stockRecordDAO.UpdateVoucherToApproved(vouchers);
            }
            else
            {
                _stockRecordDAO.UpdateVoucherToRejected(vouchers);
            }
            return RedirectToAction("Voucher", "StoreSupervisor");
        }
    }
}