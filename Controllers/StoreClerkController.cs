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
        

        public StoreClerkController()
        {
            this._disbursementDAO = new DisbursementDAO();
            this._requisitionDAO = new RequisitionDAO();
            this._requisitionItemDAO = new RequisitionItemDAO();
            this._stockRecordDAO = new StockRecordDAO();
            this._disbursementItemDAO = new DisbursementItemDAO();
        }
        // GET: StoreClerk
        public ActionResult Index()
        {
            return View();
        }
        // Get Method
        public ActionResult FormRetrieve()
        {
            // Assume ClerkID
            int IdStoreClerk = 1;


            bool NoDisbursement = false;
            DateTime Today = DateTime.Now;
            DateTime LastThu = DateTime.Now.AddDays(-1);
            while (LastThu.DayOfWeek != DayOfWeek.Thursday)
                LastThu = LastThu.AddDays(-1);
            // If Disbursement contain status "preparing" from last thurseday to today
            if (_disbursementDAO.CheckExistDisbursement(IdStoreClerk, Today, LastThu))
            {
                // Search Preparing status in D
                // Search Preparing status in DI
                // Pass to View
                List<int> SelectedRequisition = _requisitionDAO
                                                .SearchRequisitionForRetrival(Today, LastThu);
                List<Retrieval> SelectedItem = _requisitionItemDAO
                                                    .RetrieveRequisitionItemQuantity(SelectedRequisition);
                ViewData["SelectedItem"] = SelectedItem;
                NoDisbursement = false;
                ViewData["NoDisbursement"] = NoDisbursement;
                /*ViewBag.Today = today.ToString("dd MMMM yyyy");
                ViewBag.LastFriday = lastfriday.ToString("dd MMMM yyyy");*/

            }
            // If Disbursement contain no status "preparing" from last thursday to today
            else
            {
                NoDisbursement = true;
                ViewData["NoDisbursement"] = NoDisbursement;
                ViewBag.StartDate = "";
                ViewBag.EndDate = "";
                ViewBag.Today = Today.ToString("dd'/'MM'/'yyyy");
                ViewBag.LastThu = LastThu.ToString("dd'/'MM'/'yyyy");
            }


            return View();
        }
        // Post Method
        [HttpPost]
        public ActionResult FormRetrieve(string StartDate, string EndDate)
        {
            // Assume ClerkID
            int IdStoreClerk = 1;

            ViewData["NoDisbursement"] = true;
            ViewBag.StartDate = StartDate;
            ViewBag.EndDate = EndDate;
            // Search Requisition
            _requisitionDAO.RetrieveRequisition(IdStoreClerk, StartDate, EndDate);
            // Search Requisition Item
            // Create D
            _disbursementDAO.CreateDisbursement();
            // Create DI
            _disbursementItemDAO.CreateDisbursementItem();
            
            // Pass to View
            return View();
        }

        
        [HttpPost]
        public ActionResult StockRetrieved(int[] IdItemRetrieved, int[] StockUnit, int[] RequestUnit) 
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
    }
}