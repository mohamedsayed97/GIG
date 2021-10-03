using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using ICP_ABC.Models;
using ICP_ABC.Models.DBSetup;
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
using ICP_ABC.Extentions;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.ApplicationBlocks.Data;
using System.Data;
using ICP_ABC.Areas.UsersSecurity.Models;
//using ICP_ABC.Areas.GroupsRights.Models;
using UpDated_2005;

namespace ICP_ABC.Areas.Account.Controllers
{//
   [Authorize]
    public class AccountController : Controller
    {
        
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext dbContext = new ApplicationDbContext();
        
        static string[] IDs;
        
        public AccountController()
        {
            //var currentuser = User.Identity.Name;
            //Mode = dbContext.Users.Where(s => s.UserName == currentuser).Select(s => s.DarkMode).FirstOrDefault();
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ActionResult Index()
        {
            //var currentuser = UserManager.FindById(User.Identity.GetUserId());
            //var Rights = dbContext.UserGroups
            //    .Include(g => g.groupRights.Where(gh=>gh.GroupId==g.GroupID))
            //    .Where(s => s.GroupID == currentuser.GroupId)
            //    .Select(g => g.GroupID == currentuser.GroupId);
            return View();
        }

        //public ActionResult ChangeColor(string Name,bool Checked)
        //{
        //    var user = dbContext.Users.Where(s => s.UserName == Name).FirstOrDefault();
        //    user.DarkMode = Checked;
        //    Mode = user.DarkMode;
        //    dbContext.SaveChanges();
        //    Response.StatusCode = (int)HttpStatusCode.OK;
        //    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        //}


        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
         {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home",new { area="Home"});
            }
            else
            {
                return View("Login");
            }
            //return RedirectToLocal("Account/Account/Login");
            //ViewBag.ReturnUrl = returnUrl;
            //return RedirectToAction("Login","Account/Account/Login");//new {area="Account",controller="Account",action="Login" });
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            
            

            SignInStatus result = new SignInStatus();
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var user = UserManager.FindByNameAsync(model.UserName).Result;
            
            if (user != null && user.DeleteFlag == DeleteFlag.Deleted && user.UserName != "admin")
            {
                result = SignInStatus.LockedOut;
                string[] errors = new string[] { "This Account Is Not Exist Or has been Deleted, Plz Contact the Admin For More Info." };
                IdentityResult LoginResult = new IdentityResult(errors);
                AddErrors(LoginResult);
                return View();
            }
            if (user!=null && user.CloseDueDate< DateTime.Now )
            {
                result= SignInStatus.LockedOut;
                return View("Lockout");
            }
            if (user != null && !user.Auth)
            {
                result = SignInStatus.LockedOut;
                string[] errors = new string[] { "You Are Not Authorized to loging plz Contact Admin" };
                IdentityResult LoginResult = new IdentityResult(errors);
                AddErrors(LoginResult);
                return View();
            }

            if (user != null && user.IsLogged && user.UserName != "admin")
            {
                result = SignInStatus.LockedOut;
                string[] errors = new string[] { "you're trying to login from another place or u didn't loged out correctly, Plz Contact the Admin to Force LogOut" };
                IdentityResult LoginResult = new IdentityResult(errors);
                AddErrors(LoginResult);
                return View();
            }
            

                if (user != null && user.IsLocked )
            {              
                UserManager.SetLockoutEnabled(user.Id, enabled:true);
                result = SignInStatus.LockedOut;
                return View("Lockout");
            }

            //expirDate----------
            if (user != null && user.ExpireDate<=DateTime.Now && user.UserName != "admin")
            {
                ViewData["UserName"] = user.UserName;
              
                return View("ChangeExpiredPassword");
               
            }
           
            //---------------
            try
            {
               var results = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: true);
              
            }
            catch (DbEntityValidationException e)
            {

                return View(model);
            }
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
             result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
          
        
                switch (result)
            {
                case SignInStatus.Success:
                    ChangeLogingStatus(model.UserName);

                    UpdateRights(user);

                    UpdateUserFunds(user);
                    //     HttpContext.Items.Add("Rights", Rights);

                    //var x = HttpContext.Session["Rights"];
                    return RedirectToLocal("Index");
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:

                   
               
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AuthorizedRights(Screen = "_users", Right = "Create")]
        public ActionResult Register()
        {          
            ViewData["Groups"] = new SelectList(dbContext.UserGroups.ToList(), "GroupID", "Name");
            ViewData["Branches"] = new SelectList(dbContext.Branches.ToList(), "BranchID", "BName");
            ViewData["Positions"] = new SelectList(dbContext.Titles.ToList(), "TitleID", "Name");
            var expiredate = DateTime.Now;
            
           var ex=  expiredate.AddDays(dbContext.UserSecurities.Select(u => u.ExpireInterval).FirstOrDefault());           
            ViewData["expiredate"] = ex.ToString("dd/MM/yyyy");//{9/23/2019 12:00:00 AM}
            var cd = DateTime.Now.AddYears(10);
            ViewData["CloseDueDate"] = cd.ToString("dd/MM/yyyy");
            //var lastcode = dbContext.Users.OrderByDescending(s => s.Code).Select(s => s.Code).FirstOrDefault();
            var lastcode = GetLastCode();


            var trieng = int.TryParse(lastcode, out int Code);
            if (trieng)
            {
                ViewData["LastCode"] = Code + 1;
            }
            else
            {
                ViewData["LastCode"] = 1;
            }

            return View();
        }

