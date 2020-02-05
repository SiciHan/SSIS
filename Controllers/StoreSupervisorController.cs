using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Team8ADProjectSSIS.EmailModel;
using Team8ADProjectSSIS.DAO;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.Controllers
{
    public class StoreSupervisorController : Controller
    {
        StockRecordDAO _stockRecordDAO;
        ItemDAO _itemDAO;
        PurchaseOrderDAO _purchaseOrderDAO;
        PurchaseOrderDetailsDAO _purchaseOrderDetailsDAO;

        public StoreSupervisorController()
        {
            this._stockRecordDAO = new StockRecordDAO();
            this._itemDAO = new ItemDAO();
            this._purchaseOrderDAO = new PurchaseOrderDAO();
            this._purchaseOrderDetailsDAO = new PurchaseOrderDetailsDAO();
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
            ViewData["POD"] = PODetails;
            return View();
        }
        // GET: StoreSupervisor
    }
}