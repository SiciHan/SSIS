using System;
using System.Collections.Generic;
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
        private readonly DisbursementItemDAO _disbursementItemDAO;
        private readonly CollectionPointDAO _collectionPointDAO;

        public StoreClerkController()
        {
            _disbursementDAO = new DisbursementDAO();
            _disbursementItemDAO = new DisbursementItemDAO();
            _collectionPointDAO = new CollectionPointDAO();
        }

        int clerkId = 2;

        // GET: StoreClerk
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PurchaseOrderList()
        {

            return View();
        }

        // James: Disbursement overview
        public ActionResult Disbursement()
        {
            // retrieve 2 lists of Disbursement Lists which are "Prepared" and "Scheduled" under the same Coll Point
            // find by Status and Clerk's Collection Points
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