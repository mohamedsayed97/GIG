using ICP_ABC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ICP_ABC.Areas.Company.Models;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using PagedList;
using Rotativa;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using ICP_ABC.Extentions;

namespace ICP_ABC.Areas.Company.Controllers
{
    public class CompanyController : Controller
    {
        private ApplicationDbContext dbContext;
 //       private readonly GenericCRUD<ICP_ABC.Areas.Company.Models.Company> gcrud;

        static string[] IDs;

        public CompanyController()
        {
            dbContext = new ApplicationDbContext();
   //         gcrud = new GenericCRUD<ICP_ABC.Areas.Company.Models.Company>();
        }
        // GET: Company/Company
        public ActionResult Index()
        {
            ViewData["LastCode"] = getlastcode() + 1;
            return View();
        }


        [AuthorizedRights(Screen = "Company", Right = "Create")]
        public ActionResult Create()
        {

            ViewData["LastCode"] = getlastcode() + 1;
            return View();
        }

        [AuthorizedRights(Screen = "Company", Right = "Create")]
        public ActionResult Create(CompanyViewModel company)
        {

            if (ModelState.IsValid)
            {
                int lastcode = getlastcode().Value+1;
                ICP_ABC.Areas.Company.Models.Company companies = new ICP_ABC.Areas.Company.Models.Company
                {

                    CompanyID = lastcode,
                    Companyname = company.Companyname,
                    ComericalRecord = company.ComericalRecord,
                    Address1 = company.Address1,
                    Address2 = company.Address2,
                    Phone1 = company.Phone1,
                    Phone2 = company.Phone2,
                    Maker = User.Identity.GetUserId(),
                    UserID = User.Identity.GetUserId(),
                };
              //  gcrud.Add(companies);
                dbContext.Companies.Add(companies);
                dbContext.SaveChanges();
                return RedirectToAction("Details", new { Code = lastcode });
            }
            return View(company);
        }

        [AuthorizedRights(Screen = "Company", Right = "Update")]
        public ActionResult Edit(int code)
        {
            var company = dbContext.Companies.Where(c => c.CompanyID == code).FirstOrDefault();
            CompanyViewModel model = new CompanyViewModel
            {
                CompanyID = company.CompanyID,
                Companyname = company.Companyname,
                ComericalRecord = company.ComericalRecord,
                Address1 = company.Address1,
                Address2 = company.Address2,
                Phone1 = company.Phone1,
                Phone2 = company.Phone2
            };
            return View(model);
        }

