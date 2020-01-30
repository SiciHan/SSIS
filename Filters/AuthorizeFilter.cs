﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Team8ADProjectSSIS.Filters
{
    public class AuthorizeFilter : ActionFilterAttribute, IAuthorizationFilter
    {
        private bool isAuthOk = false;
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            string role = (string)HttpContext.Current.Session["Role"];
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
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
                if (role.Contains("StoreClerk"))
                {
                    isAuthOk = true;
                }
            }
            if (controllerName.Contains("StoreManager"))
            {
                if (role.Contains("StoreManager"))
                {
                    isAuthOk = true;
                }
            }
            if (controllerName.Contains("StoreSupervisor"))
            {
                if (role.Contains("StoreManager")||role.Contains("StoreSupervisor"))
                {
                    isAuthOk = true;
                }
            }
            if (!isAuthOk)
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        {"controller","Home" },
                        {"action","Logout" }
                    });

        }
    }
}