        //GetLastCode
        public string GetLastCode()
        {
            var querey = "SELECT MAX(CAST(Code AS INT)) as MaxCode FROM AspNetUsers";
            var appSettings = ConfigurationManager.ConnectionStrings;
            var SqlCon = appSettings["ICPRO"];
            var ReturnedData = SqlHelper.ExecuteDataset(SqlCon.ToString(), CommandType.Text, querey);
            var LastCode = ReturnedData.Tables[0].Rows[0][0].ToString();
            return LastCode;
        }
        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizedRights(Screen = "_users", Right = "Create")]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            //var x = DateTime.ParseExact(ExpireDate, "dd/MM/yyy",null);
            ViewData["Groups"] = new SelectList(dbContext.UserGroups.ToList(), "GroupID", "Name");
            ViewData["Branches"] = new SelectList(dbContext.Branches.ToList(), "BranchID", "BName");
            ViewData["Positions"] = new SelectList(dbContext.Titles.ToList(), "TitleID", "Name");
            var expiredate = DateTime.Now;
            var ex = expiredate.AddDays(dbContext.UserSecurities.Select(u => u.ExpireInterval).FirstOrDefault());
            ViewData["expiredate"] = ex.ToString("dd/MM/yyyy");//{9/23/2019 12:00:00 AM}
            var cd = DateTime.Now.AddYears(10);
            ViewData["CloseDueDate"] = ex.ToString("dd/MM/yyyy");
            //var lastcode = dbContext.Users.OrderByDescending(s => s.Code).Select(s => s.Code).FirstOrDefault();
            //ViewData["LastCode"] = int.Parse(lastcode) + 1;
            //if (model.ExpireDate < DateTime.Now)
            //{
            //    string[] errors = new string[] { "time is lower than we excepect" };
            //    IdentityResult result = new IdentityResult(errors);
            //    AddErrors(result);
            //    return View();
            //}
            if (dbContext.Users.Select(s=>s.Code).Contains(model.Code))
            {
                string[] errors = new string[] { "Code Is Already Exist" };
                IdentityResult result = new IdentityResult(errors);
                AddErrors(result);
                return View();
            }
            //var lastcode = dbContext.Users.OrderByDescending(s => s.Code).Select(s => s.Code).FirstOrDefault();
            var lastcode = GetLastCode();

            

            var trieng = int.TryParse(lastcode, out int Code);
            if (trieng)
            {
                lastcode =(Code + 1).ToString();
            }
            else
            {
                lastcode = 1.ToString();
            }

