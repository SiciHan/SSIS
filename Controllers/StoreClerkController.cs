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

        // James
        public ActionResult Disbursement()
        {
            // retrieve 2 lists of Disbursement Lists which are "Prepared" and "Scheduled" under the same Coll Point
            // find by Status and Collection Point
            List<Disbursement> prepList= _disbursementDAO.FindByStatus("Prepared");
            List<Disbursement> scheList= _disbursementDAO.FindByStatus("Scheduled");

            ViewBag.prepList = prepList;
            ViewBag.scheList = scheList;

            return View();
        }

        // James
        [HttpPost]
        public ActionResult Schedule(IEnumerable<int> disbIdsToSchedule)
        {
            _disbursementDAO.UpdateStatus(disbIdsToSchedule);
            return RedirectToAction("Disbursement");
        }

        // James
        public ActionResult Redistribute(int disbId)
        {
            //var disbId = Url.RequestContext.RouteData.Values["id"];
            Disbursement targetDisbursement = _disbursementDAO.FindById(disbId);
            ViewBag.disb = targetDisbursement;

            return PartialView("Redistribute", targetDisbursement);
        }
    }
}