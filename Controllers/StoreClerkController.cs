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

        // James: Disbursement overview
        public ActionResult Disbursement()
        {
            // retrieve 2 lists of Disbursement Lists which are "Prepared" and "Scheduled" under the same Coll Point
            // find by Status and Clerk's Collection Points
            // assume clerkId is 2
            //int clerkId = 2;
            //List<Disbursement> prepList= _disbursementDAO.FindByStatus("Prepared", clerkId);
            //List<Disbursement> scheList= _disbursementDAO.FindByStatus("Scheduled", clerkId);
            //scheList.AddRange(_disbursementDAO.FindByStatus("Received", clerkId));
            //scheList.AddRange(_disbursementDAO.FindByStatus("Disbursed", clerkId));
            List<Disbursement> prepList= _disbursementDAO.FindByStatus("Prepared");
            List<Disbursement> scheList= _disbursementDAO.FindByStatus("Scheduled");
            scheList.AddRange(_disbursementDAO.FindByStatus("Received"));

            ViewBag.prepList = prepList;
            ViewBag.scheList = scheList;

            return View();
        }

        // James: Selecting multiple disbursements to schedule for collection
        [HttpPost]
        public ActionResult Schedule(IEnumerable<int> disbIdsToSchedule)
        {
            _disbursementDAO.UpdateStatus(disbIdsToSchedule, 10);
            // add in notification here upon updating status

            return RedirectToAction("Disbursement");
        }

        // James: Scheduling a single disbursement with redistribution if necessary
        [HttpPost]
        public ActionResult ScheduleSingle(IEnumerable<int> disbId, IList<int> disbItemId, IList<int> transferQtyNum, IList<int> disbItemIdDeptFrom)
        {
            _disbursementItemDAO.GiveAndTake(disbItemId, transferQtyNum, disbItemIdDeptFrom);
            _disbursementDAO.UpdateStatus(disbId, 10);
            // add in notification here upon updating status and notify on shortfall (if any)

            return RedirectToAction("Disbursement");
        }

        // James: Opens page to redistribute qty from other disbursements
        public ActionResult Redistribute(int disbId)
        {
            Disbursement targetDisbursement = _disbursementDAO.FindById(disbId);
            ViewBag.disb = targetDisbursement;

            ViewBag.dropdownDisbursementItems = _disbursementItemDAO.FindCorrespondingDisbursementItems(targetDisbursement.DisbursementItems);

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

            // if qtyDisbursed < disbItem.UnitIssued then raise a SA-missing/broken

            // updates the disbitemId's unitissued to the qtyDisbursed
            _disbursementItemDAO.UpdateUnitIssued(disbItemId, qtyDisbursed);

            // add notification below to DR

            return PartialView("DisbursementDetails");
        }

        //James: For clerk to sign, post the transactions to stock record and email out a copy of the disbursement details
        [HttpPost]
        public ActionResult ClerkSign(IEnumerable<int> disbId, IList<int> disbItemId, IList<int> qtyDisbursed)
        {
            // assume clerkId is 2
            int clerkId = 2;
            Disbursement targetDisbursement = _disbursementDAO.FindById(disbId.First());
            List<DisbursementItem> targetDisbItems = targetDisbursement.DisbursementItems.ToList();
            // updates the disb's status to "Disbursed" or 7
            _disbursementDAO.UpdateStatus(disbId, 7);

            // creates stockrecords for each disbItemId passing in the itemId, departmentId and qtyDisbursed
            for (int i = 0; i < disbItemId.Count; i++)
                _stockRecordDAO.CreateDisbursementTransaction(targetDisbItems[i].IdItem, targetDisbursement.CodeDepartment, qtyDisbursed[i], clerkId);

            // emails a copy / sends notification to DR and Clerk

            return RedirectToAction("Disbursement");
        }
    }
}