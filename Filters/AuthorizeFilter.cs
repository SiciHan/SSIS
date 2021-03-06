﻿//Shutong
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            isAuthOk = false;
            Debug.WriteLine("OnAuthorization", filterContext.RouteData);
            string role = (string)filterContext.HttpContext.Session["Role"];
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

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
                    if (role.Equals("Head"))
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
                filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary
                        {
                        {"controller","Home" },
                        {"action","Login" }
                        });
            }
        }
    }
}