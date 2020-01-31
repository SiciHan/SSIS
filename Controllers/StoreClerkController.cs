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


            bool NoDisbursement = false;
            DateTime Today = DateTime.Now;
            DateTime LastThu = DateTime.Now.AddDays(-1);
            while (LastThu.DayOfWeek != DayOfWeek.Thursday)
                LastThu = LastThu.AddDays(-1);
            // If Disbursement contain status "preparing" from last thurseday to today
            if (_disbursementDAO.CheckExistDisbursement(IdStoreClerk, Today, LastThu))
            {
                // Search Disbursement with status set as "preparing"
                // Search Disbursement with status set as "preparing"
                List<int> SelectedRequisition = _requisitionDAO
                                                .SearchRequisitionForRetrival(Today, LastThu);
                
                NoDisbursement = false;
                ViewData["NoDisbursement"] = NoDisbursement;

            }
            // If Disbursement contain no status "preparing" from last thursday to today
            else
            {
                NoDisbursement = true;
                ViewData["NoDisbursement"] = NoDisbursement;
                ViewBag.StartDate = "";
                ViewBag.EndDate = "";
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

            ViewData["NoDisbursement"] = true;
            ViewBag.StartDate = StartDate;
            ViewBag.EndDate = EndDate;
            DateTime SDate = DateTime.ParseExact(StartDate, "dd-MM-yyyy", 
                            System.Globalization.CultureInfo.InvariantCulture);
            DateTime EDate = DateTime.ParseExact(EndDate, "dd-MM-yyyy",
                            System.Globalization.CultureInfo.InvariantCulture);
            // Search and retrieve Requisition & Requisition Item
            List<Retrieval> RetrievalItem = _requisitionDAO
                                            .RetrieveRequisition(IdStoreClerk, SDate, EDate);
            List<Retrieval> RetrievalForm = _requisitionItemDAO
                                            .RetrieveRequisitionItem(RetrievalItem);
            ViewData["RetrievalForm"] = RetrievalForm;
            ViewData["RetrievalItem"] = RetrievalItem;
            // Create Disbursement and set status to "preparing"
            _disbursementDAO.CreateDisbursement(RetrievalItem);
            // Create DisbursementItem and set status to "preparing"
            _disbursementItemDAO.CreateDisbursementItem();
            
            return View();
        }

        
        [HttpPost]
        public ActionResult SaveDisbursement(int[] IdItemRetrieved, int[] StockUnit, int[] RequestUnit) 
        {
            bool ItemRetrieved = false;
            if (IdItemRetrieved.Any())
            {
                //update disbursement

                //update stockrecord
                

                //raise alert
                ItemRetrieved = false;
            }
            if (ItemRetrieved)
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
            ViewData["IsStockLow"] = _itemDAO.IsStockLow();
            return View();
        }
        public ActionResult MakePurchaseOrder()
        {
            return View();
        }
    }
}