            var obj = new UpDated_2005.Common();
            var Level = dbContext.UserSecurities.Select(Val => Val.Levels).FirstOrDefault();
            if (Level == Levels.TwoLevels)
            {
                if (ModelState.IsValid)
                {
                    //var MakerId = User.Identity.GetUserId();
                    var user = new ApplicationUser
                    {
                        UserName = model.UserName,
                        //Email = "",
                        //UserId=model.Code.ToString() ,//
                        //Password= model.Password,
                        BranchId = model.BranchID, //model.BranchID,
                        Code = lastcode,
                        GroupId = model.GroupID, //model.GroupID,
                        TitleId = model.PositionID, //model.PositionID,
                        BranchRight = model.BranchRight,
                        UserId = dbContext.Users.Where(u => u.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                        IsLocked = false,
                        EditFlag = false,
                        DeleteFlag = DeleteFlag.NotDeleted,//Models.IsDeleted.NotDeleted,
                        CloseDueDate = DateTime.Now.AddYears(10),
                        Maker = User.Identity.GetUserId(),
                        Checker = User.Identity.GetUserId(),
                        Auth = false,
                        Chk = true,
                        ExpireDate = ex,
                        FullName = model.FullName,
                        Email = model.Email,
                        PhoneNumber = "010",
                        IsAdmin = false,
                        CreationDate = DateTime.Now,
                        EmailConfirmed = false,
                        UnBlockRight = model.UnBlockRight,
                        defaultHashedPassword = obj.cipher_cls.Encryption(model.Password)


                    };

                    var SecondTable = new User
                    {

                        UserName = model.UserName,
                        BranchID = model.BranchID, //model.BranchID,
                        Code = lastcode,
                        GroupID = model.GroupID, //model.GroupID,
                        Position = model.PositionID, //model.PositionID,
                        BranchRight = model.BranchRight,
                        Password = model.Password,
                        LockUser = false,
                        EditFlag = false,
                        DeleteFlag = DeleteFlag.NotDeleted,//Models.IsDeleted.NotDeleted,
                        UserIDForMaker = User.Identity.GetUserId(),
                        ExpireDate = ex,
                        Admin = false,
                        SysDate = DateTime.Now
                    };


                    //i commented this to prevent the identity of login the the registered account 
                    try
                    {
                        var resultd = await UserManager.CreateAsync(user, model.Password);
                        if (resultd.Succeeded)
                        {

                            //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                            // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                            // Send an email with this link
                            // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                            // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                            // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                            //return RedirectToAction("Index", "Home");
                            dbContext.User.Add(SecondTable);
                            try
                            {
                                dbContext.SaveChanges();
                            }
                            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                            {
                                Exception raise = dbEx;
                                foreach (var validationErrors in dbEx.EntityValidationErrors)
                                {
                                    foreach (var validationError in validationErrors.ValidationErrors)
                                    {
                                        string message = string.Format("{0}:{1}",
                                            validationErrors.Entry.Entity.ToString(),
                                            validationError.ErrorMessage);
                                        // raise a new exception nesting
                                        // the current instance as InnerException
                                        raise = new InvalidOperationException(message, raise);
                                    }
                                }
                                throw raise;
                            }


                            return RedirectToAction("Details", new { UserName = user.UserName });
                            //return RedirectToAction("Search");
                        }
                        else
                        {

                            AddErrors(resultd);
                            ViewData["LastCode"] = lastcode;
                            return View(model);
                        }
                    }
                    catch (DbEntityValidationException e)
                    {

                        foreach (var eve in e.EntityValidationErrors)
                        {
                            Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                eve.Entry.Entity.GetType().Name, eve.Entry.State);
                            foreach (var ve in eve.ValidationErrors)
                            {
                                Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                    ve.PropertyName, ve.ErrorMessage);
                            }
                        }
                        var resultss = await UserManager.CreateAsync(user, model.Password);


                        AddErrors(resultss);
                        return View(model);
                    }

                    //  var result = await UserManager.CreateAsync(user, model.Password);
                    //if (result.Succeeded)
                    //{

                    //    //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                    //    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    //    // Send an email with this link
                    //    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    //    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    //    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    //    //return RedirectToAction("Index", "Home");
                    //    dbContext.User.Add(SecondTable);
                    //    try
                    //    {
                    //        dbContext.SaveChanges();
                    //    }
                    //    catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                    //    {
                    //        Exception raise = dbEx;
                    //        foreach (var validationErrors in dbEx.EntityValidationErrors)
                    //        {
                    //            foreach (var validationError in validationErrors.ValidationErrors)
                    //            {
                    //                string message = string.Format("{0}:{1}",
                    //                    validationErrors.Entry.Entity.ToString(),
                    //                    validationError.ErrorMessage);
                    //                // raise a new exception nesting
                    //                // the current instance as InnerException
                    //                raise = new InvalidOperationException(message, raise);
                    //            }
                    //        }
                    //        throw raise;
                    //    }


                    //    return RedirectToAction("Details", new { UserName = user.UserName });
                    //}
                    //AddErrors(result);
                }
            }
            else
            {
                if (ModelState.IsValid)
                {
                    //var MakerId = User.Identity.GetUserId();
                    var user = new ApplicationUser
                    {
                        UserName = model.UserName,
                        //Email = "",
                        //UserId=model.Code.ToString() ,//
                        //Password= model.Password,
                        BranchId = model.BranchID, //model.BranchID,
                        Code = lastcode,
                        GroupId = model.GroupID, //model.GroupID,
                        TitleId = model.PositionID, //model.PositionID,
                        BranchRight = model.BranchRight,
                        UserId = dbContext.Users.Where(u => u.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault(),
                        IsLocked = false,
                        EditFlag = false,
                        DeleteFlag = DeleteFlag.NotDeleted,//Models.IsDeleted.NotDeleted,
                        CloseDueDate = DateTime.Now.AddYears(10),
                        Maker = User.Identity.GetUserId(),
                        Auth = false,
                        ExpireDate = ex,
                        FullName = model.FullName,
                        Email = model.Email,
                        PhoneNumber = "010",
                        IsAdmin = false,
                        CreationDate = DateTime.Now,
                        EmailConfirmed = false,
                        UnBlockRight = model.UnBlockRight,
                        defaultHashedPassword = obj.cipher_cls.Encryption(model.Password)



                    };

                    var SecondTable = new User
                    {

                        UserName = model.UserName,
                        BranchID = model.BranchID, //model.BranchID,
                        Code = lastcode,
                        GroupID = model.GroupID, //model.GroupID,
                        Position = model.PositionID, //model.PositionID,
                        BranchRight = model.BranchRight,
                        Password = model.Password,
                        LockUser = false,
                        EditFlag = false,
                        DeleteFlag = DeleteFlag.NotDeleted,//Models.IsDeleted.NotDeleted,
                        UserIDForMaker = User.Identity.GetUserId(),
                        ExpireDate = ex,
                        Admin = false,
                        SysDate = DateTime.Now
                    };


                    //i commented this to prevent the identity of login the the registered account 
                    try
                    {
                        var resultd = await UserManager.CreateAsync(user, model.Password);
                        if (resultd.Succeeded)
                        {

                            //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                            // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                            // Send an email with this link
                            // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                            // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                            // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                            //return RedirectToAction("Index", "Home");
                            dbContext.User.Add(SecondTable);
                            try
                            {
                                dbContext.SaveChanges();
                            }
                            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                            {
                                Exception raise = dbEx;
                                foreach (var validationErrors in dbEx.EntityValidationErrors)
                                {
                                    foreach (var validationError in validationErrors.ValidationErrors)
                                    {
                                        string message = string.Format("{0}:{1}",
                                            validationErrors.Entry.Entity.ToString(),
                                            validationError.ErrorMessage);
                                        // raise a new exception nesting
                                        // the current instance as InnerException
                                        raise = new InvalidOperationException(message, raise);
                                    }
                                }
                                throw raise;
                            }


                            return RedirectToAction("Details", new { UserName = user.UserName });
                            //return RedirectToAction("Search");
                        }
                        else
                        {

                            AddErrors(resultd);
                            ViewData["LastCode"] = lastcode;
                            return View(model);
                        }
                    }
                    catch (DbEntityValidationException e)
                    {

                        foreach (var eve in e.EntityValidationErrors)
                        {
                            Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                eve.Entry.Entity.GetType().Name, eve.Entry.State);
                            foreach (var ve in eve.ValidationErrors)
                            {
                                Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                    ve.PropertyName, ve.ErrorMessage);
                            }
                        }
                        var resultss = await UserManager.CreateAsync(user, model.Password);


                        AddErrors(resultss);
                        return View(model);
                    }

                    //  var result = await UserManager.CreateAsync(user, model.Password);
                    //if (result.Succeeded)
                    //{

                    //    //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                    //    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    //    // Send an email with this link
                    //    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    //    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    //    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    //    //return RedirectToAction("Index", "Home");
                    //    dbContext.User.Add(SecondTable);
                    //    try
                    //    {
                    //        dbContext.SaveChanges();
                    //    }
                    //    catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                    //    {
                    //        Exception raise = dbEx;
                    //        foreach (var validationErrors in dbEx.EntityValidationErrors)
                    //        {
                    //            foreach (var validationError in validationErrors.ValidationErrors)
                    //            {
                    //                string message = string.Format("{0}:{1}",
                    //                    validationErrors.Entry.Entity.ToString(),
                    //                    validationError.ErrorMessage);
                    //                // raise a new exception nesting
                    //                // the current instance as InnerException
                    //                raise = new InvalidOperationException(message, raise);
                    //            }
                    //        }
                    //        throw raise;
                    //    }


                    //    return RedirectToAction("Details", new { UserName = user.UserName });
                    //}
                    //AddErrors(result);
                }
            }


           

            // If we got this far, something failed, redisplay form
            return View();
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [Authorize(Users="admin")]
        public async Task <ActionResult> ResetPassword(string UserName)
        {
            string relativePath = ConfigurationManager.AppSettings["filePath"];
            string combinedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
            var fileContents = System.IO.File.ReadAllText(path:relativePath);
            //var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            //var manager = new UserManager<ApplicationUser>(store);
            //var currentUser = manager.FindByName(UserName);
            //var ResetPasswordViewModel = new ResetPasswordViewModel
            //{
            //    UserName = currentUser.UserName
            //};
            //return View(ResetPasswordViewModel);

            //return code == null ? View("Error") : View();
            
            var user = await UserManager.FindByNameAsync(UserName);
            
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("Search", "Account");
            }
            //var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            //var manager = new UserManager<ApplicationUser>(store);
            //var currentUser = manager.FindByName(model.UserName);
            // var ModelPassword = manager.PasswordHasher.HashPassword(model.OldPassword);
            //var ValidPass = manager.CheckPassword(currentUser, model.Password);   
            var passwordValidator = UserManager.PasswordValidator;
            var result = await passwordValidator.ValidateAsync(fileContents);
            if (result.Succeeded)
            {
                user.PasswordHash = UserManager.PasswordHasher.HashPassword(fileContents);
                user.ExpireDate = DateTime.Now.AddDays(30);
                //var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
                var manager = new UserManager<ApplicationUser>(store);
                var currentUser = manager.FindByName(UserName);
                currentUser.PasswordHash= UserManager.PasswordHasher.HashPassword(fileContents);
                manager.Update(currentUser);
                //currentUser.ExpireDate.AddDays(30);
                var ctx = store.Context;
                ctx.SaveChanges();
                return RedirectToAction("Search", "Account");
            }
           TempData["Err"] =result;
            return RedirectToAction("Search", "Account");
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
       
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var passwordValidator = UserManager.PasswordValidator;
            var result = await passwordValidator.ValidateAsync("ABCDEF");
            if (result.Succeeded)
            {
                user.PasswordHash = UserManager.PasswordHasher.HashPassword("ABC@123");
                user.ExpireDate = DateTime.Now.AddDays(30);
                var ctx = store.Context;
                ctx.SaveChanges();
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult ChangeExpiredPassword()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ChangeExpiredPassword(ChangeExpiredPasswordViewModel model)
        {
            //ModelState
            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var manager = new UserManager<ApplicationUser>(store);
            var currentUser = manager.FindByName(model.UserName);
            var NewNassIsNotTheOld = SignInManager.PasswordSignIn(currentUser.UserName, model.NewPassword,true,true);
            var ValidPass = manager.CheckPassword(currentUser, model.OldPassword);
            if (NewNassIsNotTheOld == SignInStatus.Success)
            {
                ValidPass = false;
            }
            //var oldpasses = dbContext.OldHachedPasswords.Where(o => o.UserID == currentUser.Id).Select(o=>o.HachedPass).Take(5).ToList();

            var newpass = manager.PasswordHasher.HashPassword(model.NewPassword);
            var ctx = store.Context;
            //foreach (var oldpass in oldpasses)
            //{
            //    if (oldpass == newpass)
            //    {
            //        ValidPass = false;
            //    }
            //}
            
            if (ValidPass)
            {
                try
                {
                    var oldpass = new OldHachedPasswords
                    {
                        UserID = currentUser.Id,
                        HachedPass = currentUser.PasswordHash

                    };
                    //dbContext.OldHachedPasswords.Add(oldpass);
                    currentUser.PasswordHash = manager.PasswordHasher.HashPassword(model.NewPassword);
                    var NumberOfAddDays =dbContext.UserSecurities.Select(x => x.ExpireInterval).FirstOrDefault();
                    currentUser.ExpireDate = DateTime.Now.AddDays(NumberOfAddDays);
                    ctx.SaveChanges();
                    dbContext.SaveChanges();
                   

                }
                catch (Exception e)
                {
                    var errs = e;
                    throw;
                }
                
            }
            else
            {
                string[] errors = new string[] { "this passwored used before try new one" };
                IdentityResult result = new IdentityResult(errors);
                AddErrors(result);
                return View();
            }
            return RedirectToAction("Login");
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public  ActionResult LogOff()
        {

            ////var aft = "VrNtqJCxKJ4h46lUGt31ch27__Rc09qchZTZWmL-zHFf53EZQcSBftSffTIyha73eUI2kNHKk0emGaytAXsDUpYASJvYjTvG73HnsmZ9Z_MqZeqAB7YX4WUED3hGOzeZrpBuf5G0kplaZR0J7Isw6w2";
            //var m=Request.Params.GetValues("__RequestVerificationToken");
            //var x = Request;

            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var manager = new UserManager<ApplicationUser>(store);
            
            var currentUser = manager.FindByName(User.Identity.GetUserName());
            currentUser.IsLogged = false;
             manager.Update(currentUser);
            var ctx = store.Context;
             ctx.SaveChanges();
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }

        public ActionResult ForceLogOff(string UserName)
        {
            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var manager = new UserManager<ApplicationUser>(store);

            var currentUser = manager.FindByName(UserName);
            currentUser.IsLogged = false;
            manager.Update(currentUser);
            var ctx = store.Context;
            ctx.SaveChanges();
            
            return RedirectToAction("Search");
        }
        private void ChangeLogingStatus(string UserName)
        {
            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var manager = new UserManager<ApplicationUser>(store);
            var name = User.Identity.GetUserName();
            var currentUser = manager.FindByName(UserName);
            currentUser.IsLogged = true;
            manager.Update(currentUser);
            var ctx = store.Context;
            ctx.SaveChanges();
          
            
        }
        [AuthorizedRights(Screen = "_users", Right = "Read")]
        public ViewResult Search(string sortOrder,string RadioCHeck, string currentFilter, string searchString, int? page, string Branch,string Group, string Code, string Title, string Branches,string Groups, string CodeString , string name )
        {
            int seculevel = (int)dbContext.UserSecurities.FirstOrDefault().Levels;
            ViewBag.SecuLevel = seculevel;
            ViewData["Groups"] = new SelectList(dbContext.UserGroups.ToList(), "GroupID", "Name");
            ViewData["Branches"] = new SelectList(dbContext.Branches.ToList(), "BranchID", "BName");

            if (page == null && sortOrder == null && RadioCHeck == null && currentFilter == null && searchString == null  && Title == null && Branch == null && CodeString == null  && Branches == null  && Code == null && Groups == null  && Group == null && name == null)
            {
                int pageSize = 3;
                int pageNumber = (page ?? 1);
                var Users = dbContext.Users.ToList().Take(0);
               return View(Users.ToPagedList(pageNumber, pageSize));
                   
            }
            else { 
            //ViewData["color"] = Mode;
            if (TempData["Err"] != null)
            {
                string[] errors = new string[] { "the password in the file is not a valid password" };
                IdentityResult result = new IdentityResult(errors);
                AddErrors(result);
            }
            
            //if (searchString != null)
            //{
            //    page = 1;
            //}
            //else
            //{

            //    searchString = currentFilter;
            //}

            //int Branchcheckdata = 0;
            //int Groupcheckdata = 0;
            var Users = dbContext.Users.Where(c => c.DeleteFlag == DeleteFlag.NotDeleted).Select(s => s);
            var currentuserId = User.Identity.GetUserId();
            var currentUser = dbContext.Users.Where(u => u.Id == currentuserId).First();

                
            if (! currentUser.BranchRight)
            {
                Users = Users.Where(u => u.BranchId == currentUser.BranchId);
            }
            //barnches dropdown search
            if (!String.IsNullOrEmpty(Branches))
            {
                int BranchId;
                BranchId = int.Parse(Branches);
                Users = dbContext.Users
               .Where(u => u.BranchId == BranchId)
               .Select(s => s);
                ViewBag.CurrentFilter = searchString == null ? Branches : searchString;
            }
            // groups dropdown search
              if (!String.IsNullOrEmpty(Groups))
            {
                int GroupId;
                GroupId = int.Parse(Groups);
                Users = dbContext.Users
               .Where(u => u.GroupId == GroupId)
               .Select(s => s);
                ViewBag.CurrentFilter = searchString == null ? Groups : searchString;
            }
            else
            {
                ViewBag.CurrentFilter = searchString;
            }

                //checkRadio 
                if (!String.IsNullOrEmpty(RadioCHeck))
                {
                    //Case Auth
                    if (RadioCHeck == "1")
                    {
                        Users = Users.Where(s => s.Auth == true);

                    }
                    //Case Checker
                    if (RadioCHeck == "2")
                    {
                        Users = Users.Where(s => s.Chk == true && s.Auth == false);

                    }
                    //Case maker
                    if (RadioCHeck == "3")
                    {
                        Users = Users.Where(s => s.Auth == false && s.Chk == false);

                    }
                    //Case All
                    if (RadioCHeck == "4")
                    {
                        Users = Users.Where(c => c.DeleteFlag == DeleteFlag.NotDeleted);

                    }


                }
                //


                TempData["UserForExc"] = Users.Select(x => x.Code).ToList();
                //set sortorder
                if (!String.IsNullOrEmpty(Branch))
            {
                sortOrder = "Branch";
            }

            else if (!String.IsNullOrEmpty(Group))
            {
                sortOrder = "Group";
            }
            else if(!String.IsNullOrEmpty(Code))
            {
                sortOrder = "Code";
            }
            else if (!String.IsNullOrEmpty(Title))
            {
                sortOrder = "Title";
            }
            else if (!String.IsNullOrEmpty(name))
            {
                sortOrder = "name";
            }
            else
            {
                //sortOrder = !String.IsNullOrEmpty(sortOrder)?sortOrder:"name";
            }


            ViewBag.DateSortParm = sortOrder;
            ViewBag.CurrentSort = sortOrder;
            if (!String.IsNullOrEmpty(searchString))
            {
                Users = Users.Where(s => s.UserName.StartsWith(searchString));
            }
            if (!String.IsNullOrEmpty(CodeString))
            {                
                Users = Users.Where(s => s.Code.StartsWith(CodeString));
            }
            //sorting
            switch (sortOrder)
            {
                case "name":
                    Users = Users.OrderByDescending(s => s.UserName);
                    break;
                case "Group":
                    Users = Users.OrderByDescending(s => s.UserGroup.Name);
                    break;
                case "Title":
                    Users = Users.OrderByDescending(s => s.Title.Name);
                    break;
                case "Code":
                    Users = Users.OrderBy(s => s.Code);
                    break;
                case "Branch Name":
                    Users = Users.OrderByDescending(s => s.Branch.BName);
                    break;
                default:  // Name ascending 
                    Users = Users.OrderBy(s => s.Code);
                    break;
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);

                ViewData["radioCHeck"] = RadioCHeck;
                ViewData["searchString"] = searchString;
                ViewData["Group"] = Group;
                ViewData["Branch"] = Branch;
                ViewData["CodeString"] = CodeString;

                int count = Users.ToList().Count();
            IDs = new string[count];
            IDs = Users.Select(s => s.Code).ToArray();

                int[] myInts = Array.ConvertAll(IDs, s => int.Parse(s));
                Array.Sort(myInts);
                IDs = myInts.Select(x => x.ToString()).ToArray();

                return View(Users.ToPagedList(pageNumber, pageSize));
            }
        }

        [AuthorizedRights(Screen = "_users", Right = "Read")]
        public ActionResult ExportToPDF()
        {
            var users = dbContext.Users.Select(s => s).Where(s =>IDs.Contains(s.Code) ).OrderBy(u=>u.UserName).ToList();
            string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");
           
            var report = new PartialViewAsPdf("~/Areas/Account/Views/Account/ExportToPDF.cshtml", users);
            return report;
            
        }
        [AuthorizedRights(Screen = "_users", Right = "Read")]
        public ActionResult ExportToExcel()
        {
            string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");
            var gv = new GridView();
            
            var List = (List<string>)TempData["UserForExc"];
             gv.DataSource = dbContext.Users.Select(s => s).Where(s => IDs.Contains(s.Code)).Where(Del => List.Contains(Del.Code) && Del.DeleteFlag == DeleteFlag.NotDeleted).OrderBy(u => u.UserName).Select(x=>new{x.Code,x.UserName,BarnchName=x.Branch.BName,Group=x.UserGroup.Name, Title=x.Title.Name,LockedUser =x.IsLocked}).ToList();

            //gv.DataSource = (from s in dbContext.Users
            //                 select new
            //                 {   s.Code,
            //                     s.UserName,
            //                     s.Title.Name,
            //                     GrouopName = s.UserGroup.Name ,
            //                     BranchName =s.Branch.BName
            //                 })
            //                     .ToList();                           
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Users"+NowTime+".xls");
            Response.ContentType = "application/ms-excel";
            Response.ContentEncoding = System.Text.Encoding.Unicode;
            Response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble());

            Response.Charset = "UTF-8";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            ViewData["Groups"] = new SelectList(dbContext.UserGroups.ToList(), "GroupID", "Name");
            ViewData["Branches"] = new SelectList(dbContext.Branches.ToList(), "BranchID", "BName");
            return RedirectToAction("Search");
        }

        [AuthorizedRights(Screen = "_users", Right = "Update")]
        public ViewResult Edit(string UserName)
        {
            ApplicationUser appUser = new ApplicationUser();
            appUser = UserManager.FindByName(UserName);
            
            EditViewModel user = new EditViewModel
            {
                Code = appUser.Code,
                OldUserName = appUser.UserName,
                ////BranchID = appUser.BranchId,
                BranchRight = appUser.BranchRight,
                GroupID = appUser.GroupId,
                UserName = appUser.UserName,
                //ExpireDate= appUser.ExpireDate,
                Email = appUser.Email,
                FullName= appUser.FullName,
                Password = "Abcd@123",
                ConfirmPassword= "Abcd@123",
                UnBlockRight=appUser.UnBlockRight
                //CloseDueDate =appUser.CloseDueDate


            };
          
            

            ViewData["Groups"] = new SelectList(dbContext.UserGroups.ToList(), "GroupID", "Name",user.GroupID);
            ViewData["Branches"] = new SelectList(dbContext.Branches.Where(x=>x.DeletFlag != DeleteFlag.Deleted).ToList(), "BranchID", "BName",user.BranchID);
            ViewData["Positions"] = new SelectList(dbContext.Titles.ToList(), "TitleID", "Name",user.PositionID);
            var expiredate = DateTime.Now;
            var ex = expiredate.AddDays(dbContext.UserSecurities.Select(u => u.ExpireInterval).FirstOrDefault());
            ViewData["expiredate"] = ex.ToString("dd/MM/yyyy");
            ViewData["CloseDueDate"] = DateTime.Now.AddYears(10).ToString("dd/MM/yyyy");


            //string relativePath = Server.MapPath("~//Files//pass.txt");
            //string combinedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
            //ViewData["defaultpass"] = System.IO.File.ReadAllText(path: relativePath);
            ViewData["defaultpass"] = "Admin@123";


            return View(user);
        }
        [HttpPost]
        [AuthorizedRights(Screen = "_users", Right = "Update")]
        public async Task<ActionResult> Edit(EditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Edit", new { UserName = model.UserName });
            }
            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var manager = new UserManager<ApplicationUser>(store);
            var currentUser = await UserManager.FindByNameAsync(model.OldUserName);
            var SecondTable = dbContext.User.Where(x => x.Code == currentUser.Code).FirstOrDefault();
            var obj = new UpDated_2005.Common();


            currentUser.UserName = model.UserName;
            currentUser.Code = model.Code;
            currentUser.BranchId = model.BranchID;
            currentUser.GroupId = model.GroupID;
            currentUser.SysDate = DateTime.Now;
            currentUser.FullName = model.FullName;
            currentUser.UserId = dbContext.Users.Where(u => u.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault();
            currentUser.PasswordHash = manager.PasswordHasher.HashPassword(model.Password);
            currentUser.SysDate = DateTime.Now;
            currentUser.ExpireDate = currentUser.ExpireDate;
            currentUser.CloseDueDate = DateTime.Now.AddYears(10);
            currentUser.BranchRight = model.BranchRight;
            currentUser.UnBlockRight = model.UnBlockRight;
            currentUser.defaultHashedPassword = obj.cipher_cls.Encryption(model.Password);
                       
            UserManager.Update(currentUser);
          
            if (SecondTable != null)
            {
                SecondTable.Code = model.Code;
                SecondTable.UserName = model.UserName;
                SecondTable.Password = model.Password;
                SecondTable.BranchID = model.BranchID;
                SecondTable.BranchRight = model.BranchRight;
                SecondTable.GroupID = model.GroupID;
                SecondTable.SysDate = DateTime.Now;
                SecondTable.ExpireDate = currentUser.ExpireDate;
                SecondTable.LockUser = currentUser.IsLocked;
                SecondTable.Admin = currentUser.IsAdmin;
                SecondTable.DeleteFlag = currentUser.DeleteFlag;
                SecondTable.EditFlag = currentUser.EditFlag;
                SecondTable.Position = currentUser.TitleId;
                SecondTable.UserIDForMaker = currentUser.Maker;

                try
                {
                    dbContext.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    Exception raise = dbEx;
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = string.Format("{0}:{1}",
                                validationErrors.Entry.Entity.ToString(),
                                validationError.ErrorMessage);
                            // raise a new exception nesting
                            // the current instance as InnerException
                            raise = new InvalidOperationException(message, raise);
                        }
                    }
                    throw raise;
                }

            }
            //manager.Update(currentUser);
            //var ctx = store.Context;
            //ctx.SaveChanges();
            TempData["msg"] = "Profile Changes Saved !";
            return RedirectToAction("Details", new { UserName = currentUser.UserName });
        }
        public  ActionResult UnlockUser(string Id)
        {
            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var manager = new UserManager<ApplicationUser>(store);
            ApplicationUser appUser = new ApplicationUser();
            appUser = UserManager.FindById(Id);
            var SecondTable = dbContext.User.Where(x => x.Code == appUser.Code).FirstOrDefault();
            if (appUser.IsLocked)
            {
                appUser.IsLocked = false;
                SecondTable.LockUser = false;

            }
            else
            {
                appUser.IsLocked = true;
                SecondTable.LockUser = true;
            }
            
            UserManager.Update(appUser);
            dbContext.SaveChanges();

            return RedirectToAction("Search");
        }

        [AuthorizedRights(Screen = "_users", Right = "Read")]
        public ActionResult Next(string Code)
        {
            var arrlenth = IDs.Count();
            var LastObj = dbContext.Users.OrderBy(s => s.Code).ToList().LastOrDefault(s => s.Code == IDs[arrlenth - 1]);
            if (Code == LastObj.Code)
            {
                TempData["Last"] = "Last";
                return RedirectToAction("Details", new { UserName = LastObj.UserName });
            }
            else
            {
                int counter = 0;
                int next = 0;
                foreach (var item in IDs)
                {

                    if (item == Code)
                    {
                        next = counter + 1;

                    }
                    counter++;
                }
                Code = IDs[next];
                if (Code == LastObj.Code)
                {
                    TempData["Last"] = "Last";
                }
                else
                {
                    TempData["Last"] = "NotLast";
                }
                var UserName = dbContext.Users.Where(u=>u.Code==Code).Select(s=>s.UserName).FirstOrDefault(); 

                return RedirectToAction("Details", new { UserName = UserName });
            }

        }

        [AuthorizedRights(Screen = "_users", Right = "Read")]
        public ActionResult Previous(string Code)
        {
            var arrindex = IDs[0];
            var FirstObj = dbContext.Users.OrderBy(s => s.Code).FirstOrDefault(s => s.Code == arrindex);
            if (Code == FirstObj.Code)
            {
                TempData["First"] = "First";
                return RedirectToAction("Details", new { UserName = FirstObj.UserName });
            }
            else
            {
                int counter = 0;
                int previous = 0;
                foreach (var item in IDs)
                {

                    if (item == Code)
                    {
                        previous = counter - 1;

                    }
                    counter++;
                }
                Code = IDs[previous];
                if (Code == FirstObj.Code)
                {
                    TempData["First"] = "First";
                }
                else
                {
                    TempData["First"] = "NotFirst";
                }
                
                var UserName = dbContext.Users.Where(u => u.Code == Code).Select(s => s.UserName).FirstOrDefault();
                return RedirectToAction("Details", new { UserName = UserName });
            }

        }

       
        [AuthorizedRights(Screen = "_users", Right = "Read")]
        public ViewResult Details(string UserName)
        {
            if (TempData["CanNotDelete"]!=null)
            {
                string[] errors = new string[] { "Can't Delete This User, Plz Unauther it first" };
                IdentityResult result = new IdentityResult(errors);
                AddErrors(result);
            }
            
            ApplicationUser appUser = new ApplicationUser();
            appUser = UserManager.FindByName(UserName);

            var Actualy_auth = appUser.Auth;
            var ThisUser = User.Identity.GetUserId();

            if (appUser.Maker == ThisUser)
            {
                if (appUser.Chk == true && appUser.Auth == false)
                {
                    appUser.Chk = true;
                    appUser.Auth = false;
                }
                if (appUser.Chk == false && appUser.Auth == false)
                {
                    appUser.Chk = true;
                    appUser.Auth = true;

                }
            }
            else
            {
                if (appUser.Chk == true && appUser.Auth == true)
                {
                    appUser.Chk = true;
                    appUser.Auth = true;

                }

                else if (appUser.Chk == true && appUser.Auth == false)
                {
                    if (appUser.Checker != ThisUser)
                    {
                        appUser.Chk = true;
                        appUser.Auth = false;

                    }
                    else
                    {
                        appUser.Chk = true;
                        appUser.Auth = true;

                    }
                }
                else if (appUser.Chk == false && appUser.Auth == false)
                {
                    appUser.Chk = false;
                    appUser.Auth = true;

                }

            }


            if (IDs != null)
            {
                if (appUser.Code == IDs.Last())
                {
                    TempData["Last"] = "Last";
                }
                if (appUser.Code == IDs.First())
                {
                    TempData["First"] = "First";
                }
            }
            else
            {
                TempData["Last"] = "Last";
                TempData["First"] = "First";
            }
            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var manager = new UserManager<ApplicationUser>(store);
            DetailsViewModel model = new DetailsViewModel
            {
                AuthForEditAndDelete = Actualy_auth,
                UserName = appUser.UserName,
                Code = appUser.Code,
                Check = appUser.Chk,
                Auth =appUser.Auth,
                BranchRight = appUser.BranchRight,
                SysDate = appUser.SysDate,
                ExpireDate = appUser.ExpireDate,
                Maker = dbContext.Users.Where(u => u.Id == appUser.Maker).Select(s => s.UserName).FirstOrDefault(),
                LastEditor = dbContext.Users.Where(u=>u.UserId== appUser.UserId).Select(s=>s.UserName).FirstOrDefault(),
                FullName = appUser.FullName,
                Email = appUser.Email,
                Password = "Abc123",
                ConfirmPassword= "Abc123",
                CloseDueDate= appUser.CloseDueDate,
                UnBlockRight =appUser.UnBlockRight

            };
            ViewData["Groups"] = new SelectList(dbContext.UserGroups.ToList(), "GroupID", "Name", appUser.GroupId);
            ViewData["Branches"] = new SelectList(dbContext.Branches.ToList(), "BranchID", "BName", appUser.BranchId);
            ViewData["Positions"] = new SelectList(dbContext.Titles.ToList(), "TitleID", "Name", appUser.TitleId);
            ViewData["ExpireDate"] = appUser.ExpireDate.ToString("dd/MM/yyyy");
            ViewData["CloseDueDate"] = appUser.CloseDueDate.ToString("dd/MM/yyyy");
            return View(model);
        }
        [AuthorizedRights(Screen = "_users", Right = "Delete")]
        public async Task<ActionResult> Delete(string UserName)
        {
            //ApplicationUser appUser = new ApplicationUser();
            //appUser = UserManager.FindByName(UserName);
            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var currentUserName = User.Identity.GetUserName();
            var manager = new UserManager<ApplicationUser>(store);
            var currentUser = manager.FindByName(UserName);
            var SecondTable = dbContext.User.Where(x => x.Code == currentUser.Code).FirstOrDefault();


            if (!currentUser.Auth && currentUserName != UserName )
            {

                currentUser.DeleteFlag = DeleteFlag.Deleted;
                SecondTable.DeleteFlag = DeleteFlag.Deleted;
                var ctx = store.Context;
                ctx.SaveChanges();
                dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                TempData["CanNotDelete"] = true;
                
                return RedirectToAction("Details", new { UserName = currentUser.UserName });

            }

        }

        [AuthorizedRights(Screen = "_users", Right = "Authorized")]
        public ActionResult AuthorizeUser(string UserName)
        {
            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var manager = new UserManager<ApplicationUser>(store);
            var currentUser = manager.FindByName(UserName);
            currentUser.Auth = true;
            currentUser.Auther = User.Identity.GetUserId();
            manager.Update(currentUser);
            var ctx = store.Context;
            ctx.SaveChanges();
           return RedirectToAction("Details",new { UserName = UserName });
        }

        [AuthorizedRights(Screen = "_users", Right = "Check")]
        public ActionResult CheckUser(string UserName)
        {
            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var manager = new UserManager<ApplicationUser>(store);
            var currentUser = manager.FindByName(UserName);
            currentUser.Chk = true;
            currentUser.Checker = User.Identity.GetUserId();
            currentUser.Auther = User.Identity.GetUserId(); //manager.FindByName(User.Identity.Name).ToString();
            manager.Update(currentUser);
            var ctx = store.Context;
            ctx.SaveChanges();
            return RedirectToAction("Details", new { UserName = UserName });

        }
        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        public  void UpdateRights(ApplicationUser user )
        {
            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var manager = new UserManager<ApplicationUser>(store);
            //var User = manager.FindByName(user.UserName);
            var Rights = dbContext.GroupRights
                        .Where(r => r.GroupId == user.GroupId)
                        .ToList();
            HttpContext.Session.Add("Rights", Rights);
        }

        public  void UpdateUserFunds(ApplicationUser user)
        {
            //var UserGroups = dbContext.UserGroups.Where(u => u.GroupID == user.GroupId).FirstOrDefault();
            //var FundRights = dbContext.FundRights.Where(fr => fr.GroupID == UserGroups.GroupID).Select(r => r.FundID).ToList();
            //var UserFunds = dbContext.Funds.Where(f => FundRights.Contains(f.FundID)).ToList();
            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var manager = new UserManager<ApplicationUser>(store);
            //var User = manager.FindByName(user.UserName));
            var FundsOfUser = dbContext.Funds.Where(f => dbContext.FundRights
                    .Where(fr => fr.GroupID == dbContext.UserGroups
                    .Where(g => g.GroupID == user.GroupId).FirstOrDefault().GroupID)
                    .Select(h => h.FundID).ToList().Contains(f.FundID) && f.Auth == true).ToList();

            HttpContext.Session.Add("Funds", FundsOfUser);
        }

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home",new {area="Home" });
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }



            
        }
        #endregion
    }
}