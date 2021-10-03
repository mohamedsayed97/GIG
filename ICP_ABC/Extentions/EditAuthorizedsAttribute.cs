using ICP_ABC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ICP_ABC.Models.DBSetup;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Data.Entity.Validation;
using PagedList;
using Microsoft.AspNet.Identity.EntityFramework;
using Rotativa;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using ICP_ABC.Areas.Account.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;

namespace ICP_ABC.Extentions
{
    public class AuthorizedRightsAttribute : AuthorizeAttribute
    {
        ApplicationDbContext DbContext = new ApplicationDbContext();
        public string Screen { get; set; }
        public string Right { get; set; }
        //var user = User.Identity.GetUserId() // DbContext.Users.Where(u => u.UserName == AP).First();
        //var hasright = DbContext.GroupRights.Where(r => r.GroupId == user.GroupId).FirstOrDefault();//.Include(gh => gh.UserGroup.groupRights).FirstOrDefault(u=>u.Id==AP.Id );
        //public EditAuthorizedsAttribute(HttpContextBase httpContext, string ScreentName)
        //{
        //    var user = httpContext.User;
        //    if (user.Identity.HasTheRights(user.Identity.Name,ScreentName).None)
        //     {
        //        // Administrator => let him in
        //        return ;
        //    }


        //}


        protected override bool AuthorizeCore(HttpContextBase httpContext )
        {
            var user = httpContext.User;
            if (user.Identity.GetUserName()=="admin")
            {
                return true;
            }
            
            if (user.Identity.HasTheRights(Screen, Right))
            {
                // Administrator => let him in
                return true;
            }
            return false;
        }
    }
}