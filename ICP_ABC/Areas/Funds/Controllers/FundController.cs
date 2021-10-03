using ICP_ABC.Areas.Funds.Models;
using ICP_ABC.Areas.UsersSecurity.Models;
using ICP_ABC.Extentions;
using ICP_ABC.Models;
using Microsoft.ApplicationBlocks.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PagedList;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ICP_ABC.Areas.Funds.Controllers
{
    [Authorize]
    public class FundController : Controller
    {
        //private ApplicationSignInManager _signInManager;
        //private ApplicationUserManager _userManager;
        private ApplicationDbContext dbContext = new ApplicationDbContext();
        UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(new ApplicationDbContext());
        static string[] IDs;
        // GET: Funds/Fund
        public ActionResult Index()
        {
             //ViewData["Currencies"] = new SelectList(dbContext.Currencies.Where(c => c.Auth).ToList(), "CurrencyID", "ShortName");
            //ViewData["CustTypes"] = new SelectList(dbContext.CustTypes.Where(c => c.Auth).ToList(), "CustTypeID", "Name");
            //ViewData["Sposors"] = new SelectList(dbContext.Sponsors.Where(c => c.Auth).ToList(), "SponsorID", "Name");
            ViewData["FundDays"] = dbContext.Day.ToList();

            return View();
        }
        public string GetLastCode()
        {
            var querey = "SELECT MAX(CAST(Code AS INT)) as MaxCode FROM fund";
            var appSettings = ConfigurationManager.ConnectionStrings;
            var SqlCon = appSettings["ICPRO"];
            var ReturnedData = SqlHelper.ExecuteDataset(SqlCon.ToString(), CommandType.Text, querey);
            var LastCode = ReturnedData.Tables[0].Rows[0][0].ToString();
            return LastCode;
        }
        [AuthorizedRights(Screen = "FundSetup", Right = "Create")]
        public ActionResult Create()
        {

            //ViewData["FundDaySub"] = dbContext.Day.ToList();
            //ViewData["FundDayRed"] = dbContext.Day.ToList();
            //ViewData["FundDayAuthSub"] = dbContext.Day.ToList();
            //ViewData["FundDayAuthRed"] = dbContext.Day.ToList();

            ViewData["FundDays"] = dbContext.Day.ToList();

            ViewData["Currencies"] = new SelectList(dbContext.Currencies.Where(c => c.Auth).ToList(), "CurrencyID", "ShortName");
            ViewData["CustTypes"] = new SelectList(dbContext.CustTypes.Where(c => c.Auth).ToList(), "CustTypeID", "Name");
            ViewData["Sposors"] = new SelectList(dbContext.Sponsors.Where(c => c.Auth).ToList(), "SponsorID", "Name");

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

        //public ActionResult Create(string Models, string SubL, string RedL, string AuthRedL, string AuthSubL)
        [HttpPost]
        [AuthorizedRights(Screen = "FundSetup", Right = "Create")]
        public ActionResult Create(FundDaysViewModel FundDaysViewModel)
        {
            //var JsonSub = FundDaysViewModel.FundDayForSub;
            //var JsonRed = FundDaysViewModel.FundDayForRed;
            //var JsonAuthRed = FundDaysViewModel.FundAuthDayRed;
            //var JsonAuthSub = FundDaysViewModel.FundAuthDaySub;
            
            CreateFundViewModel Model = new CreateFundViewModel();
            Model = FundDaysViewModel.ThisFund;
           
            if (!ModelState.IsValid)
            {
                //var lastcode = dbContext.Funds.OrderByDescending(s => s.Code).Select(s => s.Code).FirstOrDefault();
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
                return View(Model);
            }

            var isValid = dbContext.Funds.Where(f => f.Code == Model.Code).FirstOrDefault();
           
            if (isValid != null)
            {
                string[] errors = new string[] { "Code Is Already Exist" };
                IdentityResult result = new IdentityResult(errors);
                AddErrors(result);
                return View(Model);
            }

            var Level = dbContext.UserSecurities.Select(Val => Val.Levels).FirstOrDefault();
            if (Level == Levels.TwoLevels)
            {

                Fund fund = new Fund
                {
                    
                    Code = Model.Code,
                    Name = Model.Name,
                    CurrencyID = Model.CurrencyID,
                    SponsorID = Model.SponsorID,
                    ICprice = Model.ICprice,
                    PriceTol = Model.PriceTol,
                    no_ics = Model.no_ics,
                    nomval = Model.nomval,


                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    ParView = 0,
                    Units = 0,
                    SubFeesBar = 0,
                    RedFeesBar = 0,
                    MinInd = 0,
                    MaxInd = 0,
                    MinCor = 0,
                    MaxCor = 0,
                    OtherSubFees = 0,
                    OtherRedFees = 0,
                    AdminFees = 0,
                    EarlyFees = 0,
                    SysDate = DateTime.Now,
                    EntryDate = DateTime.Now,
                    UserID = User.Identity.GetUserId(),
                    EditFlag = false,
                    DeletFlag = DeleteFlag.NotDeleted,
                    Maker = User.Identity.GetUserId(),
                    
                    Checker = User.Identity.GetUserId(),
                    Chk = true,
                    Min_hold_units =0,//minimum fund utilization
                    min_pos = 0,//minimum client holding
                    ceiling = 0,//bank sead capital
                    cper_flag = 0,//unit or amount for minimum fund utilization
                    Auth = false,
                    Auther = null,
                    Timestamp = DateTime.Now,
                    Nav = 0,
                    Remark = "",
                    ISIN = null,
                    InvDate = DateTime.Now,
                    FundAcc = null,
                    MarkFees = 0m,
                    SubFeesAcc = null,
                    RedemFeesAcc = null,
                    ManageFeesAcc = "",
                    AdminFeesAcc = "",
                    OtherSubAcc = null,
                    OtherRedAcc = null,
                    //EarlyFeesAcc=,
                    CustTypeID = 0,
                    ProductEligable = 0,
                    GuaranteeNotePer = "",
                    GuranteeNote = "",
                    CboType = 0,
                    UpFrontFees = 0m,
                    UpFrontAcc = "",
                    FreeText = "",
                    CouponBox = 0,
                    UserLog = "",
                    FUNSDND = 0,
                    Inception_Date = DateTime.Now,
                    fmatlead = DateTime.Now,
                   
                    min_Date = DateTime.Now,
                    
                    SpecialCase = false
                    
                    

                };
                dbContext.Funds.Add(fund);
                dbContext.SaveChanges();
                var LastFundID = dbContext.Funds.OrderByDescending(x => x.FundID).Select(Fi => Fi.FundID).FirstOrDefault();

                //List<FundDay> DaySub = new List<FundDay>();
                //List<FundDay> DayRed = new List<FundDay>();
                //foreach (var item in JsonSub)
                //{
                //    FundDay Fundmodel = new FundDay
                //    {
                //        Day_Id = item.Day_Id,
                //        FundId = LastFundID,
                //        Sub_Red = item.Sub_Red

                //    };
                //    DaySub.Add(Fundmodel);
                //}
                //foreach (var item in JsonRed)
                //{
                //    FundDay Fundmodel1 = new FundDay
                //    {
                //        Day_Id = item.Day_Id,
                //        FundId = LastFundID,
                //        Sub_Red = item.Sub_Red


                //    };
                //    DayRed.Add(Fundmodel1);
                //}

                //List<FundAuthDay> AuthDaySub = new List<FundAuthDay>();
                //List<FundAuthDay> AuthDayRed = new List<FundAuthDay>();
                //foreach (var item in JsonAuthRed)
                //{
                //    FundAuthDay modelAuth1 = new FundAuthDay
                //    {
                //        Day_Id = item.Day_Id,
                //        FundId = LastFundID,
                //        Sub_Red = item.Sub_Red


                //    };
                //    AuthDayRed.Add(modelAuth1);
                //}
                //foreach (var item in JsonAuthSub)
                //{
                //    FundAuthDay modelAuth2 = new FundAuthDay
                //    {
                //        Day_Id = item.Day_Id,
                //        FundId = LastFundID,
                //        Sub_Red = item.Sub_Red


                //    };
                //    AuthDaySub.Add(modelAuth2);
                //}


                //dbContext.FundDay.AddRange(DaySub);
                //dbContext.FundDay.AddRange(DayRed);
                //dbContext.FundAuthDay.AddRange(AuthDayRed);
                //dbContext.FundAuthDay.AddRange(AuthDaySub);


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


                return RedirectToAction("Details", new { Code = fund.Code });
            }
            else
            {
                Fund fund = new Fund
                {

                    Code = Model.Code,
                    Name = Model.Name,
                    CurrencyID = Model.CurrencyID,
                    SponsorID = Model.SponsorID,
                    ICprice = Model.ICprice,
                    PriceTol = Model.PriceTol,
                    no_ics = Model.no_ics,
                    nomval = Model.nomval,

                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    ParView = 0,
                    Units = 0,
                    SubFeesBar = 0,
                    RedFeesBar = 0,
                    MinInd = 0,
                    MaxInd = 0,
                    MinCor = 0,
                    MaxCor = 0,
                    OtherSubFees = 0,
                    OtherRedFees = 0,
                    AdminFees = 0,
                    EarlyFees = 0,
                    SysDate = DateTime.Now,
                    EntryDate = DateTime.Now,
                    UserID = User.Identity.GetUserId(),
                    EditFlag = false,
                    DeletFlag = DeleteFlag.NotDeleted,
                    Maker = User.Identity.GetUserId(),
                    
                    Chk = false,
                    Min_hold_units = 0,//minimum fund utilization
                    min_pos =0,//minimum client holding
                    ceiling = 0,//bank sead capital
                    cper_flag = 0,//unit or amount for minimum fund utilization
                    Checker = null,
                    Auth = false,
                    Auther = null,
                    Timestamp = DateTime.Now,
                    Nav = 0,
                    Remark = "",
                    ISIN =null,
                    InvDate = DateTime.Now,
                    FundAcc = null,
                    MarkFees = 0m,
                    SubFeesAcc = null,
                    RedemFeesAcc = null,
                    ManageFeesAcc = "",
                    AdminFeesAcc = "",
                    OtherSubAcc = null,
                    OtherRedAcc = null,
                    //EarlyFeesAcc=,
                    CustTypeID = 0,
                    ProductEligable = 0,
                    GuaranteeNotePer = "",
                    GuranteeNote = "",
                    CboType = 0,
                    UpFrontFees = 0m,
                    UpFrontAcc = "",
                    FreeText = "",
                    CouponBox = 0,
                    UserLog = "",
                    FUNSDND = 0,
                    Inception_Date = DateTime.Now,
                    fmatlead = DateTime.Now,
                  
                    min_Date = DateTime.Now,
                 
                    SpecialCase = false
                    
                    

                };
                dbContext.Funds.Add(fund);
                dbContext.SaveChanges();
                var LastFundID = dbContext.Funds.OrderByDescending(x => x.FundID).Select(Fi => Fi.FundID).FirstOrDefault();

                //List<FundDay> DaySub = new List<FundDay>();
                //List<FundDay> DayRed = new List<FundDay>();
                //foreach (var item in JsonSub)
                //{
                //    FundDay Fundmodel = new FundDay
                //    {
                //        Day_Id = item.Day_Id,
                //        FundId = LastFundID,
                //        Sub_Red = item.Sub_Red

                //    };
                //    DaySub.Add(Fundmodel);
                //}
                //foreach (var item in JsonRed)
                //{
                //    FundDay Fundmodel1 = new FundDay
                //    {
                //        Day_Id = item.Day_Id,
                //        FundId = LastFundID,
                //        Sub_Red = item.Sub_Red


                //    };
                //    DayRed.Add(Fundmodel1);
                //}

                //List<FundAuthDay> AuthDaySub = new List<FundAuthDay>();
                //List<FundAuthDay> AuthDayRed = new List<FundAuthDay>();
                //foreach (var item in JsonAuthRed)
                //{
                //    FundAuthDay modelAuth1 = new FundAuthDay
                //    {
                //        Day_Id = item.Day_Id,
                //        FundId = LastFundID,
                //        Sub_Red = item.Sub_Red


                //    };
                //    AuthDayRed.Add(modelAuth1);
                //}
                //foreach (var item in JsonAuthSub)
                //{
                //    FundAuthDay modelAuth2 = new FundAuthDay
                //    {
                //        Day_Id = item.Day_Id,
                //        FundId = LastFundID,
                //        Sub_Red = item.Sub_Red


                //    };
                //    AuthDaySub.Add(modelAuth2);
                //}


                //dbContext.FundDay.AddRange(DaySub);
                //dbContext.FundDay.AddRange(DayRed);
                //dbContext.FundAuthDay.AddRange(AuthDayRed);
                //dbContext.FundAuthDay.AddRange(AuthDaySub);


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


                return RedirectToAction("Details", new { Code = fund.Code });
            }


          
          
        }
        [AuthorizedRights(Screen = "FundSetup", Right = "Update")]
        public ActionResult Edit(string Code)
        {
            var Model = dbContext.Funds.Where(f => f.Code == Code).FirstOrDefault();
            ViewData["Currencies"] = new SelectList(dbContext.Currencies.ToList(), "CurrencyID", "ShortName");
            ViewData["Sposors"] = new SelectList(dbContext.Sponsors.ToList(), "SponsorID", "Name");
            EditFundViewModel fund = new EditFundViewModel
            {
                Code = Model.Code,
                Name = Model.Name,
                CurrencyID = Model.CurrencyID,
                SponsorID = Model.SponsorID,
                StartDate = Model.StartDate,
                EndDate = Model.EndDate,
                //ParView = 0,
                //Units = 0,
                SubFeesBar = Model.SubFeesBar,
                RedFeesBar = Model.RedFeesBar,
                MinInd = Model.MinInd,
                MaxInd = Model.MaxInd,
                MinCor = Model.MinCor,
                MaxCor = Model.MaxCor,
                OtherSubFees = Model.OtherSubFees,
                OtherRedFees = Model.OtherRedFees,
                //AdminFees = 0,
                //EarlyFees = 0,
                //SysDate = DateTime.Now,
                //EntryDate = DateTime.Now,
                //UserID = User.Identity.GetUserId(),
                //EditFlag = false,
                //DeletFlag = DeleteFlag.NotDeleted,
                //Maker = User.Identity.GetUserId(),
                nomval = Model.nomval,
                //Chk = false,
                Min_hold_units = Model.Min_hold_units,//minimum fund utilization
                min_pos = Model.min_pos,//minimum client holding
                ceiling = Model.ceiling,//bank sead capital
                cper_flag = Model.cper_flag,//unit or amount for minimum fund utilization
                Checker = null,
                Auth = false,
                Auther = null,
                //Timestamp = DateTime.Now,
                //Nav = 0,
                //Remark = "",
                ISIN = null,
                // = DateTime.Now,
                FundAcc = Model.FundAcc,
                //MarkFees = 0m,
                SubFeesAcc = Model.SubFeesAcc,
                RedemFeesAcc = Model.RedemFeesAcc,
                //ManageFeesAcc = "",
                //AdminFeesAcc = "",
                OtherSubAcc = Model.OtherSubAcc,
                OtherRedAcc = Model.OtherRedAcc,
                //EarlyFeesAcc=,
                CustTypeID = 0,
                // ProductEligable = 0,
                //GuaranteeNotePer = "",
                //GuranteeNote = "",
                CboType = Model.CboType,
                //UpFrontFees = 0m,
                //UpFrontAcc = "",
                //FreeText = "",
                //CouponBox = 0,
                //UserLog = "",
                FUNSDND = Model.FUNSDND,
                //Inception_Date = DateTime.Now,
                //fmatlead = DateTime.Now,
                ICprice = Model.ICprice,
                //min_Date = DateTime.Now,
                PriceTol = Model.PriceTol,
                no_ics = Model.no_ics,
                SpecialCase = Model.SpecialCase
            };
            ViewData["Currencies"] = new SelectList(dbContext.Currencies.ToList(), "CurrencyID", "ShortName", fund.CurrencyID);
            ViewData["CustTypes"] = new SelectList(dbContext.CustTypes.ToList(), "CustTypeID", "Name", fund.CustTypeID);
            ViewData["Sposors"] = new SelectList(dbContext.Sponsors.ToList(), "SponsorID", "Name", fund.SponsorID);
            ViewData["Days"] = dbContext.Day.ToList();
            var fundID = dbContext.Funds.Where(x => x.Code == Code).Select(x => x.FundID).FirstOrDefault();
            ViewData["FundAuthDays_Sub"] = dbContext.FundAuthDay.Where(x => x.FundId == fundID && x.Sub_Red == 0).ToList();
            ViewData["FundAuthDays_Red"] = dbContext.FundAuthDay.Where(x => x.FundId == fundID && x.Sub_Red == 1).ToList();
            ViewData["FundDays_sub"] = dbContext.FundDay.Where(x => x.FundId == fundID && x.Sub_Red == 0).ToList();
            ViewData["FundDays_Red"] = dbContext.FundDay.Where(x => x.FundId == fundID && x.Sub_Red == 1).ToList();
            return View(fund);
        }
        [HttpPost]
        [AuthorizedRights(Screen = "FundSetup", Right = "Update")]
        public ActionResult Edit(FundDaysViewModel FundDaysViewModel)
        {

            var fund = dbContext.Funds.Where(f => f.Code == FundDaysViewModel.ThisFund.Code).FirstOrDefault();

            CreateFundViewModel Model = new CreateFundViewModel();
            Model = FundDaysViewModel.ThisFund;

            //----------

            //var x = context.Projects.Where(p => p.ProjectId == projectId)
            //   .ToList().ForEach(p => context.Projects.Remove(p));

            //----------
          
            fund.Code = FundDaysViewModel.ThisFund.Code;
            fund.SpecialCase = false;
            fund.Name = FundDaysViewModel.ThisFund.Name;
            fund.CurrencyID = FundDaysViewModel.ThisFund.CurrencyID;
            fund.SponsorID = FundDaysViewModel.ThisFund.SponsorID;
            //StartDate = DateTime.Now;
            //EndDate = DateTime.Now.AddYears(50) ;
            //ParView = 0;
            //Units = 0;
            fund.SubFeesBar = 0;
            fund.RedFeesBar = 0;
            fund.MinInd = 0;
            fund.MaxInd = 0;
            fund.MinCor = 0;
            fund.MaxCor = 0;
            fund.OtherSubFees = 0;
            fund.OtherRedFees = 0;
            //AdminFees= 0 ;
            //EarlyFees=0 ;
            //SysDate = DateTime.Now;
            //EntryDate= DateTime.Now;
            //UserID= User.Identity.GetUserId();
            //EditFlag = false;
            //DeletFlag= false;
            //Maker = User.Identity.GetUserId();
            //Check = false;
            //Checker= null;
            //Auth = false;
            //Auther = null;
            //Timestamp= DateTime.Now;
            //Nav=0;
            //Remark="";
            fund.ISIN = null;
            fund.no_ics = FundDaysViewModel.ThisFund.no_ics;
            fund.EditFlag = true;
            //InvDate = DateTime.Now;
            fund.FundAcc = null;
            //MarkFees= 0m;
            fund.SubFeesAcc = null;
            fund.RedemFeesAcc = null;
            // ManageFeesAcc="";
            //AdminFeesAcc="";
            fund.OtherSubAcc = null;
            fund.OtherRedAcc = null;
            //EarlyFeesAcc=;
            fund.CustTypeID = 0;
            //ProductEligable=0;
            //GuaranteeNotePer="";
            //GuranteeNote="";
            fund.CboType = 0;
            fund.ICprice = FundDaysViewModel.ThisFund.ICprice;
            fund.PriceTol = FundDaysViewModel.ThisFund.PriceTol;
            dbContext.SaveChanges();

            //UpFrontFees =0m;
            //UpFrontAcc="";
            //FreeText="";
            //CouponBox=0;
            //UserLog="" 

            // FundDay&FundAuthDay

            //var FundID = dbContext.Funds.Where(Fi => Fi.Code == FundDaysViewModel.ThisFund.Code).Select(fi => fi.FundID).FirstOrDefault();
            //var RangeForFundDay = dbContext.FundDay.Where(Fi => Fi.FundId == FundID).ToList();
            //var RangeForFundAuthDay = dbContext.FundAuthDay.Where(Fi => Fi.FundId == FundID).ToList();
            //dbContext.FundDay.RemoveRange(RangeForFundDay);
            //dbContext.FundAuthDay.RemoveRange(RangeForFundAuthDay);
            dbContext.SaveChanges();

            //var JsonSub = FundDaysViewModel.FundDayForSub;
            //var JsonRed = FundDaysViewModel.FundDayForRed;
            //var JsonAuthRed = FundDaysViewModel.FundAuthDayRed;
            //var JsonAuthSub = FundDaysViewModel.FundAuthDaySub;

            //var LastFundID = dbContext.Funds.OrderByDescending(x => x.FundID).Select(Fi => Fi.FundID).FirstOrDefault();

            //var LastFundID = FundID;
            //List<FundDay> DaySub = new List<FundDay>();
            //List<FundDay> DayRed = new List<FundDay>();
            //foreach (var item in JsonSub)
            //{
            //    FundDay Fundmodel = new FundDay
            //    {
            //        Day_Id = item.Day_Id,
            //        FundId = LastFundID,
            //        Sub_Red = item.Sub_Red

            //    };
            //    DaySub.Add(Fundmodel);
            //}
            //foreach (var item in JsonRed)
            //{
            //    FundDay Fundmodel1 = new FundDay
            //    {
            //        Day_Id = item.Day_Id,
            //        FundId = LastFundID,
            //        Sub_Red = item.Sub_Red


            //    };
            //    DayRed.Add(Fundmodel1);
            //}

            //List<FundAuthDay> AuthDaySub = new List<FundAuthDay>();
            //List<FundAuthDay> AuthDayRed = new List<FundAuthDay>();
            //foreach (var item in JsonAuthRed)
            //{
            //    FundAuthDay modelAuth1 = new FundAuthDay
            //    {
            //        Day_Id = item.Day_Id,
            //        FundId = LastFundID,
            //        Sub_Red = item.Sub_Red


            //    };
            //    AuthDayRed.Add(modelAuth1);
            //}
            //foreach (var item in JsonAuthSub)
            //{
            //    FundAuthDay modelAuth2 = new FundAuthDay
            //    {
            //        Day_Id = item.Day_Id,
            //        FundId = LastFundID,
            //        Sub_Red = item.Sub_Red


            //    };
            //    AuthDaySub.Add(modelAuth2);
            //}


            //dbContext.FundDay.AddRange(DaySub);
            //dbContext.FundDay.AddRange(DayRed);
            //dbContext.FundAuthDay.AddRange(AuthDayRed);
            //dbContext.FundAuthDay.AddRange(AuthDaySub);


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


            return RedirectToAction("Details", new { Code = fund.Code });

        }

        [AuthorizedRights(Screen = "FundSetup", Right = "Read")]
        public ActionResult Details(string Code)
        {
            var Model = dbContext.Funds.Where(f => f.Code == Code).FirstOrDefault();
            ViewData["CustTypes"] = new SelectList(dbContext.CustTypes.ToList(), "CustTypeID", "Name");
            var Actualy_auth = Model.Auth;
            var ThisUser = User.Identity.GetUserId();
            ViewData["Days"] = dbContext.Day.ToList();
            //var fundID = Convert.ToInt32(Code);
            var fundID = dbContext.Funds.Where(x => x.Code == Code).Select(x=>x.FundID).FirstOrDefault();

            ViewData["FundAuthDays_Sub"] = dbContext.FundAuthDay.Where(x => x.FundId == fundID && x.Sub_Red == 0).ToList();
            ViewData["FundAuthDays_Red"] = dbContext.FundAuthDay.Where(x => x.FundId == fundID && x.Sub_Red == 1).ToList();
            ViewData["FundDays_sub"] = dbContext.FundDay.Where(x => x.FundId == fundID && x.Sub_Red == 0).ToList();
            ViewData["FundDays_Red"] = dbContext.FundDay.Where(x => x.FundId == fundID && x.Sub_Red == 1).ToList();

            if (Model.Maker == ThisUser)
            {
                if (Model.Chk == true && Model.Auth == false)
                {
                    Model.Chk = true;
                    Model.Auth = false;
                }
                if (Model.Chk == false && Model.Auth == false)
                {
                    Model.Chk = true;
                    Model.Auth = true;

                }
            }
            else
            {
                if (Model.Chk == true && Model.Auth == true)
                {
                    Model.Chk = true;
                    Model.Auth = true;

                }

                else if (Model.Chk == true && Model.Auth == false)
                {
                    if (Model.Checker != ThisUser)
                    {
                        Model.Chk = true;
                        Model.Auth = false;

                    }
                    else
                    {
                        Model.Chk = true;
                        Model.Auth = true;

                    }
                }
                else if (Model.Chk == false && Model.Auth == false)
                {
                    Model.Chk = false;
                    Model.Auth = true;

                }

            }


            DetailsFundViewModel fund = new DetailsFundViewModel
            {
                Code = Model.Code,
                Name = Model.Name,
                CurrencyID = Model.CurrencyID,
                SponsorID = Model.SponsorID,
                StartDate = Model.StartDate,
                EndDate = Model.EndDate,
                //ParView = 0,
                //Units = 0,
                SubFeesBar = Model.SubFeesBar,
                RedFeesBar = Model.RedFeesBar,
                MinInd = Model.MinInd,
                MaxInd = Model.MaxInd,
                MinCor = Model.MinCor,
                MaxCor = Model.MaxCor,
                OtherSubFees = Model.OtherSubFees,
                OtherRedFees = Model.OtherRedFees,
                //AdminFees = 0,
                //EarlyFees = 0,
                //SysDate = DateTime.Now,
                //EntryDate = DateTime.Now,
                //UserID = User.Identity.GetUserId(),
                //EditFlag = false,
                //DeletFlag = DeleteFlag.NotDeleted,
                //Maker = User.Identity.GetUserId(),
                nomval = Model.nomval,
                //Chk = false,
                Min_hold_units = Model.Min_hold_units,//minimum fund utilization
                min_pos = Model.min_pos,//minimum client holding
                ceiling = Model.ceiling,//bank sead capital
                cper_flag = Model.cper_flag,//unit or amount for minimum fund utilization
                Checker = null,
                Auth = false,
                Auther = null,
                //Timestamp = DateTime.Now,
                //Nav = 0,
                //Remark = "",
                ISIN = "",
                // = DateTime.Now,
                FundAcc = Model.FundAcc,
                //MarkFees = 0m,
                SubFeesAcc = Model.SubFeesAcc,
                RedemFeesAcc = Model.RedemFeesAcc,
                //ManageFeesAcc = "",
                //AdminFeesAcc = "",
                OtherSubAcc = Model.OtherSubAcc,
                OtherRedAcc = Model.OtherRedAcc,
                //EarlyFeesAcc=,
                CustTypeID = 0,
                // ProductEligable = 0,
                //GuaranteeNotePer = "",
                //GuranteeNote = "",
                CboType = Model.CboType,
                //UpFrontFees = 0m,
                //UpFrontAcc = "",
                //FreeText = "",
                //CouponBox = 0,
                //UserLog = "",
                FUNSDND = Model.FUNSDND,
                //Inception_Date = DateTime.Now,
                //fmatlead = DateTime.Now,
                ICprice = Model.ICprice,
                //min_Date = DateTime.Now,
                PriceTol = Model.PriceTol,
                no_ics = Model.no_ics,
                SpecialCase = Model.SpecialCase
            };
            if (IDs != null)
            {
                if (Model.Code == IDs.Last())
                {
                    TempData["Last"] = "Last";
                }
                if (Model.Code == IDs.First())
                {
                    TempData["First"] = "First";
                }
            }
            else
            {
                TempData["Last"] = "Last";
                TempData["First"] = "First";
            }

            return View(fund);
        }

        [AuthorizedRights(Screen = "FundSetup", Right = "Read")]
        public ViewResult Search(string sortOrder, string currentFilter, string searchString, int? page, string Code, string RadioCHeck)
        {

            int seculevel = (int)dbContext.UserSecurities.FirstOrDefault().Levels;
            ViewBag.SecuLevel = seculevel;
            if (page == null && sortOrder == null && currentFilter == null && searchString == null && Code == null && RadioCHeck == null)
            {
                int pageSize = 4;
                int pageNumber = (page ?? 1);
                var funds = dbContext.Funds.Where(c => c.DeletFlag == DeleteFlag.NotDeleted).Take(0);

                return View(funds.ToPagedList(pageNumber, pageSize));

            }
            else
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                ViewBag.DateSortParm = sortOrder == "Code" ? "code_desc" : "Code";

                //if (searchString != null)
                //{
                //    page = 1;
                //}
                //else
                //{
                //    searchString = currentFilter;
                //}

                ViewBag.CurrentFilter = searchString;

                var funds = dbContext.Funds.Where(c => c.DeletFlag == DeleteFlag.NotDeleted);
                if (!String.IsNullOrEmpty(searchString))
                {
                    funds = funds.Where(s => s.Name.Contains(searchString));
                    ViewData["searchString"] = searchString;
                }

                //checkRadio 
                if (!String.IsNullOrEmpty(RadioCHeck))
                {
                    //Case Auth
                    if (RadioCHeck == "1")
                    {
                        funds = funds.Where(s => s.Auth == true);

                    }
                    //Case Checker
                    if (RadioCHeck == "2")
                    {
                        funds = funds.Where(s => s.Chk == true && s.Auth == false);

                    }
                    //Case maker
                    if (RadioCHeck == "3")
                    {
                        funds = funds.Where(s => s.Auth == false && s.Chk == false);

                    }
                    //Case All
                    if (RadioCHeck == "4")
                    {
                        funds = funds.Where(c => c.DeletFlag == DeleteFlag.NotDeleted);

                    }


                }
                //

                if (!String.IsNullOrEmpty(Code))
                {
                    funds = funds.Where(s => s.Code.StartsWith(Code));
                    ViewData["Code"] = Code;
                }

                TempData["FundForExc"] = funds.Select(x => x.FundID).ToList();

                switch (sortOrder)
                {
                    case "name_desc":
                        funds = funds.OrderByDescending(s => s.Name);
                        break;
                    case "Code":
                        funds = funds.OrderBy(s => s.Code);
                        break;
                    case "code_desc":
                        funds = funds.OrderByDescending(s => s.Code);
                        break;
                    default:  // Name ascending 
                        //funds = funds.OrderBy(s => s.Name);
                        funds = funds.OrderBy(s => s.FundID);
                        break;
                }
                //int count = cities.ToList().Count();
                //IDs = new string[count];
                //IDs = cities.Select(s => s.Code).ToArray();

                int pageSize = 4;
                int pageNumber = (page ?? 1);
                ViewData["radioCHeck"] = RadioCHeck;
                int count = funds.ToList().Count();
                IDs = new string[count];
                IDs = funds.Select(s => s.Code).ToArray();

                int[] myInts = Array.ConvertAll(IDs, s => int.Parse(s));
                Array.Sort(myInts);
                IDs = myInts.Select(x => x.ToString()).ToArray();
                //int []IDsINT= Convert.ToInt32(IDs);
                return View(funds.ToPagedList(pageNumber, pageSize));
            }
        }

        [AuthorizedRights(Screen = "FundSetup", Right = "Read")]
        public ActionResult Next(string Code)
        {
            var arrlenth = IDs.Count();
            var LastObj = dbContext.Funds.OrderBy(s => s.Code).ToList().LastOrDefault(s => s.Code == IDs[arrlenth - 1]);
            if (Code == LastObj.Code)
            {
                TempData["Last"] = "Last";
                return RedirectToAction("Details", new { Code = LastObj.Code });
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
                var code = dbContext.Funds.Where(u => u.Code == Code).Select(s => s.Code).FirstOrDefault();


                if (Code == LastObj.Code)
                {
                    TempData["Last"] = "Last";
                }
                else
                {
                    TempData["Last"] = "NotLast";
                }

                return RedirectToAction("Details", new { Code = code });
            }

        }

        [AuthorizedRights(Screen = "FundSetup", Right = "Read")]
        public ActionResult Previous(string Code)
        {
            var arrindex = IDs[0];
            var FirstObj = dbContext.Funds.OrderBy(s => s.Code).FirstOrDefault(s => s.Code == arrindex);
            if (Code == FirstObj.Code)
            {
                TempData["First"] = "First";
                return RedirectToAction("Details", new { Code = FirstObj.Code });
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
                var code = dbContext.Funds.Where(u => u.Code == Code).Select(s => s.Code).FirstOrDefault();
                return RedirectToAction("Details", new { Code = code });
            }

        }

        [AuthorizedRights(Screen = "FundSetup", Right = "Read")]
        public ActionResult ExportToExcel()
        {
            string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");
            var gv = new GridView();
            var List = (List<int>)TempData["FundForExc"];
            gv.DataSource = dbContext.Funds.Where(Del => List.Contains(Del.FundID) && Del.DeletFlag == DeleteFlag.NotDeleted).Select(x => new { x.Code, x.Name, x.ISIN }).ToList();
            //gv.DataSource = (from s in dbContext.Funds
            //                 select new
            //                 {
            //                     s.Code,
            //                     s.Name


            //                 })
            //                     .ToList();
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Fund" + NowTime + ".xls");
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
        [AuthorizedRights(Screen = "FundSetup", Right = "Read")]
        public ActionResult ExportToPDF()
        {
            var Funds = dbContext.Funds.Select(s => s).Where(s => IDs.Contains(s.Code)).OrderBy(u => u.Name).ToList();
            string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");

            var report = new PartialViewAsPdf("~/Areas/Funds/Views/Fund/ExportToPDF.cshtml", Funds);
            return report;

        }

        [AuthorizedRights(Screen = "FundSetup", Right = "Authorized")]
        public ActionResult AuthorizeFund(string Code)
        {
            var fund = dbContext.Funds.Where(f => f.Code == Code).FirstOrDefault();
            fund.Auth = true;
            fund.Auther = User.Identity.GetUserId();
            dbContext.SaveChanges();
            return RedirectToAction("Details", new { Code = Code });
        }

        [AuthorizedRights(Screen = "FundSetup", Right = "Check")]
        public ActionResult CheckFund(string Code)
        {
            var fund = dbContext.Funds.Where(f => f.Code == Code).FirstOrDefault();
            fund.Chk = true;
            fund.Checker = User.Identity.GetUserId();
            dbContext.SaveChanges();
            return RedirectToAction("Details", new { Code = Code });

        }

        [AuthorizedRights(Screen = "FundSetup", Right = "Delete")]
        public ActionResult Delete(string Code)
        {
            var fund = dbContext.Funds.Where(f => f.Code == Code).FirstOrDefault();
            fund.DeletFlag = DeleteFlag.Deleted;
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

    }
}