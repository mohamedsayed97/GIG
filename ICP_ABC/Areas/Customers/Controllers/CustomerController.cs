using ICP_ABC.Areas.Clients.Models;
using ICP_ABC.Areas.Customers.Models;
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
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ICP_ABC.Areas.Customers.Controllers
{
    [Authorize]
    [RoutePrefix("Employees")]
    public class CustomerController : Controller
    {
        private ApplicationDbContext dbContext = new ApplicationDbContext();
        UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(new ApplicationDbContext());
        static string[] IDs;

        // GET: Customers/Customer
        [Route("Index")]
        public ActionResult Index()
        {

            return View("~/Areas/Customers/Views/Customer/Index.cshtml");
        }
        //GetLastCode
        public string GetLastCode()
        {
            var querey = "SELECT MAX(CAST(Code AS INT)) as MaxCode FROM Customer";
            var appSettings = ConfigurationManager.ConnectionStrings;
            var SqlCon = appSettings["ICPRO"];
            var ReturnedData = SqlHelper.ExecuteDataset(SqlCon.ToString(), CommandType.Text, querey);
            var LastCode = ReturnedData.Tables[0].Rows[0][0].ToString();
            return LastCode;
        }

        [Route("Create")]
        [AuthorizedRights(Screen = "Customers", Right = "Create")]
        public ActionResult Create()
        {
          
            ViewData["Cities"] = new SelectList(dbContext.Cities.ToList(), "CityID", "Name");
            ViewData["Nationalities"] = new SelectList(dbContext.Nationalities.ToList(), "NationalityID", "Name");
           

            return View("~/Areas/Customers/Views/Customer/Create.cshtml");
           
        }
        [Route("Create")]
        [AuthorizedRights(Screen = "Customers", Right = "Create")]
        [HttpPost]
        public ActionResult Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var isValid = dbContext.Customers.Where(f => f.CustomerID == model.CustomerID).FirstOrDefault();
                if (isValid != null)
                {
                    string[] errors = new string[] { "Employee code is already taken" };
                    IdentityResult result = new IdentityResult(errors);
                    AddErrors(result);
                    return View("~/Areas/Customers/Views/Customer/Create.cshtml", model);
                }


                var Level = dbContext.UserSecurities.Select(Val => Val.Levels).FirstOrDefault();
                if (Level == Levels.TwoLevels)
                {
                    Customer customer = new Customer
                    {
                        CustomerID = model.CustomerID,

                        ArAddress1 = model.ArAddress1,
                        ArAddress2 = model.ArAddress2,

                        EnAddress1 = model.EnAddress1,
                        EnAddress2 = model.EnAddress2,

                        ArName = model.ArName,
                        EnName = model.EnName,

                       

                        Auth = false,
                        Chk = true,
                        Maker = User.Identity.GetUserId(),
                        Checker = User.Identity.GetUserId(),
                      
                        CityId = model.CityId.Value,
                    
                     
                   
                        Email1 = model.Email1,
                  
                      
                        
                        NationalityId = model.NationalityId.Value,
                    
                        IdNumber = model.IdNumber,
                     
                        SysDate = DateTime.Now,
                      
                  
                        tel1 = model.tel1,
                       
                     
                        DateOfContribute = model.DateOfContribute,
                        DateOfHiring = model.DateOfHiring, 
                        DateOfBirth = model.DateOfBirth,
                        Salary = model.Salary
                       


                    };
                    dbContext.Customers.Add(customer);

                    try
                    {
                        dbContext.SaveChanges();

                        var Customer = dbContext.Customers.Where(x=>x.DeletFlag==DeleteFlag.NotDeleted).ToList();
                        int count = Customer.Count();
                        IDs = new string[count];
                        IDs = Customer.Select(s => s.CustomerID).ToArray();
                        int[] myInts = Array.ConvertAll(IDs, s => int.Parse(s));
                        Array.Sort(myInts);
                        IDs = myInts.Select(z => z.ToString()).ToArray();
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
                        throw;
                    }
                    return RedirectToAction("Details", new { Id = model.CustomerID });
                }
                else
                {
                    Customer customer = new Customer
                    {
                        CustomerID = model.CustomerID,

                        ArAddress1 = model.ArAddress1,
                        ArAddress2 = model.ArAddress2,

                        EnAddress1 = model.EnAddress1,
                        EnAddress2 = model.EnAddress2,

                        ArName = model.ArName,
                        EnName = model.EnName,


                        Auth = false,
                        Chk = false,
                        Maker = User.Identity.GetUserId(),
                        Checker = null,
                     
                  //      CityId = model.CityId.Value,
                     
                        Email1 = model.Email1,


               //         NationalityId = model.NationalityId.Value,
                      
                        SysDate = DateTime.Now,
                    
                        tel1 = model.tel1,
                     
                        DateOfContribute = model.DateOfContribute,
                        DateOfHiring = model.DateOfHiring,
                        DateOfBirth = model.DateOfBirth,
                        Salary = model.Salary



                    };
                    dbContext.Customers.Add(customer);

                    try
                    {
                        dbContext.SaveChanges();
                        var Customer = dbContext.Customers.Where(x => x.DeletFlag == DeleteFlag.NotDeleted).ToList();
                        int count = Customer.Count();
                        IDs = new string[count];
                        IDs = Customer.Select(s => s.CustomerID).ToArray();
                        int[] myInts = Array.ConvertAll(IDs, s => int.Parse(s));
                        Array.Sort(myInts);
                        IDs = myInts.Select(z => z.ToString()).ToArray();
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
                        throw;
                    }
                    return RedirectToAction("Details", new { Id = model.CustomerID });
                }

               
            }
            else
            {
                return View("~/Areas/Customers/Views/Customer/Create.cshtml", model);
            }

           
        }
        [Route("Edit")]
        [AuthorizedRights(Screen = "Customers", Right = "Update")]
        public ActionResult Edit(string CustomerID)
        {
            ViewData["Cities"] = new SelectList(dbContext.Cities.ToList(), "CityID", "Name");
            ViewData["Nationalities"] = new SelectList(dbContext.Nationalities.ToList(), "NationalityID", "Name");
           

            var model = dbContext.Customers.Where(c => c.CustomerID == CustomerID).FirstOrDefault();
            CreateViewModel Customer = new CreateViewModel
            {
               
               

                

              



             

                Auth = model.Auth,
               
           
                //--------
                CustomerID = model.CustomerID,

                ArAddress1 = model.ArAddress1,
              
                EnAddress1 = model.EnAddress1,
              

                ArName = model.ArName,
                EnName = model.EnName,

             
                CityId = model.CityId,
              
              
                Email1 = model.Email1,
               
               
             Salary =model.Salary,

                NationalityId = model.NationalityId,
              
                IdNumber = model.IdNumber,
            
                tel1 = model.tel1,
                
                DateOfContribute = model.DateOfContribute,
                DateOfHiring = model.DateOfHiring
            };
            return View("~/Areas/Customers/Views/Customer/Edit.cshtml", Customer);
        }
        [Route("Edit")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [AuthorizedRights(Screen = "Customers", Right = "Update")]
        public ActionResult Edit(CreateViewModel model)
        {
            var Customer = dbContext.Customers.Where(c => c.CustomerID == model.CustomerID).First();
            if (Customer != null)
            {
                Customer.CustomerID = model.CustomerID;

                Customer.ArAddress1 = model.ArAddress1;
                Customer.ArAddress2 = model.ArAddress2;

                Customer.ArName = model.ArName;

           
              //  Customer.CityId = model.CityId.Value;
              
         
           
                Customer.Email1 = model.Email1;
               
                Customer.EnAddress1 = model.EnAddress1;
              
                Customer.EnName = model.EnName;
           //     Customer.NationalityId = model.NationalityId.Value;
              
              
                Customer.DateOfHiring = model.DateOfHiring;
                Customer.DateOfContribute = model.DateOfContribute;

                Customer.IdNumber = model.IdNumber;
                
                Customer.tel1 = model.tel1;
             
              
                Customer.EditFlag = true;
                Customer.UserID = User.Identity.GetUserId();
                dbContext.SaveChanges();

            }
            return RedirectToAction("Details",new { Id = model.CustomerID });
        }
        [Route("Search")]
        [AuthorizedRights(Screen = "Customers", Right = "Read")]
        public ViewResult Search(string sortOrder,string RadioCHeck, string currentFilter,string searchString,  int? page, string Code, string ArName, string EnName,string CustomerType,string IdNum, string BranchId )
        {
            int seculevel = (int)dbContext.UserSecurities.FirstOrDefault().Levels;
            ViewBag.SecuLevel = seculevel;
            //var LogUser = User.Identity.GetUserId();
            //var thisUser = dbContext.Users.Where(x => x.Id == LogUser && x.BranchRight == true).FirstOrDefault();

            if (page==null&& Code == null  && RadioCHeck == null  && ArName == null  && EnName == null  && BranchId == null && IdNum == null && CustomerType == null )
            {
                int pageSize = 4;
                int pageNumber = (page ?? 1);
                ViewData["CustType"] = new SelectList(dbContext.CustTypes.ToList(), "CustTypeID", "Name");
                ViewData["Branches"] = new SelectList(dbContext.Branches.ToList(), "BranchID", "BName");
                var Customers = dbContext.Customers.Where(c => c.DeletFlag == DeleteFlag.NotDeleted).Take(0);

                return View("~/Areas/Customers/Views/Customer/Search.cshtml", Customers.ToPagedList(pageNumber, pageSize));

            }
            
            else
            {



                ViewData["CustType"] = new SelectList(dbContext.CustTypes.ToList(), "CustTypeID", "Name");
                ViewData["Branches"] = new SelectList(dbContext.Branches.ToList(), "BranchID", "BName");
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

                var Customers = dbContext.Customers.Where(c => c.DeletFlag == DeleteFlag.NotDeleted);
                var currentuserId = User.Identity.GetUserId();
                var currentUser = dbContext.Users.Where(u => u.Id == currentuserId).First();

                if (!currentUser.BranchRight)

                {
                    Customers = Customers.Where(u => u.BranchId == currentUser.BranchId);
                }
                if (!String.IsNullOrEmpty(searchString))
                {
                    Customers = Customers.Where(s => s.EnName.Contains(searchString));
                }


                if (!String.IsNullOrEmpty(Code))
                {
                    //var code = int.Parse(Code);
                    Customers = Customers.Where(s => s.CustomerID == Code);
                    ViewData["Code"] = Code;
                }
                var dfg = Customers.ToList();
                if (!String.IsNullOrEmpty(EnName))
                {
                    Customers = Customers
                   .Where(u => u.EnName.Contains(EnName))
                   .Select(s => s);
                    ViewData["EnName"] = EnName;
                }

                if (!String.IsNullOrEmpty(ArName))
                {
                    Customers = Customers
                   .Where(u => u.ArName.Contains(ArName))
                   .Select(s => s);
                    ViewData["ArName"] = ArName;

                }

                if (!String.IsNullOrEmpty(CustomerType))
                {
                    int cutType;
                    cutType = int.Parse(CustomerType);
                    Customers = Customers
                   .Where(u => u.CustTypeId == cutType)
                   .Select(s => s);
                    ViewData["CustType"] = new SelectList(dbContext.CustTypes.ToList(), "CustTypeID", "Name", cutType);


                }
                if (!String.IsNullOrEmpty(IdNum))
                {
                    // int IdNumber;
                    // IdNumber = int.Parse(IdNum);
                    Customers = Customers
                   .Where(u => u.IdNumber == IdNum)
                   .Select(s => s);
                    ViewData["IdNum"] = IdNum;

                }

                if (!String.IsNullOrEmpty(BranchId))
                {
                    int BranchID;
                    BranchID = int.Parse(BranchId);
                    Customers = Customers
                   .Where(u => u.BranchId == BranchID)
                   .Select(s => s);
                    ViewData["Branches"] = new SelectList(dbContext.Branches.ToList(), "BranchID", "BName", BranchID);

                }

                //checkRadio 
                if (!String.IsNullOrEmpty(RadioCHeck))
                {
                    //Case Auth
                    if (RadioCHeck == "1")
                    {
                        Customers = Customers.Where(s => s.Auth == true);

                    }
                    //Case Checker
                    if (RadioCHeck == "2")
                    {
                        Customers = Customers.Where(s => s.Chk == true && s.Auth == false);

                    }
                    //Case maker
                    if (RadioCHeck == "3")
                    {
                        Customers = Customers.Where(s => s.Auth == false && s.Chk == false);

                    }
                    //Case All
                    if (RadioCHeck == "4")
                    {
                        Customers = Customers.Where(c => c.DeletFlag == DeleteFlag.NotDeleted);

                    }


                }
                //


                var x = Customers.ToList();
                TempData["CustomerForExc"] = Customers.Select(ID => ID.CustomerID).ToList();

                switch (sortOrder)
                {
                    case "name_desc":
                        Customers = Customers.OrderByDescending(s => s.EnName);
                        break;
                    case "Code":
                        Customers = Customers.OrderBy(s => s.CustomerID);
                        break;
                    case "code_desc":
                        Customers = Customers.OrderByDescending(s => s.CustomerID);
                        break;
                    default:  // Name ascending 
                        //Customers = Customers.OrderBy(s => s.EnName);
                        Customers = Customers.OrderBy(s => s.CustomerID);
                        break;
                }
                //int count = Currencies.ToList().Count();
                //IDs = new string[count];
                //IDs = Currencies.Select(s => s.Code).ToArray();

                int pageSize = 4;
                int pageNumber = (page ?? 1);
                ViewData["radioCHeck"] = RadioCHeck;
                int count = Customers.ToList().Count();
                IDs = new string[count];
                IDs = Customers.Select(s => s.CustomerID).ToArray();
                int[] myInts = Array.ConvertAll(IDs, s => int.Parse(s));
                Array.Sort(myInts);
                IDs = myInts.Select(z => z.ToString()).ToArray();

                return View("~/Areas/Customers/Views/Customer/Search.cshtml", Customers.ToPagedList(pageNumber, pageSize));
            }
        }
        
             [Route("Details")]
        [AuthorizedRights(Screen = "Customers", Right = "Read")]
        public ActionResult Details(string Id)
        {
            ViewData["Cities"] = new SelectList(dbContext.Cities.ToList(), "CityID", "Name");
            ViewData["Nationalities"] = new SelectList(dbContext.Nationalities.ToList(), "NationalityID", "Name");
            ViewData["CustType"] = new SelectList(dbContext.CustTypes.ToList(), "CustTypeID", "Name");
            ViewData["Branches"] = new SelectList(dbContext.Branches.ToList(), "BranchID", "BName");
            ViewData["IdType"] = new SelectList(dbContext.UserIdentityTypes.ToList(), "UserIdentityTypeID", "Name");
            var Model = dbContext.Customers.Where(c => c.CustomerID == Id).FirstOrDefault();

            var Actualy_auth = Model.Auth;
            var ThisUser = User.Identity.GetUserId();

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
            string test = null;

            CreateViewModel Customer = new CreateViewModel
            {
                CustomerID = Model.CustomerID,

                ArAddress1 = Model.ArAddress1,

                EnAddress1 = Model.EnAddress1,
                Salary = Model.Salary,
              

                ArName = Model.ArName,
                EnName = Model.EnName,

               
                CityId = Model.CityId,
                //Code = Model.Code,
              
                Email1 = Model.Email1,
            
               
                NationalityId = Model.NationalityId,
              
                IdNumber = Model.IdNumber,
             
                tel1 = Model.tel1,
              
                Auth = Model.Auth,
                Check= Model.Chk.GetValueOrDefault(),
                AuthForEditAndDelete=Actualy_auth,
                DateOfContribute = Model.DateOfContribute,
                DateOfHiring = Model.DateOfHiring
            };
            if (IDs != null)
            {
                if (Customer.CustomerID == IDs.Last())
                {
                    TempData["Last"] = "Last";
                }
                if (Customer.CustomerID == IDs.First())
                {
                    TempData["First"] = "First";
                }
            }
            else
            {
                TempData["Last"] = "Last";
                TempData["First"] = "First";
            }
            return View("~/Areas/Customers/Views/Customer/Details.cshtml", Customer);
        }
        
             [Route("Next")]
        [AuthorizedRights(Screen = "Customers", Right = "Read")]
        public ActionResult Next(string id)
        {
            var arrlenth = IDs.Count();
            var LastObj = dbContext.Customers.OrderBy(s => s.CustomerID).ToList().LastOrDefault(s => s.CustomerID == IDs[arrlenth - 1].ToString());
            if (id == LastObj.CustomerID)
            {
                TempData["Last"] = "Last";
                return RedirectToAction("Details", new { Id = LastObj.CustomerID });
            }
            else
            {
                int counter = 0;
                int next = 0;
                foreach (var item in IDs)
                {

                    if (item == id)
                    {
                        next = counter + 1;

                    }
                    counter++;
                }
                id = IDs[next];
                var Code = dbContext.Customers.Where(u => u.CustomerID == id.ToString()).Select(s => s.CustomerID).FirstOrDefault();
                if (Code == LastObj.CustomerID)
                {
                    TempData["Last"] = "Last";
                }
                else
                {
                    TempData["Last"] = "NotLast";
                }

                return RedirectToAction("Details", new { Id = Code });
            }

        }
        [Route("Previous")]
        [AuthorizedRights(Screen = "Customers", Right = "Read")]
        public ActionResult Previous(string id)
        {
            var arrindex = IDs[0];
            var FirstObj = dbContext.Customers.OrderBy(s => s.CustomerID).FirstOrDefault(s => s.CustomerID == arrindex.ToString());
            if (id.ToString() == FirstObj.CustomerID)
            {
                TempData["First"] = "First";
                return RedirectToAction("Details", new { Id = FirstObj.CustomerID });
            }
            else
            {
                int counter = 0;
                int previous = 0;
                foreach (var item in IDs)
                {

                    if (item == id)
                    {
                        previous = counter - 1;

                    }
                    counter++;
                }
                id = IDs[previous];

                if (id.ToString() == FirstObj.CustomerID)
                {
                    TempData["First"] = "First";
                }
                else
                {
                    TempData["First"] = "NotFirst";
                }
                var Code = dbContext.Customers.Where(u => u.CustomerID == id.ToString()).Select(s => s.CustomerID).FirstOrDefault();
                return RedirectToAction("Details", new { Id = Code });
            }

        }
        
              [Route("ExportToExcel")]
        [AuthorizedRights(Screen = "Customers", Right = "Read")]
        public ActionResult ExportToExcel()
        {
            string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");
            var gv = new GridView();
            var List = (List<string>)TempData["CustomerForExc"];
            gv.DataSource = dbContext.Customers.Where(s => List.Contains(s.CustomerID)).Select(x =>new { ArName = x.ArName, Name = x.EnName, CBOType = x.CustType.Name, Telephone = x.tel1, IdNumber = x.IdNumber, CRNnumber = x.CRNumber, City = x.City.Name, PostalCode = x.PostalCode, Branch = x.Branch.BName }).ToList();

            //gv.DataSource = dbContext.Customers.ToList();
            //gv.DataSource = dbContext.Customers.Where(Del => List.Contains(Del.CustomerID) && Del.DeletFlag == DeleteFlag.NotDeleted).Select(x => new {ArName= x.ArName,Name= x.EnName, CBOType = x.CustType.Name, Telephone=x.tel1, IdNumber=x.IdNumber, CRNnumber=x.CRNumber , City =x.City.Name, PostalCode= x.PostalCode,Branch=x.Branch.BName }).ToList();
            //gv.DataSource = dbContext.Customers.Where(s => List.Contains(s.CustomerID)).Select(x => new { ArName = x.EnName, Name = x.EnName, CBOType = x.CustType.Name, Telephone = x.tel1, IdNumber = x.IdNumber, CRNnumber = x.CRNumber, City = x.City.Name, PostalCode = x.PostalCode, Branch = x.Branch.BName }).OrderBy(u => u.ArName).ToList();
            //.Select(x => new { ArName = x.ArName, Name = x.EnName, CBOType = x.CustType.Name, Telephone = x.tel1, IdNumber = x.IdNumber, CRNnumber = x.CRNumber, City = x.City.Name, PostalCode = x.PostalCode, Branch = x.Branch.BName })

            //gv.DataSource = (from s in dbContext.Customers
            //                 select new
            //                 {
            //                     s.Code,
            //                     s.EnName,
            //                     s.ArName

            //                 })
            //                     .ToList();
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Customer" + NowTime + ".xls");
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

            return RedirectToAction("Search");
        }

        [AuthorizedRights(Screen = "Customers", Right = "Read")]
        public ActionResult ExportToPDF()
        {
            var Customers = dbContext.Customers.Select(s => s).Where(s => IDs.Contains(s.CustomerID)).OrderBy(u => u.EnName).ToList();
            string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");
            var report = new PartialViewAsPdf("~/Areas/Customers/Views/Customer/ExportToPDF.cshtml", Customers);
            return report;
        }
        [Route("Authorize")]
        [AuthorizedRights(Screen = "Customers", Right = "Authorized")]
        public ActionResult Authorize(string Id)
        {
            var customer = dbContext.Customers.Where(f => f.CustomerID == Id).FirstOrDefault();
            customer.Auth = true;
            customer.Auther = User.Identity.GetUserId();
            dbContext.SaveChanges();
            return RedirectToAction("Details",new { Id = Id });
        }
        [Route("Check")]
        [AuthorizedRights(Screen = "Customers", Right = "Check")]
        public ActionResult Check(string Id)
        {
            var customer = dbContext.Customers.Where(f => f.CustomerID == Id).FirstOrDefault();
            customer.Chk = true;
            customer.Checker = User.Identity.GetUserId();
            dbContext.SaveChanges();
            return RedirectToAction("Details", new { Id = Id });

        }
        [Route("Delete")]
        [AuthorizedRights(Screen = "Customers", Right = "Delete")]
        public ActionResult Delete(string Id)
        {
            var Customer = dbContext.Customers.Where(f => f.CustomerID == Id).FirstOrDefault();
            Customer.DeletFlag = DeleteFlag.Deleted;
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