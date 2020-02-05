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
        public DepartmentHeadController()
        {
            _employeeDAO = new EmployeeDAO();
            _requisitionDAO= new RequisitionDAO();
            _requisitionItemDAO = new RequisitionItemDAO();
            _itemDAO = new ItemDAO();
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
            return View();
        }

        // change Rep and CP
        public ActionResult ChangeRepCP()
        {
            return View();
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