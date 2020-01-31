using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Team8ADProjectSSIS.DAO;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.Controllers
{
    public class StoreClerkController : Controller
    {
        private readonly DisbursementDAO _disbursementDAO;
        private readonly RequisitionDAO _requisitionDAO;
        private readonly RequisitionItemDAO _requisitionItemDAO;
        private readonly StockRecordDAO _stockRecordDAO;
        private readonly DisbursementItemDAO _disbursementItemDAO;
        private readonly PurchaseOrderDAO _purchaseOrderDAO;
        private readonly ItemDAO _itemDAO;
        public StoreClerkController()
        {
            this._disbursementDAO = new DisbursementDAO();
            this._requisitionDAO = new RequisitionDAO();
            this._requisitionItemDAO = new RequisitionItemDAO();
            this._stockRecordDAO = new StockRecordDAO();
            this._disbursementItemDAO = new DisbursementItemDAO();
            this._purchaseOrderDAO = new PurchaseOrderDAO();
            this._itemDAO = new ItemDAO();
        }
        

        // GET: StoreClerk
        public ActionResult Index()
        {
            return View();
        }
        // Get: FormRetrieve Method
        public ActionResult FormRetrieve()
        {
            // Assume ClerkID
            int IdStoreClerk = 3;

            // Get Department that seleceted same collection point as store clerk
            List<string> DClerk = _disbursementDAO.ReturnStoreClerkCP(IdStoreClerk);

            bool NoDisbursement = false;
            DateTime Today = DateTime.Now;
            DateTime LastThu = Today.AddDays(-3);
            while (LastThu.DayOfWeek != DayOfWeek.Thursday)
                LastThu = LastThu.AddDays(-1);

            // If Disbursement contain status "preparing" from last thurseday to today
            if (_disbursementDAO.CheckExistDisbursement(DClerk, Today, LastThu))
            {
                // Search Disbursement with status set as "preparing"
                List<Retrieval> RetrievalForm = _disbursementDAO.RetrievePreparingItem(DClerk);
                ViewData["RetrievalForm"] = RetrievalForm;
                NoDisbursement = false;
                ViewData["NoDisbursement"] = NoDisbursement;

            }
            // If Disbursement contain no status "preparing" from last thursday to today
            else
            {
                NoDisbursement = true;
                ViewData["NoDisbursement"] = NoDisbursement;
                ViewBag.Today = Today.ToString("dd-MM-yyyy");
                ViewBag.LastThu = LastThu.ToString("dd-MM-yyyy");
            }


            return View();
        }
        // Post Method
        [HttpPost]
        public ActionResult FormRetrieve(string StartDate, string EndDate)
        {
            // Assume ClerkID
            int IdStoreClerk = 3;

            // Get Department that seleceted same collection point as store clerk
            List<string> DClerk = _disbursementDAO.ReturnStoreClerkCP(IdStoreClerk);

            if (StartDate.Equals("") || EndDate.Equals(""))
            {
                return RedirectToAction("FormRetrieve", "StoreClerk");
            }
            DateTime SDate = DateTime.ParseExact(StartDate, "dd-MM-yyyy", 
                            System.Globalization.CultureInfo.InvariantCulture);
            DateTime EDate = DateTime.ParseExact(EndDate, "dd-MM-yyyy",
                            System.Globalization.CultureInfo.InvariantCulture);
            // Search and retrieve Requisition & Requisition Item
            List<Retrieval> RetrievalItem = _requisitionDAO
                                            .RetrieveRequisition(DClerk, SDate, EDate);
            // Check if the the RetrievalForm has been created
            List<Retrieval> NewRetrievalItem = _disbursementDAO.CheckRetrievalFormExist(RetrievalItem);

            // Create Disbursement and set status to "preparing"
            List<int> PKDisbursement = _disbursementDAO.CreateDisbursement(NewRetrievalItem);
            // Create DisbursementItem and set status to "preparing"
            List<int> PKDisbursementItem = _disbursementItemDAO
                                           .CreateDisbursementItem(PKDisbursement, NewRetrievalItem);
            // Search Disbursement with status set as "preparing"
            List<Retrieval> RetrievalForm = _disbursementDAO.RetrievePreparingItem(DClerk);
            ViewData["RetrievalForm"] = RetrievalForm;
            ViewData["NoDisbursement"] = false;

            return View();
        }

        
        [HttpPost]
        public ActionResult SaveDisbursement(int[] IdItemRetrieved) 
        {
            // Assume ClerkID
            int IdStoreClerk = 3;

            // Get Department that seleceted same collection point as store clerk
            List<string> DClerk = _disbursementDAO.ReturnStoreClerkCP(IdStoreClerk);

            if (IdItemRetrieved.Any())
            {
                // update disbursementitem and set status to "prepared"
                // return IdDisbursement with at lease one items have been set as "prepared"
                List<int> IdDisbursement = _disbursementItemDAO.UpdateDisbursementItem(DClerk, IdItemRetrieved);
                // update disbursement and set status to "prepared"
                _disbursementDAO.UpdateDisbursement(IdDisbursement);
                // update item stock unit

                // raise alert

            }
            else
            {
                return RedirectToAction("FormRetrieve", "StoreClerk");
            }
            return RedirectToAction("FormRetrieve", "StoreClerk");

        } 
        public ActionResult PurchaseOrderList()
        {
            ViewData["Incomplete"]=_purchaseOrderDAO.FindIncompletePO();
            ViewData["Pending"]=_purchaseOrderDAO.FindPendingPO();
            ViewData["Rejected"]=_purchaseOrderDAO.FindRejectedPO();
            ViewData["Approved"]=_purchaseOrderDAO.FindApprovedPO();
            ViewData["Delivered"]=_purchaseOrderDAO.FindDeliveredPO();
            ViewData["Cancelled"]=_purchaseOrderDAO.FindCancelledPO();
            ViewData["LowStock"] = _itemDAO.FindLowStockItems();
            return View();
        }
        public ActionResult MakePurchaseOrder(string searchStr)
        {
            if (!string.IsNullOrEmpty(searchStr))
            {
                ViewData["SearchResult"]= _itemDAO.FindItemsByKeyword(searchStr);
                ViewData["SearchStr"] = searchStr;
            }
            ViewData["LowStock"] = _itemDAO.FindLowStockItems();
            return View();
        }
    }
}