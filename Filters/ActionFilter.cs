using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Diagnostics;

namespace Team8ADProjectSSIS.Filters
{
    public class ActionFilter : ActionFilterAttribute, IActionFilter
    {
        private bool isAuthOk = false;
        public override void OnActionExecuting(ActionExecutingContext aec)
        {
            isAuthOk = false;
            Debug.WriteLine("OnActionExecuting", aec.RouteData);
            string role = (string)aec.HttpContext.Session["Role"];
            string controllerName = aec.ActionDescriptor.ControllerDescriptor.ControllerName;

            if (String.IsNullOrEmpty(role))
            {
                isAuthOk = false;
            }
            else {
                if (controllerName.Contains("DepartmentActingHead"))
                {
                    if (role.Contains("ActingHead"))
                    {
                        isAuthOk = true;
                    }
                }
                if (controllerName.Contains("DepartmentHead"))
                {
                    if (role.Contains("Head"))
                    {
                        isAuthOk = true;
                    }
                }
                if (controllerName.Contains("DepartmentRepresentative"))
                {
                    if (role.Contains("Representative"))
                    {
                        isAuthOk = true;
                    }
                }
                if (controllerName.Contains("Employee"))
                {
                    if(role.Contains("Employee") || role.Contains("Representative"))
                    {
                        isAuthOk = true;
                    }
                }
                if (controllerName.Contains("StoreClerk"))
                {
                    if (role.Contains("Clerk"))
                    {
                        isAuthOk = true;
                    }
                }
                if (controllerName.Contains("StoreManager"))
                {
                    if (role.Contains("Manager"))
                    {
                        isAuthOk = true;
                    }
                }
                if (controllerName.Contains("StoreSupervisor"))
                {
                    if (role.Contains("Manager")||role.Contains("Supervisor"))
                    {
                        isAuthOk = true;
                    }
                }
            }

            if (!isAuthOk)
            {
                HttpContext.Current.Session.Clear();
                aec.Result = new RedirectToRouteResult(
                        new RouteValueDictionary
                        {
                        {"controller","Home" },
                        {"action","Login" }
                        });
            }
        }
    }
}