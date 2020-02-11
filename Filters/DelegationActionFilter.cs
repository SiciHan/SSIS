using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Diagnostics;
using Team8ADProjectSSIS.DAO;

namespace Team8ADProjectSSIS.Filters
{
    public class DelegationActionFilter : ActionFilterAttribute, IActionFilter
    {

        public override void OnActionExecuting(ActionExecutingContext aec)
        {

            DelegationDAO delegationDAO = new DelegationDAO();
            EmployeeDAO employeeDAO = new EmployeeDAO();
            Debug.WriteLine("OnActionExecuting", aec.RouteData);
            int IdEmployee = (int)aec.HttpContext.Session["IdEmployee"];
            string controllerName = aec.ActionDescriptor.ControllerDescriptor.ControllerName;

            if (controllerName.Contains("Employee"))
            {
                if (delegationDAO.CheckIfInDelegationPeriod(IdEmployee))
                {
                    Debug.WriteLine("OnActionExecuting should be acting head");
                    employeeDAO.UpdateRoleToActingHead(IdEmployee);
                    HttpContext.Current.Session.Clear();
                    aec.Result = new RedirectToRouteResult(
                            new RouteValueDictionary
                            {
                        {"controller","DepartmentActingHead" },
                        {"action","Notification" }
                            });
                }
            }

            if (controllerName.Contains("ActingHead"))
            {
                if (!delegationDAO.CheckIfInDelegationPeriod(IdEmployee))
                {
                    Debug.WriteLine("OnActionExecuting should be Employee");
                    employeeDAO.UpdateRoleToEmployee(IdEmployee);
                    HttpContext.Current.Session.Clear();
                    aec.Result = new RedirectToRouteResult(
                            new RouteValueDictionary
                            {
                        {"controller","Employee" },
                        {"action","Index" }
                            });
                }

            }
        }
    }
}