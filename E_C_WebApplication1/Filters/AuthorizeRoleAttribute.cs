using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using E_C_WebApplication1.Models;

public class AuthorizeRoleAttribute : AuthorizeAttribute
{
    private readonly string[] allowedRoles;
    public AuthorizeRoleAttribute(params string[] roles)
    {
        this.allowedRoles = roles;
    }

    protected override bool AuthorizeCore(HttpContextBase httpContext)
    {
        bool authorize = false;
        var userName = httpContext.User.Identity.Name;
        if (!string.IsNullOrEmpty(userName))
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var userRole = (from u in db.Users
                                join r in db.Roles on u.RoleId equals r.RoleId
                                where u.Username == userName
                                select r.RoleName).FirstOrDefault();

                foreach (var role in allowedRoles)
                {
                    if (role == userRole) return true;
                }
            }
        }
        return authorize;
    }

    protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
    {
        filterContext.Result = new RedirectResult("/Account/Login");
    }
}
