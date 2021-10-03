using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Data.Entity;
using ICP_ABC.Models;
using Microsoft.AspNet.Identity;
using ICP_ABC.Areas.GroupsRights.Models;
using ICP_ABC.Areas.Funds.Models;
using ICP_ABC.Areas.Account.Controllers;

namespace ICP_ABC.Extentions
{
    public static class IdentityExtentions
    {
       
        static ApplicationDbContext dbContext = new ApplicationDbContext();

        //public static GroupRight HasTheRights(this IIdentity identity, string screenName )
        //{ 
        //    //ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);
        //    var user = dbContext.Users.Where(u => u.UserName == identity.Name).First();
        //    var hasright = dbContext.GroupRights
        //        .Where(r => r.GroupId == user.GroupId && r.FormID== dbContext.Screens
        //            .Where(s=>s.Name== screenName)
        //            .Select(s=>s.ScreenID)
        //            .FirstOrDefault())
        //        .FirstOrDefault();//.Include(gh => gh.UserGroup.groupRights).FirstOrDefault(u=>u.Id==AP.Id );

        //   // var claim = ((ClaimsIdentity)identity).FindFirst("hasright")
        //    // Test for null to avoid issues during local testing
        //    return hasright;
        //}
        static List<Fund> currentFunds;  
        public static List<Fund> UserFunds(this IIdentity identity)
        {
            currentFunds = currentFunds?? (List<Fund>)HttpContext.Current.Session["Funds"];
            //currentFunds = (List<Fund>)HttpContext.Current.Session["Funds"];

            if (currentFunds == null)
            {
                currentFunds= new List<Fund>();
                var UserId = identity.GetUserId();
                var user =  dbContext.Users.Where(u => u.Id == UserId).FirstOrDefault();
                currentFunds = dbContext.Funds.Where(f => dbContext.FundRights
                    .Where(fr => fr.GroupID == dbContext.UserGroups
                    .Where(g => g.GroupID == user.GroupId).FirstOrDefault().GroupID)
                    .Select(h => h.FundID).ToList().Contains(f.FundID) ).ToList();               
            }
            
            return currentFunds;
            
        }
        
            public static bool HasTheRights(this IIdentity identity, string screenName,string Right )
        {
            List<GroupRight> currentRight = new List<GroupRight>();           
            currentRight = (List<GroupRight>)HttpContext.Current.Session["Rights"];

            if (currentRight == null)
            {             
                //currentRight = (List<GroupRight>)HttpContext.Current.Items["Rights"];
                var UserId = identity.GetUserId();
                var currentUser = dbContext.Users.Where(u => u.Id == UserId).FirstOrDefault();
                currentRight = dbContext.GroupRights
                .Where(r => r.GroupId == currentUser.GroupId)
                    .ToList();
            }
            //ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);
            //var dbContext = new ApplicationDbContext();
            var user = identity.GetUserName(); //dbContext.Users.Where(u => u.UserName == UserName).First();
            bool HasRight = false;
            switch (Right)
            {
                case "Read":

                    foreach (var rightItem in currentRight)
                    {
                        if (rightItem.Screen.Name == screenName) 
                        {
                            if (rightItem.Update == true || rightItem.Create == true || rightItem.Authorized 
                                || rightItem.Read == true || rightItem.Delete == true || rightItem.Check )
                            {
                                HasRight = true;
                            }   
                           
                        }
                        

                    }

                    break;

                case "Update":

                    foreach (var rightItem in currentRight)
                    {
                        if (rightItem.Screen.Name==screenName)
                        {
                            HasRight = rightItem.Update;
                        }
                        
                    }
                    
                    break;
                case "Create":
                     //currentRight = (List<GroupRight>)HttpContext.Current.Session["rights"];
                    foreach (var rightItem in currentRight)
                    {
                        if (rightItem.Screen.Name == screenName)
                        {
                            HasRight = rightItem.Create;
                        }

                    }
                    break;
                case "Delete":
                    //currentRight = (List<GroupRight>)HttpContext.Current.Session["rights"];
                    foreach (var rightItem in currentRight)
                    {
                        if (rightItem.Screen.Name == screenName)
                        {
                            HasRight = rightItem.Delete;
                        }

                    }
                    break;
                //case "Read":
                //    //currentRight = (List<GroupRight>)HttpContext.Current.Session["rights"];
                //    foreach (var rightItem in currentRight)
                //    {
                //        if (rightItem.Screen.Name == screenName)
                //        {
                //            HasRight = rightItem.Read;
                //        }

                //    }
                //    break;
                case "Authorized":
                    
                    //currentRight = (List<GroupRight>)HttpContext.Current.Session["rights"];
                    foreach (var rightItem in currentRight)
                    {
                        if (rightItem.Screen.Name == screenName)
                        {
                            HasRight = rightItem.Authorized;
                        }

                    }
                    break;
                case "Check":
                   // currentRight = (List<GroupRight>)HttpContext.Current.Session["rights"];
                    foreach (var rightItem in currentRight)
                    {
                        if (rightItem.Screen.Name == screenName)
                        {
                            HasRight = rightItem.Check;
                        }

                    }
                    break;

                //case "Check":
                //    // currentRight = (List<GroupRight>)HttpContext.Current.Session["rights"];
                //    foreach (var rightItem in currentRight)
                //    {
                //        if (rightItem.Screen.Name == screenName)
                //        {
                //            HasRight = rightItem.Read;
                //        }

                //    }

            }   
            //var hasright = dbContext.GroupRights
            //    .Where(r => r.GroupId == user.GroupId && r.FormID == dbContext.Screens
            //        .Where(s => s.Name == screenName)
            //        .Select(s => s.ScreenID)
            //        .FirstOrDefault())
            //    .FirstOrDefault();//.Include(gh => gh.UserGroup.groupRights).FirstOrDefault(u=>u.Id==AP.Id );

            // var claim = ((ClaimsIdentity)identity).FindFirst("hasright")
            // Test for null to avoid issues during local testing
            return HasRight;
        }

    }
}