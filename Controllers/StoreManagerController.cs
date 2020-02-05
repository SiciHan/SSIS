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

    [AuthorizeFilter]
    [AuthenticateFilter]
    public class StoreManagerController : Controller
    {
        private readonly ItemDAO _itemDAO;
        private readonly SupplierItemDAO _supplieritemDAO;
        private readonly PurchaseOrderDAO _purchaseOrderDAO;
        private readonly PurchaseOrderDetailsDAO _purchaseOrderDetailsDAO;
        private readonly DisbursementDAO _disbursementDAO;
        private readonly DisbursementItemDAO _disbursementItemDAO;
        private readonly StockRecordDAO _stockRecordDAO;
        private readonly NotificationChannelDAO _notificationChannelDAO;

        public StoreManagerController()
        {
            _itemDAO = new ItemDAO();
            _supplieritemDAO = new SupplierItemDAO();
            _purchaseOrderDAO = new PurchaseOrderDAO();
            _purchaseOrderDetailsDAO = new PurchaseOrderDetailsDAO();
            _disbursementDAO = new DisbursementDAO();
            _disbursementItemDAO = new DisbursementItemDAO();
            _stockRecordDAO = new StockRecordDAO();
            _notificationChannelDAO = new NotificationChannelDAO();
        }

        // GET: StoreManager
        public ActionResult Home()
        {
            
            return View();
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

        public ActionResult Voucher()
        {
            List<StockRecord> vouchers = _stockRecordDAO.FindVoucher();
            List<float> prices = new List<float>(); 
            foreach(StockRecord voucher in vouchers)
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
            List<StockRecord> vouchers = _stockRecordDAO.FindJudgedVoucher();
            List<float> prices = new List<float>();
            List<string> status = new List<string>();
            foreach (StockRecord voucher in vouchers)
            {
                float price = _itemDAO.FindPriceById(voucher.IdItem);
                prices.Add(price);
                if(voucher.IdOperation == 7 || voucher.IdOperation == 9 || voucher.IdOperation == 12 || voucher.IdOperation == 14)
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

        [HttpPost]
        public ActionResult JudgeAdjustment(string judge, List<StockRecord> vouchers)
        {
            if(judge == "Approve")
            {
                _stockRecordDAO.UpdateVoucherToApproved(vouchers);
            }
            else
            {
                _stockRecordDAO.UpdateVoucherToRejected(vouchers);
            }
            return RedirectToAction("Voucher", "StoreManager");
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