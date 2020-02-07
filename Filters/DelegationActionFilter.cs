﻿using System;
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
        private readonly DelegationDAO delegationDAO = new DelegationDAO();
        private readonly EmployeeDAO employeeDAO = new EmployeeDAO();
        public override void OnActionExecuting(ActionExecutingContext aec)
        {

            Debug.WriteLine("OnActionExecuting", aec.RouteData);
            int IdEmployee = (int)aec.HttpContext.Session["IdEmployee"];
            string controllerName = aec.ActionDescriptor.ControllerDescriptor.ControllerName;

            if (controllerName.Contains("Employee"))
            {
                if (delegationDAO.CheckIfInDelegationPeriod(IdEmployee))
                {
                    employeeDAO.UpdateRoleToActingHead(IdEmployee);
                    HttpContext.Current.Session.Clear();
                    aec.Result = new RedirectToRouteResult(
                            new RouteValueDictionary
                            {
                        {"controller","Home" },
                        {"action","Login" }
                            });
                }
            }

            if (controllerName.Contains("ActingHead"))
            {
                if (!delegationDAO.CheckIfInDelegationPeriod(IdEmployee))
                {
                    employeeDAO.UpdateRoleToEmployee(IdEmployee);
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
}