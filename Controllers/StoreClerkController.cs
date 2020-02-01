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
        private readonly CollectionPointDAO _collectionPointDAO;

        public StoreClerkController()
        {
            this._disbursementDAO = new DisbursementDAO();
            this._requisitionDAO = new RequisitionDAO();
            this._requisitionItemDAO = new RequisitionItemDAO();
            this._stockRecordDAO = new StockRecordDAO();
            this._disbursementItemDAO = new DisbursementItemDAO();
            this._purchaseOrderDAO = new PurchaseOrderDAO();
            this._itemDAO = new ItemDAO();
            this._collectionPointDAO = new CollectionPointDAO();

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
        
        //@Shutong
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
        //@Shutong
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

        // James: Disbursement overview
        public ActionResult Disbursement()
        {
            // retrieve 2 lists of Disbursement Lists which are "Prepared" and "Scheduled" under the same Coll Point
            // find by Status and Clerk's Collection Points
            // assume clerkId is 2
            int clerkId = 2;
            List<Disbursement> prepList = _disbursementDAO.FindByStatus("Prepared", clerkId);
            List<Disbursement> scheList = _disbursementDAO.FindByStatus("Scheduled", clerkId);
            scheList.AddRange(_disbursementDAO.FindByStatus("Received", clerkId));

            ViewBag.prepList = prepList;
            ViewBag.scheList = scheList;

            // Finds Next Monday
            DateTime NextMon = DateTime.Now;
            while (NextMon.DayOfWeek != DayOfWeek.Monday)
                NextMon = NextMon.AddDays(1);

            ViewBag.NextMon = NextMon.ToString("yyyy-MM-dd");

            return View();
        }

        // James: Selecting multiple disbursements to schedule for collection
        [HttpPost]
        public ActionResult Schedule(IEnumerable<int> disbIdsToSchedule, String pickDate)
        {
            if(disbIdsToSchedule != null)
            {
                // schedule for selected date by setting the date from the form
                DateTime SDate = DateTime.ParseExact(pickDate, "yyyy-MM-dd",
                                System.Globalization.CultureInfo.InvariantCulture);

                // add in notification here upon updating status

                _disbursementDAO.UpdateStatus(disbIdsToSchedule, 10, SDate);
            } 

            return RedirectToAction("Disbursement");
        }

        // James: Scheduling a single disbursement with redistribution if necessary
        [HttpPost]
        public ActionResult ScheduleSingle(IEnumerable<int> disbId, IList<int> disbItemId, IList<int> transferQtyNum, IList<int> disbItemIdDeptFrom, String pickDate)
        {
            _disbursementItemDAO.GiveAndTake(disbItemId, transferQtyNum, disbItemIdDeptFrom);

            // schedule for selected date by setting the date from the form
            DateTime SDate = DateTime.ParseExact(pickDate, "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture);

            // add in notification here upon updating status and notify on shortfall (if any)

            _disbursementDAO.UpdateStatus(disbId, 10, SDate);
            return RedirectToAction("Disbursement");
        }

        // James: Opens page to redistribute qty from other disbursements
        public ActionResult Redistribute(int disbId)
        {
            // assume clerkId is 2
            int IdStoreClerk = 2;
            Disbursement targetDisbursement = _disbursementDAO.FindById(disbId);
            ViewBag.disb = targetDisbursement;

            ViewBag.dropdownDisbursementItems = _disbursementItemDAO.FindCorrespondingDisbursementItems(targetDisbursement.DisbursementItems, IdStoreClerk);

            // Finds Next Monday
            DateTime NextMon = DateTime.Now;
            while (NextMon.DayOfWeek != DayOfWeek.Monday)
                NextMon = NextMon.AddDays(1);

            ViewBag.NextMon = NextMon.ToString("yyyy-MM-dd");

            return PartialView("Redistribute", targetDisbursement);
        }

        //James: Opens page to handle disbursement with Dep Rep
        public ActionResult DisbursementDetails(int disbId)
        {
            Disbursement targetDisbursement = _disbursementDAO.FindById(disbId);
            ViewBag.disb = targetDisbursement;

            return PartialView("DisbursementDetails");
        }

        //James: Refresh Disbursement page and updates the unitIssued to the QtyDisbursed
        [HttpPost]
        public ActionResult RefreshDisbursement(IEnumerable<int> disbId, IList<int> disbItemId, IList<int> qtyDisbursed)
        {
            Disbursement targetDisbursement = _disbursementDAO.FindById(disbId.First());
            ViewBag.disb = targetDisbursement;

            // if qtyDisbursed < disbItem.UnitIssued then raise a SA-broken

            // updates the disbitemId's unitissued to the qtyDisbursed
            _disbursementItemDAO.UpdateUnitIssued(disbItemId, qtyDisbursed);

            // add notification below to DR

            return PartialView("DisbursementDetails");
        }

        //James: For clerk to sign, raises SA in case of discrepancy and email out a copy of the disbursement details
        [HttpPost]
        public ActionResult ClerkSign(IEnumerable<int> disbId, IList<int> disbItemId, IList<int> qtyDisbursed)
        {
            // assume clerkId is 2
            int clerkId = 2;
            Disbursement targetDisbursement = _disbursementDAO.FindById(disbId.First());
            List<DisbursementItem> targetDisbItems = targetDisbursement.DisbursementItems.ToList();
            // updates the disb's status to "Disbursed" or 7
            _disbursementDAO.UpdateStatus(disbId, 7, DateTime.Now);

            // creates stockrecords for each disbItemId passing in the itemId, departmentId and qtyDisbursed
            //for (int i = 0; i < disbItemId.Count; i++)
            //    _stockRecordDAO.CreateDisbursementTransaction(targetDisbItems[i].IdItem, targetDisbursement.CodeDepartment, qtyDisbursed[i], clerkId);

            // emails a copy / sends notification to DR and Clerk

            return RedirectToAction("Disbursement");
        }
    }
}