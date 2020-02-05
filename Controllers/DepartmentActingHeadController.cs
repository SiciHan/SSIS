using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Team8ADProjectSSIS.DAO;
using Team8ADProjectSSIS.Filters;

namespace Team8ADProjectSSIS.Controllers
{
    [AuthorizeFilter]
    [AuthenticateFilter]
    public class DepartmentActingHeadController : Controller
    {

        private readonly EmployeeDAO _employeeDAO;
        private readonly RequisitionDAO _requisitionDAO;
        private readonly RequisitionItemDAO _requisitionItemDAO;
        private readonly ItemDAO _itemDAO;
        private readonly DepartmentDAO _departmentDAO;
        private readonly NotificationChannelDAO _notificationChannelDAO;
        public DepartmentActingHeadController()
        {
            _employeeDAO = new EmployeeDAO();
            _requisitionDAO = new RequisitionDAO();
            _requisitionItemDAO = new RequisitionItemDAO();
            _itemDAO = new ItemDAO();
            _departmentDAO = new DepartmentDAO();
            _notificationChannelDAO = new NotificationChannelDAO();
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
    }
}