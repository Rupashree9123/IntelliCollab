using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntelliCollabUI
{
    public class Authentication : FilterAttribute, IAuthorizationFilter
    {
        void IAuthorizationFilter.OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Session["UserID"] == null && filterContext.HttpContext.Session["AssessList"] == null)
            {
                filterContext.Result = new RedirectResult("/LogIn/LogIn");
            }
        }
    }
}