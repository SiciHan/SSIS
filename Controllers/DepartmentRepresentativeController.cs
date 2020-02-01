using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Team8ADProjectSSIS.DAO;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.Controllers
{
    public class DepartmentRepresentativeController : Controller
    {
        private readonly DisbursementDAO _disbursementDAO;
        private readonly EmployeeDAO _employeeDAO;
        private readonly DisbursementItemDAO _disbursementItemDAO;
        private readonly CollectionPointDAO _collectionPointDAO;
        private readonly DepartmentDAO _departmentDAO;
        public DepartmentRepresentativeController()
        {
            _disbursementDAO = new DisbursementDAO();
            _employeeDAO = new EmployeeDAO();
            _disbursementItemDAO = new DisbursementItemDAO();
            _collectionPointDAO = new CollectionPointDAO();
            _departmentDAO = new DepartmentDAO();
        }

        // GET: DepartmentRepresentative
        [HttpGet]
        public ActionResult Home()
        {
            //Testing Hardcode
            int idEmployee = 4;
            //int idEmployee = (int)Session["IdEmployee"];
            Employee employee = _employeeDAO.FindEmployeeById(idEmployee);
            Disbursement disbursement = _disbursementDAO.GetScheduledDisbursement(employee.CodeDepartment);
            List<CollectionPoint> collectionPoints = _collectionPointDAO.FindAll();
            ViewBag.disbursement = disbursement;
            if (disbursement != null)
            {
                ViewBag.status = disbursement.Status;
                ViewBag.department = disbursement.Department;
                ViewBag.collectionPt = disbursement.Department.CollectionPt;
                ViewBag.storeClerk = disbursement.Department.CollectionPt.CPClerks.FirstOrDefault().StoreClerk;
                ViewBag.disbursementItems = disbursement.DisbursementItems;
            }
            else
            {
                ViewBag.disbursement = new Disbursement();
            }
            ViewBag.allCollectionPoints = collectionPoints;
            return View();
        }

        [HttpGet]
        public ActionResult UpdateCollectionPoint(int idCollectionPt)
        {
            int idEmployee = 4;
            //int idEmployee = (int)Session["IdEmployee"];
            Employee employee = _employeeDAO.FindEmployeeById(idEmployee);
            bool result = _departmentDAO.UpdateCollectionPt(employee.CodeDepartment, idCollectionPt);
            return RedirectToAction("Home");
        }

        [HttpGet]
        public ActionResult AcknowledgeCollection(int idDisbursement)
        {
            bool result = _disbursementDAO.AcknowledgeCollection(idDisbursement);
            return RedirectToAction("Home");
        }

        public ActionResult History()
        {
            //Testing Hardcode
            int idEmployee = 4;
            //int idEmployee = (int)Session["IdEmployee"];
            Employee employee = _employeeDAO.FindEmployeeById(idEmployee);
            List<Disbursement> disbursements = _disbursementDAO.GetReceivedDisbursements(employee.CodeDepartment);
            ViewBag.disbursements = disbursements;
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
                ViewBag.collectionPt = disbursement.Department.CollectionPt;
                ViewBag.storeClerk = disbursement.Department.CollectionPt.CPClerks.FirstOrDefault().StoreClerk;
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
            return View("CollectionPoints");
        }
    }
}