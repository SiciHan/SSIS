﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using Team8ADProjectSSIS.DAO;
using Team8ADProjectSSIS.EmailModel;
using Team8ADProjectSSIS.Filters;
using Team8ADProjectSSIS.Hubs;
using Team8ADProjectSSIS.Models;
//@Yu Shaohang
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
        private readonly DelegationDAO _delegationDAO;

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
            _delegationDAO = new DelegationDAO();
        }
        //@Shutong
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
        public ActionResult ApproveReject(string judge,int idRequisition,string remarks)
        {
            int id = idRequisition;
            string r = remarks;
            string w = r;
            if (judge.Equals("Approve"))
            {
                _requisitionDAO.UpdateApproveStatusAndRemarks(idRequisition, remarks);

                //@Shutong: send notification here
                Requisition req = _requisitionDAO.FindRequisitionByRequisionId(idRequisition);
                int IdEmployee = req.IdEmployee;
                var hub = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                hub.Clients.All.receiveNotification(IdEmployee);
                EmailClass emailClass = new EmailClass();
                string message = "Hi," + _employeeDAO.FindEmployeeById(IdEmployee).Name
                    + " your requisition: " + req.IdRequisition + " raised on " + req.RaiseDate + " has been approved. Remarks: "+remarks;

                _notificationChannelDAO.CreateNotificationsToIndividual(IdEmployee, (int)Session["IdEmployee"], message);
                emailClass.SendTo(_employeeDAO.FindEmployeeById(IdEmployee).Email, "SSIS System Email", message);
                //end of notification sending 
                return RedirectToAction("PendingLists", "DepartmentHead");
            }
            else if (judge.Equals("Reject"))
            {
                _requisitionDAO.UpdateRejectStatusAndRemarks(idRequisition, remarks);

                //@Shutong: send notification here
                Requisition req = _requisitionDAO.FindRequisitionByRequisionId(idRequisition);
                int IdEmployee = req.IdEmployee;
                var hub = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                hub.Clients.All.receiveNotification(IdEmployee);
                EmailClass emailClass = new EmailClass();
                string message = "Hi," + _employeeDAO.FindEmployeeById(IdEmployee).Name
                    + " your requisition: " + req.IdRequisition + " raised on " + req.RaiseDate + " has been rejected. Remarks: "+remarks;

                _notificationChannelDAO.CreateNotificationsToIndividual(IdEmployee, (int)Session["IdEmployee"], message);
                emailClass.SendTo(_employeeDAO.FindEmployeeById(IdEmployee).Email, "SSIS System Email", message);
                //end of notification sending 
                return RedirectToAction("PendingLists", "DepartmentHead");
            }
            return RedirectToAction("PendingLists", "DepartmentHead");
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
            //String codeDepartment = "CPSC";
            string codeDepartment = _departmentDAO.FindCodeDepartmentByIdEmployee((int)Session["IdEmployee"]);
            Employee employee=_employeeDAO.FindDepartmentRep(codeDepartment);
            Department department = _departmentDAO.FindDepartmentCollectionPoint(codeDepartment);
            if (employee == null)
            {

            }
            else
            {
                ViewData["employee"] = employee;
                ViewData["department"] = department;
            }
            
            return View();
        }

        // change Rep and CP
        public ActionResult ChangeRepCP()
        {
            // find employee from the department
            //String codeDepartment = "CPSC";
            string codeDepartment = _departmentDAO.FindCodeDepartmentByIdEmployee((int)Session["IdEmployee"]);
            List<Employee> empList=_employeeDAO.FindEmployeeListByDepartment(codeDepartment);
            List<CollectionPoint> cpList = _collectionPointDAO.FindAll();
            //ViewBag.Employee = new SelectList(empList, "IdEmployee", "Name"); // put inside drop down list
            ViewBag.EmployeeList = empList;
            ViewBag.CollectionPoint = cpList;

            // find current rep
            int idHead = (int)Session["IdEmployee"];
            Employee oldRep = _employeeDAO.FindCurrentRepAndCPByHeadId(idHead);
            ViewBag.oldRep = oldRep;
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
                    if (oldRep == null)
                    {
                        //if there is no oldRep then do not need to set idrole etc
                        //but need set new rep
                        _employeeDAO.ChangeNewRepCP(newRep.Name, location);

                        //@Shutong: send notification here
                        Employee e = _employeeDAO.FindEmployeeByName(newRep.Name);
                        int IdEmployee = e.IdEmployee;
                        var hub = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                        hub.Clients.All.receiveNotification(IdEmployee);
                        EmailClass emailClass = new EmailClass();
                        string message = "Hi " + newRep.Name
                               + ", you are appointed as Department Rep. Collection Point is "+location;

                        _notificationChannelDAO.CreateNotificationsToIndividual(IdEmployee, (int)Session["IdEmployee"], message);
                        emailClass.SendTo(_employeeDAO.FindEmployeeById(IdEmployee).Email, "SSIS System Email", message);
                        //end of notification sending 
                    }
                    else
                    {
                        // old rep is not null
                        //change old rep back to employee                   
                        //change the current employee to rep and change collection point
                        _employeeDAO.PutOldRepBack(oldRep.Name);
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
                               + ", you are appointed as Department Rep. Collection Point is " + location;
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
                    }
                                         
                    return RedirectToAction("CurrentRepCP", "DepartmentHead");
                }
            }

            return RedirectToAction("CurrentRepCP", "DepartmentHead");
        }
        public ActionResult Delegation()
        {
            //string codeDepartment = "ENGL";
            string codeDepartment = _departmentDAO.FindCodeDepartmentByIdEmployee((int)Session["IdEmployee"]);
            List<Employee>empList=_employeeDAO.FindEmployeeListByDepartmentAndRole(codeDepartment);
            ViewBag.EmployeeList = empList;
            // retrieve employee where idrole=1 from the same dept
            return View();
        }
        [HttpPost]
        public ActionResult PostDelegation(string emp,string cp,string judge, string StartDate, string EndDate)
        {
            int idDepartmentHead = ((int)Session["IdEmployee"]);
            string submit = judge;
            string empName = emp;
            string remarks = cp; // for notification
            string s1 = StartDate;
            string s2 = EndDate;
            if (submit.Equals("Back"))
            {
                Debug.WriteLine("Back");
                return RedirectToAction("ViewDelegations", "DepartmentHead");
            }
            else if (submit.Equals("Approve Delegate"))
            {
                Debug.WriteLine("Approve");
                // First check if they are null
                if (empName != null && !string.IsNullOrEmpty(s1) && !string.IsNullOrEmpty(s2))
                {
                    DateTime SDate = DateTime.ParseExact(s1, "dd-MM-yyyy",
                               System.Globalization.CultureInfo.InvariantCulture);
                    DateTime EDate = DateTime.ParseExact(s2, "dd-MM-yyyy",
                                    System.Globalization.CultureInfo.InvariantCulture);
                    // Find the emp by empName--> change emp role to idRole=4, get employee Id, pass id and startdate, end date into delegation
                    if (EDate >= SDate)
                    {
                        // approve delegation
                        // add one day
                        EDate = EDate.AddDays(1);
                        Delegation d=_delegationDAO.CreateDelegation(empName, SDate, EDate);
                        //@Shutong: send notification to acting head
                        Employee e = _employeeDAO.FindEmployeeByName(empName);
                        int IdEmployee = e.IdEmployee;
                        var hub = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                        hub.Clients.All.receiveNotification(IdEmployee);
                        EmailClass emailClass = new EmailClass();
                        string message = "Hi," + empName
                            + " You are delegated to Acting Department Head from " + d.StartDate + " to " + d.EndDate
                            + " to assist approve stationery requisition. \r\nRemarks: " + remarks;

                        _notificationChannelDAO.CreateNotificationsToIndividual(IdEmployee, (int)Session["IdEmployee"], message);
                        emailClass.SendTo(_employeeDAO.FindEmployeeById(IdEmployee).Email, "SSIS System Email", message);
                        //end of notification sending 

                        //@Shutong: send notification head who delegating acting head
                        Employee headDept = _employeeDAO.FindEmployeeById(idDepartmentHead);
                        hub.Clients.All.receiveNotification(idDepartmentHead);
                        message = "Hi," + headDept.Name
                            + " You have delegated " + empName + " as Acting Department Head from " + d.StartDate.ToString() + " to " + d.EndDate.ToString()
                            + " to assist approve stationery requisition. \r\nRemarks: " + remarks;

                        _notificationChannelDAO.CreateNotificationsToIndividual(idDepartmentHead, (int)Session["IdEmployee"], message);
                        emailClass.SendTo(_employeeDAO.FindEmployeeById(idDepartmentHead).Email, "SSIS System Email", message);
                        //end of notification sending 
                    }
                    else
                    {
                        ModelState.AddModelError("", "The end date should be later than startdate.");

                        string codeDepartment = _departmentDAO.FindCodeDepartmentByIdEmployee((int)Session["IdEmployee"]);
                        List<Employee> empList = _employeeDAO.FindEmployeeListByDepartmentAndRole(codeDepartment);
                        ViewBag.EmployeeList = empList;
                        return View("Delegation");
                    }

                }
                else
                {
                    ModelState.AddModelError("", "The fields are invalid.");

                    string codeDepartment = _departmentDAO.FindCodeDepartmentByIdEmployee((int)Session["IdEmployee"]);
                    List<Employee> empList = _employeeDAO.FindEmployeeListByDepartmentAndRole(codeDepartment);
                    ViewBag.EmployeeList = empList;
                    return View("Delegation");
                }
                return RedirectToAction("ViewDelegations", "DepartmentHead");
            }
            return RedirectToAction("ViewDelegations", "DepartmentHead");
            //int idDepartmentHead=((int)Session["IdEmployee"]);
            //string submit = judge;
            //string empName = emp;
            //string remarks = cp; // for notification
            //string s1 = StartDate;
            //string s2 = EndDate;
            //if (submit.Equals("Back"))
            //{
            //    return RedirectToAction("ViewDelegations", "DepartmentHead");
            //}
            //else if (submit.Equals("Approve Delegate"))
            //{
            //    // First check if they are null
            //    if(empName!=null && !string.IsNullOrEmpty(s1) && !string.IsNullOrEmpty(s2) )
            //    {
            //        DateTime SDate = DateTime.ParseExact(s1, "dd-MM-yyyy",
            //                   System.Globalization.CultureInfo.InvariantCulture);
            //        DateTime EDate = DateTime.ParseExact(s2, "dd-MM-yyyy",
            //                        System.Globalization.CultureInfo.InvariantCulture);
            //        // Find the emp by empName--> change emp role to idRole=4, get employee Id, pass id and startdate, end date into delegation
            //        if (EDate >= SDate)
            //        {
            //            // approve delegation
            //            // add one day
            //            EDate = EDate.AddDays(1);
            //            _employeeDAO.DelegateEmployeeToActingRole(empName);
            //            _delegationDAO.UpdateDelegation(empName, SDate, EDate);

            //            //@Shutong: send notification to acting head
            //            Employee e = _employeeDAO.FindEmployeeByName(empName);
            //            int IdEmployee = e.IdEmployee;
            //            Delegation d = _delegationDAO.FindDelegationById(IdEmployee);
            //            var hub = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            //            hub.Clients.All.receiveNotification(IdEmployee);
            //            EmailClass emailClass = new EmailClass();
            //            string message = "Hi," + empName
            //                + " You are delegated to Acting Department Head from " + d.StartDate + " to " + d.EndDate 
            //                + " to assist approve stationery requisition. \r\nRemarks: "+remarks;

            //            _notificationChannelDAO.CreateNotificationsToIndividual(IdEmployee, (int)Session["IdEmployee"], message);
            //            emailClass.SendTo(_employeeDAO.FindEmployeeById(IdEmployee).Email, "SSIS System Email", message);
            //            //end of notification sending 

            //            //@Shutong: send notification head who delegating acting head
            //            Employee headDept = _employeeDAO.FindEmployeeById(idDepartmentHead);
            //            hub.Clients.All.receiveNotification(idDepartmentHead);                        
            //            message = "Hi," + headDept.Name
            //                + " You have delegated "+empName+" as Acting Department Head from " + d.StartDate.ToString() + " to " + d.EndDate.ToString()
            //                + " to assist approve stationery requisition. \r\nRemarks: " + remarks;

            //            _notificationChannelDAO.CreateNotificationsToIndividual(idDepartmentHead, (int)Session["IdEmployee"], message);
            //            emailClass.SendTo(_employeeDAO.FindEmployeeById(idDepartmentHead).Email, "SSIS System Email", message);
            //            //end of notification sending 

            //        }
            //    }
            //    return RedirectToAction("ViewDelegations", "DepartmentHead");
            //}
            //return RedirectToAction("ViewDelegations", "DepartmentHead");
        }
        public ActionResult ViewDelegations()
        {
            // display delegation
            string codeDepartment = _departmentDAO.FindCodeDepartmentByIdEmployee((int)Session["IdEmployee"]);

            ViewData["currentDelegation"] = _delegationDAO.FindCurrentDelgatiobListByDepartment(codeDepartment);
            ViewData["futureDelegation"] = _delegationDAO.FindFutureDelgatiobListByDepartment(codeDepartment);
            ViewData["pastDelegation"] = _delegationDAO.FindPastDelgatiobListByDepartment(codeDepartment);
            return View();
        }
        public ActionResult RemoveDelegation(int idDelegation, int idEmployee)
        {
            Delegation d= _delegationDAO.RemoveDelegate(idDelegation);
            
            //@Shutong: send notification here
            Employee e = _employeeDAO.FindEmployeeById(idEmployee);
            string empName = e.Name;
            int IdEmployee = e.IdEmployee;
            var hub = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            hub.Clients.All.receiveNotification(IdEmployee);
            EmailClass emailClass = new EmailClass();
            string message = "Hi," + empName
                + " your delegation assignment start on " + d.StartDate + " has been canceled.";
                
            _notificationChannelDAO.CreateNotificationsToIndividual(IdEmployee, (int)Session["IdEmployee"], message);
            emailClass.SendTo(_employeeDAO.FindEmployeeById(IdEmployee).Email, "SSIS System Email", message);
            //end of notification sending 
            return RedirectToAction("ViewDelegations", "DepartmentHead");
        }
        public ActionResult DeactivateDelegation(int idDelegation, int idEmployee)
        {
            // set the end date to today's date
            //_delegationDAO.DeactivateDelegationById(idEmployee);
            Delegation d=_delegationDAO.DeactivateDelegationByDelegationId(idDelegation);

            //@Shutong: send notification here
            Employee e = _employeeDAO.FindEmployeeById(idEmployee);
            string empName = e.Name;
            int IdEmployee = e.IdEmployee;
            var hub = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            hub.Clients.All.receiveNotification(IdEmployee);
            EmailClass emailClass = new EmailClass();
            string message = "Hi," + empName
                + " Thanks for approving stationery requisition on behalf of me! "
                + " Well done. I will now approve on my own.";

            _notificationChannelDAO.CreateNotificationsToIndividual(IdEmployee, (int)Session["IdEmployee"], message);
            emailClass.SendTo(_employeeDAO.FindEmployeeById(IdEmployee).Email, "SSIS System Email", message);
            //end of notification sending 

            return RedirectToAction("ViewDelegations", "DepartmentHead");
        }
    }
}