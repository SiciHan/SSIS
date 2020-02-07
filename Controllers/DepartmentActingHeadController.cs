using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using Team8ADProjectSSIS.DAO;
using Team8ADProjectSSIS.EmailModel;
using Team8ADProjectSSIS.Filters;
using Team8ADProjectSSIS.Hubs;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.Controllers
{
    [AuthorizeFilter]
    [AuthenticateFilter]
    [DelegationActionFilter]
    public class DepartmentActingHeadController : Controller
    {

        private readonly EmployeeDAO _employeeDAO;
        private readonly RequisitionDAO _requisitionDAO;
        private readonly RequisitionItemDAO _requisitionItemDAO;
        private readonly ItemDAO _itemDAO;
        private readonly DepartmentDAO _departmentDAO;
        private readonly NotificationChannelDAO _notificationChannelDAO;
        private readonly CollectionPointDAO _collectionPointDAO;
        public DepartmentActingHeadController()
        {
            _employeeDAO = new EmployeeDAO();
            _requisitionDAO = new RequisitionDAO();
            _requisitionItemDAO = new RequisitionItemDAO();
            _itemDAO = new ItemDAO();
            _departmentDAO = new DepartmentDAO();
            _notificationChannelDAO = new NotificationChannelDAO();
            _collectionPointDAO = new CollectionPointDAO();
        }

        // GET: DepartmentActingHead
        public ActionResult Index()
        {
            string codeDepartment = _departmentDAO.FindCodeDepartmentByIdEmployee((int)Session["IdEmployee"]);
            return View();
        }

        public ActionResult Notification()
        {
            int IdReceiver = (int)Session["IdEmployee"];

            ViewData["NCs"] = _notificationChannelDAO.FindAllNotificationsByIdReceiver(IdReceiver);

            return View();
        }

        public ActionResult PendingLists()
        {
            string codeDepartment = _departmentDAO.FindCodeDepartmentByIdEmployee((int)Session["IdEmployee"]);
            //String codeDepartment = "ENGL"; // temporary use this employee id 36 with engL

            List<Requisition> empReqList = _employeeDAO.RaisesRequisitions(codeDepartment);
            ViewBag.empReqList = empReqList;
            return View();
        }

        // show the pending detail
        public ActionResult PendingDetail(int idRequisition)
        {
            // need to find Requisition by this idReq by employee
            Requisition requisition = _requisitionDAO.FindRequisitionByRequisionId(idRequisition);

            // find the RequisitionItems to get unit requests by this idReq

            List<RequisitionItem> requisitionItemList = _requisitionItemDAO.FindRequisitionItem(idRequisition);


            ViewData["requisition"] = requisition;
            ViewData["requisitionItemList"] = requisitionItemList;


            return View();
        }

        public ActionResult Approve(int idRequisition)
        {
            _requisitionDAO.UpdateApproveStatus(idRequisition);

            //@Shutong: send notification here
            Requisition req = _requisitionDAO.FindRequisitionByRequisionId(idRequisition);
            int IdEmployee = req.IdEmployee;
            var hub = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            hub.Clients.All.receiveNotification(IdEmployee);
            EmailClass emailClass = new EmailClass();
            string message = "Hi," + _employeeDAO.FindEmployeeById(IdEmployee).Name
                + " your requisition: " + req.IdRequisition + " raised on " + req.RaiseDate + " has been approved.";

            _notificationChannelDAO.CreateNotificationsToIndividual(IdEmployee, (int)Session["IdEmployee"], message);
            emailClass.SendTo(_employeeDAO.FindEmployeeById(IdEmployee).Email, "SSIS System Email", message);
            //end of notification sending 

            return RedirectToAction("PendingLists", "DepartmentActingHead");
        }
        public ActionResult Reject(int idRequisition)
        {
            _requisitionDAO.UpdateRejectStatus(idRequisition);

            //@Shutong: send notification here
            Requisition req = _requisitionDAO.FindRequisitionByRequisionId(idRequisition);
            int IdEmployee = req.IdEmployee;
            var hub = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            hub.Clients.All.receiveNotification(IdEmployee);
            EmailClass emailClass = new EmailClass();
            string message = "Hi," + _employeeDAO.FindEmployeeById(IdEmployee).Name
                + " your requisition: " + req.IdRequisition + " raised on " + req.RaiseDate + " has been rejected.";

            _notificationChannelDAO.CreateNotificationsToIndividual(IdEmployee, (int)Session["IdEmployee"], message);
            emailClass.SendTo(_employeeDAO.FindEmployeeById(IdEmployee).Email, "SSIS System Email", message);
            //end of notification sending 

            return RedirectToAction("PendingLists", "DepartmentActingHead");
        }

        // view CurrentRep and CP
        // view CurrentRep and CP
        public ActionResult CurrentRepCP()
        {
            String codeDepartment = "CPSC";
            Employee employee = _employeeDAO.FindDepartmentRep(codeDepartment);
            Department department = _departmentDAO.FindDepartmentCollectionPoint(codeDepartment);
            ViewData["employee"] = employee;
            ViewData["department"] = department;
            return View();
        }

        // change Rep and CP
        public ActionResult ChangeRepCP()
        {
            // find employee from the department
            String codeDepartment = "CPSC";
            List<Employee> empList = _employeeDAO.FindEmployeeListByDepartment(codeDepartment);
            List<CollectionPoint> cpList = _collectionPointDAO.FindAll();
            ViewBag.Employee = new SelectList(empList, "IdEmployee", "Name"); // put inside drop down list
            ViewBag.EmployeeList = empList;
            ViewBag.CollectionPoint = cpList;
            return View();
        }
        [HttpPost]
        public ActionResult GetChangeRepCP(string emp, string cp, string judge)
        {
            string submit = judge;
            string location = cp;
            string empName = emp;
            if (judge.Equals("Cancel"))
            {
                RedirectToAction("CurrentRepCP", "DepartmentActingHead");
            }
            else if (judge.Equals("Apply Change"))
            {
                if (location != null && empName != null)
                {
                    Employee newRep = _employeeDAO.FindEmployeeByName(empName);
                    string codeDepartment = newRep.CodeDepartment;
                    Employee oldRep = _employeeDAO.FindDepartmentRep(codeDepartment);

                    //change old rep status back to 1
                    _employeeDAO.PutOldRepBack(oldRep.Name);

                    _employeeDAO.ChangeNewRepCP(newRep.Name, location);
                    return RedirectToAction("CurrentRepCP", "DepartmentActingHead");
                }
            }



            return RedirectToAction("CurrentRepCP", "DepartmentActingHead");
        }

    }
}