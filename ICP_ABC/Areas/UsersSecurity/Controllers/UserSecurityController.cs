using ICP_ABC.Areas.UsersSecurity.Models;
using ICP_ABC.Models;
using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ICP_ABC.Areas.UsersSecurity.Controllers
{
    public class UserSecurityController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext dbContext = new ApplicationDbContext();

        // GET: UsersSecurity/UserSecurity

        public ActionResult Index()
        {
            var HaveRecord = dbContext.UserSecurities.Count();
            if (dbContext.UserSecurities.Count()==0)
            {
                ViewBag.FirstTime = true;
                return View();
            }
            else
            {
                ViewBag.FirstTime = false;
                return View(dbContext.UserSecurities.FirstOrDefault());               
            }
          
        }
        public ActionResult Create()
        {
            if (dbContext.UserSecurities.Count()> 0)
            {

                return RedirectToAction("Index");
            }

            return View();
        }
        [HttpPost]
        public ActionResult Create(UserSecurity model )
        {
            if (dbContext.UserSecurities.Count() > 0)
            {
                throw new System.ArgumentException("Parameter cannot be null", "original");
            }
            UserSecurity userSecurity = new UserSecurity
            {
                Levels = model.Levels,
                ExpireInterval = model.ExpireInterval,
                NumberOfTrials = model.NumberOfTrials,
                UserID = User.Identity.GetUserId(),
                Maker = User.Identity.GetUserId(),
                SysDate = DateTime.Now,
                DeletFlag = DeleteFlag.NotDeleted,
                ViewTransaction = model.ViewTransaction,
               CreateTransaction = model.CreateTransaction,
               Auth = false,
                Chk = false,
                EditFlag = false,
            };
            dbContext.UserSecurities.Add(userSecurity);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        public ViewResult Edit()
        {
            var UserSecu = dbContext.UserSecurities.FirstOrDefault();
            
            return View(UserSecu);
        }
        [HttpPost]
        public ActionResult Edit(UserSecurity model)
        {
            var userSecurity = dbContext.UserSecurities.FirstOrDefault();
            userSecurity.Levels = model.Levels;
            userSecurity.ExpireInterval = model.ExpireInterval;
            userSecurity.NumberOfTrials = model.NumberOfTrials;
            userSecurity.UserID = User.Identity.GetUserId();
            userSecurity.Maker = User.Identity.GetUserId();
            userSecurity.SysDate = DateTime.Now;
            userSecurity.DeletFlag = DeleteFlag.NotDeleted;
            userSecurity.ViewTransaction =model.ViewTransaction;
            userSecurity.CreateTransaction = model.CreateTransaction;
            userSecurity.Auth = false;
            userSecurity.Chk = false;
            userSecurity.EditFlag = false;

            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }


        

    }
}