using IntelliCollabUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntelliCollabUI
{
    public class RoleFilter : FilterAttribute, IActionFilter
    {
        void IActionFilter.OnActionExecuted(ActionExecutedContext filterContext)
        {
            //throw new NotImplementedException();
        }

        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            //throw new NotImplementedException();
            var actionName = filterContext.ActionDescriptor.ActionName;
            var controllerName = filterContext.Controller.GetType().Name;
           
            var roleList = (List<AccessRole>)(filterContext.HttpContext.Session["AssessList"]);

            if(!roleList.Select(x=> x.Controller).Contains(controllerName) || !roleList.Select(x => x.Actions).Contains(actionName))
            {
                filterContext.Result = new RedirectResult($"../DashBoard/Index");
            }

        }
    }
}