using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Team8ADProjectSSIS.DAO;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.Controllers
{
    public class DepartmentHeadController : Controller
    {
        EmployeeDAO _employeeDAO;
        RequisitionDAO _requisitionDAO;
        RequisitionItemDAO _requisitionItemDAO;
        ItemDAO _itemDAO;
        DepartmentDAO _deparmentDAO;
        CollectionPointDAO _collectionPointDAO;
        public DepartmentHeadController()
        {
            _employeeDAO = new EmployeeDAO();
            _requisitionDAO= new RequisitionDAO();
            _requisitionItemDAO = new RequisitionItemDAO();
            _itemDAO = new ItemDAO();
            _deparmentDAO = new DepartmentDAO();
            _collectionPointDAO = new CollectionPointDAO();
        }


        // show lists of requisitions
        public ActionResult PendingLists()
        {
            String codeDepartment = "ENGL"; // temporary use this employee id 36 with engL
            
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
            
            return RedirectToAction("PendingLists", "DepartmentHead");
        }
        public ActionResult Reject(int idRequisition)
        {
            _requisitionDAO.UpdateRejectStatus(idRequisition);
            return RedirectToAction("PendingLists", "DepartmentHead");
        }

        // view CurrentRep and CP
        public ActionResult CurrentRepCP()
        {
            String codeDepartment = "CPSC";        
            Employee employee=_employeeDAO.FindDepartmentRep(codeDepartment);
            Department department = _deparmentDAO.FindDepartmentCollectionPoint(codeDepartment);
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
                    Employee oldRep = _employeeDAO.FindDepartmentRep(codeDepartment);

                    //change old rep status back to 1
                    _employeeDAO.PutOldRepBack(oldRep.Name);

                    _employeeDAO.ChangeNewRepCP(newRep.Name, location);
                    return RedirectToAction("CurrentRepCP", "DepartmentHead");
                }
            }
            
            

            return RedirectToAction("CurrentRepCP", "DepartmentHead");
        }
        public ActionResult Delegation()
        {
            return View();
        }
        

        public ActionResult Logout()
        {
            // need to redirect to Login page
            return View();
        }
    }
}