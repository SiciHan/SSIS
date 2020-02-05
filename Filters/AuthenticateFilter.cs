using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;

namespace Team8ADProjectSSIS.Filters
{
    public class AuthenticateFilter : ActionFilterAttribute, IAuthenticationFilter

    {
        private bool IsAuthOK = false;
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            //if the session does not expire yet
            var sessionId = HttpContext.Current.Session["sessionId"];
            if (sessionId != null && sessionId is Guid)
            {
                IsAuthOK = true;
            }
            //If authentication failed
            else {
                //session expired
                if (sessionId == null) {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary{
                        { "controller", "Home" },
                        { "action", "Login" } }
                      );
                }
                //illegal user
                else if(!(sessionId is Guid))
                {
                    filterContext.Result = new HttpUnauthorizedResult();
                }
            }
        }
        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            if (!IsAuthOK)
            {
                HttpContext.Current.Session.Clear();
            }
            
        }
    }
}