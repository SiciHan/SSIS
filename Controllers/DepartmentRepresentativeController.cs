using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Team8ADProjectSSIS.DAO;
using Team8ADProjectSSIS.Filters;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.Controllers
{
    [AuthorizeFilter]
    [AuthenticateFilter]
    public class DepartmentRepresentativeController : Controller
    {
        private readonly DisbursementDAO _disbursementDAO;
        private readonly EmployeeDAO _employeeDAO;
        private readonly DisbursementItemDAO _disbursementItemDAO;
        private readonly CollectionPointDAO _collectionPointDAO;
        private readonly DepartmentDAO _departmentDAO;
        private readonly NotificationChannelDAO _notificationChannelDAO;
        public DepartmentRepresentativeController()
        {
            _disbursementDAO = new DisbursementDAO();
            _employeeDAO = new EmployeeDAO();
            _disbursementItemDAO = new DisbursementItemDAO();
            _collectionPointDAO = new CollectionPointDAO();
            _departmentDAO = new DepartmentDAO();
            _notificationChannelDAO = new NotificationChannelDAO();
        }

        // GET: DepartmentRepresentative
        [HttpGet]
        public ActionResult Home()
        {
            //Testing Hardcode
            //int idEmployee = 4;
            int idEmployee = (int)Session["IdEmployee"];
            Employee employee = _employeeDAO.FindEmployeeById(idEmployee);
            Disbursement disbursement = _disbursementDAO.GetScheduledDisbursement(employee.CodeDepartment);
            List<CollectionPoint> collectionPoints = _collectionPointDAO.FindAll();
            ViewBag.disbursement = disbursement;
            if (disbursement != null)
            {
                ViewBag.status = disbursement.Status;
                ViewBag.department = disbursement.Department;
                ViewBag.collectionPt = disbursement.CollectionPoint;
                ViewBag.storeClerk = disbursement.DisbursedBy;
                ViewBag.disbursementItems = disbursement.DisbursementItems;
            }
            else
            {
                ViewBag.disbursement = new Disbursement();
                ViewBag.status = new Status();
                ViewBag.department = new Department();
                ViewBag.collectionPt = new CollectionPoint();
                ViewBag.storeClerk = new Employee();
                ViewBag.disbursementItems = new DisbursementItem();
            }
            ViewBag.allCollectionPoints = collectionPoints;
            ViewData["Rep"] = employee;
            return View();
        }

        [HttpGet]
        public ActionResult UpdateCollectionPoint(int idDisbursement, int idCollectionPt)
        {
            //int idEmployee = 4;
            int idEmployee = (int)Session["IdEmployee"];
            Employee employee = _employeeDAO.FindEmployeeById(idEmployee);
            
            bool result1 =_departmentDAO.UpdateCollectionPt(employee.CodeDepartment, idCollectionPt);
            bool result2 = _disbursementDAO.UpdateCollectionPt(idDisbursement, idCollectionPt);
            return RedirectToAction("Home");
        }

        [HttpGet]
        public ActionResult AcknowledgeCollection(int idDisbursement)
        {
            //int idEmployee = 4;
            int idEmployee = (int)Session["IdEmployee"];
            Employee employee = _employeeDAO.FindEmployeeById(idEmployee);
            bool result = _disbursementDAO.AcknowledgeCollection(idDisbursement, employee.IdEmployee);
            return RedirectToAction("Home");
        }

        public ActionResult History(string searchContext = "")
        {
            
            //Testing Hardcode
            //int idEmployee = 4;
            int idEmployee = (int)Session["IdEmployee"];
            Employee employee = _employeeDAO.FindEmployeeById(idEmployee);
            List<Disbursement> disbursements = _disbursementDAO.GetReceivedDisbursements(employee.CodeDepartment, searchContext);
            ViewBag.disbursements = disbursements;
            ViewBag.searchContext = searchContext;            
            return View();
        }

        public ActionResult Details(int idDisbursement)
        {
            Disbursement disbursement = _disbursementDAO.GetDisbursement(idDisbursement);
            ViewBag.disbursement = disbursement;
            if (disbursement != null)
            {
                ViewBag.status = disbursement.Status;
                ViewBag.department = disbursement.Department;
                ViewBag.collectionPt = disbursement.CollectionPoint;
                ViewBag.collectedBy = disbursement.CollectedBy;
                ViewBag.disbursedBy = disbursement.DisbursedBy;
                ViewBag.disbursementItems = disbursement.DisbursementItems;
            }
            else
            {
                ViewBag.disbursement = new Disbursement();
            }
            return View();
        }

        public ActionResult LocationMap(int idCollectionPt)
        {
            CollectionPoint collectionPoint = _collectionPointDAO.Find(idCollectionPt);
            if (collectionPoint == null) collectionPoint = new CollectionPoint();
            ViewBag.collectionPoint = collectionPoint;
            return View();
        }
        public ActionResult CollectionPoints()
        {
            List<CollectionPoint> collectionPoints = _collectionPointDAO.FindAll();
            ViewBag.collectionPoints = collectionPoints;
            return View();
        }

        public ActionResult Notification()
        {
            int IdReceiver = (int)Session["IdEmployee"];
            
            ViewData["NCs"] = _notificationChannelDAO.FindAllNotificationsByIdReceiver(IdReceiver);

            return View();
        }
    }
}