using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Assignment3_ShongChan.Services
{
    public class NavigationHelper
    {
        // method to check if the current page is active.If the current page is active, return true
        public static bool IsActive(ViewContext viewContext, string controllerName, string actionName)
        {
            var routeData = viewContext.RouteData; 
            var currentController = viewContext.RouteData.Values["controller"]?.ToString();
            var currentAction = viewContext.RouteData.Values["action"]?.ToString(); 

            return controllerName.Equals(currentController, StringComparison.OrdinalIgnoreCase)
                   && actionName.Equals(currentAction, StringComparison.OrdinalIgnoreCase);
        }
    }
}
