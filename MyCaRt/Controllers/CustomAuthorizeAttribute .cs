using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using static MyCaRt.Enum.@enum;

namespace MyCaRt.Controllers
{
    public class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly UserRoles[] _roles;

        public CustomAuthorizeAttribute(params UserRoles[] roles)
        {
            _roles = roles;
        }
      
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var role = context.HttpContext.Session.GetInt32("Role");
            var userId = context.HttpContext.Session.GetInt32("Userid");
  

            if (role == null || (!(_roles.Contains((UserRoles)role.Value) || ((UserRoles)role == UserRoles.User))))
            {
                context.Result = new RedirectToActionResult("Error", "Error", new { statusCode = 404 });
            }
        }
    }
}