        [AuthorizedRights(Screen = "Company", Right = "Update")]
        public ActionResult Edit(CompanyViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var companydata = dbContext.Companies.Where(c => c.CompanyID == model.CompanyID).FirstOrDefault();
                    if (companydata == null)
                        return View(companydata);
                    companydata.Companyname = model.Companyname;
                    companydata.ComericalRecord = model.ComericalRecord;
                    companydata.Address1 = model.Address1;
                    companydata.Address2 = model.Address2;
                    companydata.Phone1 = model.Phone1;
                    companydata.Phone2 = model.Phone2;
                    companydata.EditFlag = true;
                    dbContext.SaveChanges();
                    //gcrud.Update((Company)model);
                    return RedirectToAction("Details", new { Code = model.CompanyID });
                }
                return View(model);

            }
            catch
            {
                return View();
            }
        }

        [AuthorizedRights(Screen = "Company", Right = "Delete")]
        public ActionResult Delete(int code)
        {
            try
            {
                var company = dbContext.Companies.Where(c => c.CompanyID == code).FirstOrDefault();
                if (company == null)
                    return HttpNotFound();
                else
                    company.DeletFlag = DeleteFlag.Deleted;
                dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [AuthorizedRights(Screen = "Company", Right = "Read")]
        public ViewResult Search(string sortOrder, string currentFilter, string searchString, int? page, string Code, string RadioCHeck)
        {
            int seculevel = (int)dbContext.UserSecurities.FirstOrDefault().Levels;
            ViewBag.SecuLevel = seculevel;

            if (page == null && sortOrder == null && currentFilter == null && searchString == null && Code == null && RadioCHeck == null)
            {
                int pageSize = 4;
                int pageNumber = (page ?? 1);
                var Companies = dbContext.Companies.Where(c => c.DeletFlag == DeleteFlag.NotDeleted).Take(0);

                return View(Companies.ToPagedList(pageNumber, pageSize));

            }
            else
            {


                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                ViewBag.DateSortParm = sortOrder == "Code" ? "code_desc" : "Code";


                ViewBag.CurrentFilter = searchString;

                var Companies = dbContext.Companies.Where(c => c.DeletFlag == DeleteFlag.NotDeleted);
                if (!String.IsNullOrEmpty(searchString))
                {
                    Companies = Companies.Where(s => s.Companyname.StartsWith(searchString));
                    ViewData["searchString"] = searchString;

                }

                //checkRadio 
                if (!String.IsNullOrEmpty(RadioCHeck))
                {
                    //Case Auth
                    if (RadioCHeck == "1")
                    {
                        Companies = Companies.Where(s => s.Auth == true);

                    }
                    //Case Checker
                    if (RadioCHeck == "2")
                    {
                    //    Companies = Companies.Where(s => s.Chk == true && s.Auth == false);

                    }
                    //Case maker
                    if (RadioCHeck == "3")
                    {
                        Companies = Companies.Where(s => s.Auth == false);
                        //cities = cities.Where(s => s.Auth == false && s.Chk == false);

                    }
                    //Case All
                    if (RadioCHeck == "4")
                    {
                        Companies = Companies.Where(c => c.DeletFlag == DeleteFlag.NotDeleted);

                    }


                }
                //

                if (!String.IsNullOrEmpty(Code))
                {
                    Companies = Companies.Where(s => s.CompanyID.ToString().StartsWith(Code));
                    ViewData["Code"] = Code;
                }
                TempData["CompanyForEXC"] = Companies.Select(x => x.CompanyID).ToList();

                switch (sortOrder)
                {
                    case "name_desc":
                        Companies = Companies.OrderByDescending(s => s.Companyname);
                        break;
                    case "Code":
                        Companies = Companies.OrderBy(s => s.CompanyID);
                        break;
                    case "code_desc":
                        Companies = Companies.OrderByDescending(s => s.CompanyID);
                        break;
                    default:  // Name ascending 
                        Companies = Companies.OrderBy(s => s.CompanyID);
                        break;
                }
                //int count = cities.ToList().Count();
                //IDs = new string[count];
                //IDs = cities.Select(s => s.Code).ToArray();

                int pageSize = 4;
                int pageNumber = (page ?? 1);

                int count = Companies.ToList().Count();
                IDs = new string[count];
                IDs = Companies.Select(s => s.CompanyID.ToString()).ToArray();
                ViewData["radioCHeck"] = RadioCHeck;
                int[] myInts = Array.ConvertAll(IDs, s => int.Parse(s));
                Array.Sort(myInts);
                IDs = myInts.Select(x => x.ToString()).ToArray();

                return View(Companies.ToPagedList(pageNumber, pageSize));
            }
        }

        [AuthorizedRights(Screen = "Company", Right = "Read")]
        public ViewResult Details(string Code)
        {
            var Model = dbContext.Companies.Where(c => c.CompanyID.ToString() == Code).FirstOrDefault();
            var Actualy_auth = Model.Auth;
            var ThisUser = User.Identity.GetUserId();

            if (Model.Maker == ThisUser)
            {
               
                if (Model.Auth == false)
                {
                    Model.Auth = true;
                }
            }
            else
            {
                if (Model.Auth == true)
                {
                    Model.Auth = true;

                }

                else if ( Model.Auth == false)
                {
                    if (Model.Auther != ThisUser)
                    {
                        Model.Auth = false;
                    }
                    else
                    {
                        Model.Auth = true;
                    }
                }
                else if (Model.Auth == false)
                {
                    Model.Auth = true;
                }
            }

            CompanyDetailsViewModel Currentcompany = new CompanyDetailsViewModel
            {
                CompanyID = Model.CompanyID,
                Companyname = Model.Companyname,
                ComericalRecord=Model.ComericalRecord,
                Address1=Model.Address1,
                Address2=Model.Address2,
                Phone1=Model.Phone1,
                Phone2=Model.Phone2,
                Auth = Model.Auth,
                AuthForEditAndDelete=Actualy_auth
            };
            if (IDs != null)
            {
                if (Model.CompanyID.ToString() == IDs.Last())
                {
                    TempData["Last"] = "Last";
                }
                if (Model.CompanyID.ToString() == IDs.First())
                {
                    TempData["First"] = "First";
                }
            }
            else
            {
                TempData["Last"] = "Last";
                TempData["First"] = "First";
            }
            return View(Currentcompany);

        }

        [AuthorizedRights(Screen = "Company", Right = "Read")]
        public ActionResult Next(string id)
        {
            var arrlenth = IDs.Count();
            var LastObj = dbContext.Companies.OrderBy(s => s.CompanyID).ToList().LastOrDefault(s => s.CompanyID.ToString() == IDs[arrlenth - 1]);
            if (id == LastObj.CompanyID.ToString())
            {
                TempData["Last"] = "Last";
                return RedirectToAction("Details", new { Code = LastObj.CompanyID.ToString()});
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
                var Code = dbContext.Companies.Where(u => u.CompanyID.ToString() == id).Select(s => s.CompanyID).FirstOrDefault();
                if (Code == LastObj.CompanyID)
                {
                    TempData["Last"] = "Last";
                }
                else
                {
                    TempData["Last"] = "NotLast";
                }
                return RedirectToAction("Details", new { Code = Code });
            }
        }

        [AuthorizedRights(Screen = "Company", Right = "Read")]
        public ActionResult Previous(string id)
        {
            var arrindex = IDs[0];
            var FirstObj = dbContext.Cities.OrderBy(s => s.Code).FirstOrDefault(s => s.Code == arrindex);
            if (id == FirstObj.Code)
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

                    if (item == id)
                    {
                        previous = counter - 1;

                    }
                    counter++;
                }
                id = IDs[previous];
                if (id == FirstObj.Code)
                {
                    TempData["First"] = "First";
                }
                else
                {
                    TempData["First"] = "NotFirst";
                }
                var Code = dbContext.Cities.Where(u => u.Code == id).Select(s => s.Code).FirstOrDefault();
                return RedirectToAction("Details", new { Code = Code });
            }

        }

        [AuthorizedRights(Screen = "Company", Right = "Read")]
        public ActionResult ExportToExcel()
        {
            string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");
            var gv = new GridView();
            var List = (List<int>)TempData["CompanyForEXC"];

            gv.DataSource = dbContext.Companies.Where(Del => List.Contains(Del.CompanyID) && Del.DeletFlag == DeleteFlag.NotDeleted).Select(x => new { Code = x.CompanyID, CompanyName = x.Companyname,Comerical=x.ComericalRecord}).ToList();

            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Company" + NowTime + ".xls");
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
            ViewData["Branches"] = new SelectList(dbContext.Branches.ToList(), "branchcode", "BName");
            return RedirectToAction("Search");
        }

        [AuthorizedRights(Screen = "Company", Right = "Read")]
        public ActionResult ExportToPDF()
        {
            var Companies = dbContext.Companies.Select(s => s).Where(s => IDs.Contains(s.CompanyID.ToString())).OrderBy(u => u.Companyname).ToList();
            string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");

            var report = new PartialViewAsPdf("~/Areas/Company/Views/Company/ExportToPDF.cshtml", Companies);
            return report;

        }

        public int? getlastcode()
        {
            int? LastID = dbContext.Companies.Max(C => (int?)C.CompanyID);
            if (LastID == null)
                LastID = 0;
            return LastID;
        }
    }
}
