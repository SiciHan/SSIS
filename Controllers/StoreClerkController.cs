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

        public StoreClerkController()
        {
            _disbursementDAO = new DisbursementDAO();
            _disbursementItemDAO = new DisbursementItemDAO();
        }

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
            List<Disbursement> prepList= _disbursementDAO.FindByStatus("Prepared");
            List<Disbursement> scheList= _disbursementDAO.FindByStatus("Scheduled");

            ViewBag.prepList = prepList;
            ViewBag.scheList = scheList;

            return View();
        }

        // James: Selecting multiple disbursements to schedule for collection
        [HttpPost]
        public ActionResult Schedule(IEnumerable<int> disbIdsToSchedule)
        {
            _disbursementDAO.UpdateStatus(disbIdsToSchedule);
            // add in notification here upon updating status

            return RedirectToAction("Disbursement");
        }

        // James: Scheduling a single disbursement with redistribution if necessary
        [HttpPost]
        public ActionResult ScheduleSingle(IEnumerable<int> disbId, IList<int> disbItemId, IList<int> transferQtyNum, IList<int> disbItemIdDeptFrom)
        {
            //Debugging and PoC
            /*System.Diagnostics.Debug.WriteLine(disbId);
            disbId.ToList().ForEach(x => System.Diagnostics.Debug.WriteLine(x));
            System.Diagnostics.Debug.WriteLine($"disbItemId Count: {disbItemId.Count}, transferQtyNum Count: {transferQtyNum.Count}");
            foreach (int i in disbItemId)
                System.Diagnostics.Debug.WriteLine("disbItemId: " + i);

            foreach (int i in transferQtyNum)
                System.Diagnostics.Debug.WriteLine("transferQtyNum: " + i);

            for (int i = 0; i < disbItemId.Count; i++)
            {
                System.Diagnostics.Debug.WriteLine("disbItemId: " + disbItemId[i]);
            }
            foreach (int i in disbItemIdDeptFrom)
                System.Diagnostics.Debug.WriteLine("disbItemIdDeptFrom: " + i);*/

            _disbursementItemDAO.GiveAndTake(disbItemId, transferQtyNum, disbItemIdDeptFrom);
            _disbursementDAO.UpdateStatus(disbId);
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
    }
}