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
    public class DepartmentHeadController : Controller
    {
        private readonly EmployeeDAO _employeeDAO;
        private readonly RequisitionDAO _requisitionDAO;
        private readonly RequisitionItemDAO _requisitionItemDAO;
        private readonly ItemDAO _itemDAO;
        private readonly DepartmentDAO _departmentDAO;
        private readonly CollectionPointDAO _collectionPointDAO;
        private readonly NotificationChannelDAO _notificationChannelDAO;

        //EmployeeDAO _employeeDAO;
        //RequisitionDAO _requisitionDAO;
        //RequisitionItemDAO _requisitionItemDAO;
        //ItemDAO _itemDAO;
        //DepartmentDAO _deparmentDAO;
        //CollectionPointDAO _collectionPointDAO;
        public DepartmentHeadController()
        {
            _employeeDAO = new EmployeeDAO();
            _requisitionDAO= new RequisitionDAO();
            _requisitionItemDAO = new RequisitionItemDAO();
            _itemDAO = new ItemDAO();

            _departmentDAO = new DepartmentDAO();
            _notificationChannelDAO = new NotificationChannelDAO();
            
            _collectionPointDAO = new CollectionPointDAO();
        }

        public ActionResult Notification()
        {
            int IdReceiver = (int)Session["IdEmployee"];

            ViewData["NCs"] = _notificationChannelDAO.FindAllNotificationsByIdReceiver(IdReceiver);

            return View();
        }
        // show lists of requisitions
        public ActionResult PendingLists()
        {
            string codeDepartment = _departmentDAO.FindCodeDepartmentByIdEmployee((int)Session["IdEmployee"]);
            //String codeDepartment = "ENGL"; // temporary use this employee id 36 with engL
            
            List<Requisition> empReqList=_employeeDAO.RaisesRequisitions(codeDepartment);
            ViewBag.empReqList = empReqList;
            return View();
        }

        // show the pending detail
        public ActionResult PendingDetail(int idRequisition)
        {
            // need to find Requisition by this idReq by employee
            Requisition requisition =_requisitionDAO.FindRequisitionByRequisionId(idRequisition);

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
                + " your requisition: "+req.IdRequisition + " raised on " + req.RaiseDate + " has been approved.";

            _notificationChannelDAO.CreateNotificationsToIndividual(IdEmployee, (int)Session["IdEmployee"], message);
            emailClass.SendTo(_employeeDAO.FindEmployeeById(IdEmployee).Email, "SSIS System Email", message);
            //end of notification sending 

            return RedirectToAction("PendingLists", "DepartmentHead");
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

            return RedirectToAction("PendingLists", "DepartmentHead");
        }

        // view CurrentRep and CP
        public ActionResult CurrentRepCP()
        {
            String codeDepartment = "CPSC";        
            Employee employee=_employeeDAO.FindDepartmentRep(codeDepartment);
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
            List<Employee> empList=_employeeDAO.FindEmployeeListByDepartment(codeDepartment);
            List<CollectionPoint> cpList = _collectionPointDAO.FindAll();
            ViewBag.Employee = new SelectList(empList, "IdEmployee", "Name"); // put inside drop down list
            ViewBag.EmployeeList = empList;
            ViewBag.CollectionPoint = cpList;
            return View();
        }
        [HttpPost]
        public ActionResult GetChangeRepCP(string emp, string cp,string judge)
        {
            string submit = judge;
            string location = cp;
            string empName = emp;
            if (judge.Equals("Cancel"))
            {
                RedirectToAction("CurrentRepCP", "DepartmentHead");
            }
            else if(judge.Equals("Apply Change"))
            {
                if(location!=null && empName != null)
                {
                    Employee newRep = _employeeDAO.FindEmployeeByName(empName);
                    string codeDepartment = newRep.CodeDepartment;
                    string oldCollectionPoint = _collectionPointDAO.FindByDepartment(codeDepartment);
                    Employee oldRep = _employeeDAO.FindDepartmentRep(codeDepartment);

                    //change old rep back to employee
                    _employeeDAO.PutOldRepBack(oldRep.Name);
                    //change the current employee to rep and change collection point
                    _employeeDAO.ChangeNewRepCP(newRep.Name, location);

                    if (newRep.Name != oldRep.Name)
                    {
                        //@Shutong: send notification here
                        int IdEmployee = oldRep.IdEmployee;
                        var hub = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                        hub.Clients.All.receiveNotification(IdEmployee);
                        EmailClass emailClass = new EmailClass();
                        string message = "Hi " + oldRep.Name
                            + ", you are not Department Rep anymore.";
                        _notificationChannelDAO.CreateNotificationsToIndividual(IdEmployee, (int)Session["IdEmployee"], message);
                        emailClass.SendTo(oldRep.Email, "SSIS System Email", message);

                        IdEmployee = newRep.IdEmployee;
                        hub.Clients.All.receiveNotification(IdEmployee);
                        message = "Hi " + newRep.Name
                           + ", you are appointed as Department Rep.";
                        _notificationChannelDAO.CreateNotificationsToIndividual(IdEmployee, (int)Session["IdEmployee"], message);
                        emailClass.SendTo(newRep.Email, "SSIS System Email", message);
                        //end of notification sending
                    }
                    //if rep didnot change but only cp changes
                    else
                    {
                        if (oldCollectionPoint != location)
                        {
                            int IdEmployee = oldRep.IdEmployee;
                            var hub = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                            hub.Clients.All.receiveNotification(IdEmployee);
                            EmailClass emailClass = new EmailClass();
                            string message = "Hi " + oldRep.Name
                                + ", your collection point has been changed by your head.";
                            _notificationChannelDAO.CreateNotificationsToIndividual(IdEmployee, (int)Session["IdEmployee"], message);
                            emailClass.SendTo(oldRep.Email, "SSIS System Email", message);

                        }

                    }

                    return RedirectToAction("CurrentRepCP", "DepartmentHead");
                }
            }

            return RedirectToAction("CurrentRepCP", "DepartmentHead");
        }
        public ActionResult Delegation()
        {
            return View();
        }
        


        //use home logout method!!
        public ActionResult Logout()
        {
            // need to redirect to Login page
            return View();
        }
    }
}