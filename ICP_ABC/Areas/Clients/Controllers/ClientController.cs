using ClientGetInfo;
using ICP_ABC.Areas.Clients.Models;
using ICP_ABC.Areas.UsersSecurity.Models;
using ICP_ABC.Extentions;
using ICP_ABC.Models;
using Microsoft.ApplicationBlocks.Data;
using Microsoft.AspNet.Identity;
using Oracle.ManagedDataAccess.Client;
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

namespace ICP_ABC.Areas.Clients.Controllers
{
    [Authorize]
    public class ClientController : Controller
    {
        private ApplicationDbContext dbContext = new ApplicationDbContext();
        static int[] IDs;

        // GET: Clients/Client
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            var LastCode = dbContext.Client_CodeMap.OrderByDescending(s => s.Code).Select(s => s.Code).FirstOrDefault();
            //var TryingToParse = int.TryParse(LastCode, out int Code);
            if (LastCode == 0)
            {
                ViewData["LastCode"] = 1;
            }
            else
            {
                ViewData["LastCode"] = LastCode + 1;
            }

            return View();
        }
        //GetLastCode
        public string GetLastCode()
        {
            var querey = "SELECT MAX(CAST(Code AS INT)) as MaxCode FROM Calendar";
            var appSettings = ConfigurationManager.ConnectionStrings;
            var SqlCon = appSettings["ICPRO"];
            var ReturnedData = SqlHelper.ExecuteDataset(SqlCon.ToString(), CommandType.Text, querey);
            var LastCode = ReturnedData.Tables[0].Rows[0][0].ToString();
            return LastCode;
        }
        [HttpPost]
        public ActionResult Create(CreateClient model)

        {

            var Level = dbContext.UserSecurities.Select(Val => Val.Levels).FirstOrDefault();
            if (Level == Levels.TwoLevels)
            {
                //----------
                if (model != null)
                {
                    var CheckClientCID = dbContext.Client_CodeMap.Where(x => x.ICproCID == model.ClientNo).FirstOrDefault();
                    var CheckCoreCID = dbContext.Client_CodeMap.Where(x => x.CoreCID == model.ClientNoV8).FirstOrDefault();
                    var CheckCoreCIDAndClientCID = dbContext.Client_CodeMap.Where(x => x.ICproCID == model.ClientNo && x.CoreCID == model.ClientNoV8).FirstOrDefault();
                    if (CheckCoreCIDAndClientCID != null)
                    {
                        ViewData["InsertCase"] = "Client Already Map.";
                        return View(model);
                    }
                    else if (CheckCoreCID != null)
                    {
                        ViewData["InsertCase"] = "V8-CODE Already Exisit.";
                        return View(model);
                    }
                    else if (CheckClientCID != null)
                    {
                        ViewData["InsertCase"] = "Client Already Exisit. ";
                        return View(model);
                    }




                }
                else
                {
                    ViewData["InsertCase"] = "Error Data. ";
                    return View(model);
                }

                //---------

                var UserId = User.Identity.GetUserId();

                var client = new Client_CodeMap
                {
                    Code = model.Code,
                    ICproCID = model.ClientNo,
                    CoreCID = model.ClientNoV8,
                    auth = 0,
                    Chk = true,
                    StartDate = DateTime.Now,
                    Maker = UserId,
                    Checker = UserId

                };
                var currentUser = dbContext.Users.Where(u => u.Id == UserId).FirstOrDefault();

                var ClientLog = new Client_CodeMap_LOG
                {
                    Code = model.Code,
                    ICproCID = model.ClientNo,
                    CoreCID = model.ClientNoV8,
                    Maker = UserId,
                    StartDate = DateTime.Now,
                    V8branch = dbContext.Branches.Where(b => b.BranchID == currentUser.BranchId).FirstOrDefault().BranchID.ToString(),
                    V8City = model.CityV8.ToString(),
                    V8cboType = model.ClientTypeV8.ToString(),
                    V8eaddress = model.AddressV8,
                    V8emaddress = model.EMailV8,
                    V8fax = model.FAXV8,
                    V8ename = model.NameV8,
                    V8idnumber = model.IdNumberV8.ToString(),
                    V8idtype = model.IdTypeV8.ToString(),
                    V8nation = model.NationalityIdV8.ToString(),
                    V8tel = model.TelephoneV8.ToString(),
                    Checker = UserId,
                    Chk=true


                };

                dbContext.Client_CodeMap.Add(client);
                dbContext.Client_CodeMap_Log.Add(ClientLog);
                try
                {

                    dbContext.SaveChanges();
                    var Clients = dbContext.Client_CodeMap_Log.Where(c => c.DeleteFlag == DeleteFlag.NotDeleted).ToList();

                    int count = Clients.ToList().Count();
                    IDs = new int[count];
                    IDs = Clients.Select(s => s.Code).ToArray();
                    //int[] myInts = Array.ConvertAll(IDs, s => int.Parse(s));
                    Array.Sort(IDs);
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

                    return View(model);
                }

                return RedirectToAction("Details", new { Code = client.Code });
            }
            else
            {
                //----------
                if (model != null)
                {
                    var CheckClientCID = dbContext.Client_CodeMap.Where(x => x.ICproCID == model.ClientNo).FirstOrDefault();
                    var CheckCoreCID = dbContext.Client_CodeMap.Where(x => x.CoreCID == model.ClientNoV8).FirstOrDefault();
                    var CheckCoreCIDAndClientCID = dbContext.Client_CodeMap.Where(x => x.ICproCID == model.ClientNo && x.CoreCID == model.ClientNoV8).FirstOrDefault();
                    if (CheckCoreCIDAndClientCID != null)
                    {
                        ViewData["InsertCase"] = "Client Already Map.";
                        return View(model);
                    }
                    else if (CheckCoreCID != null)
                    {
                        ViewData["InsertCase"] = "V8-CODE Already Exisit.";
                        return View(model);
                    }
                    else if (CheckClientCID != null)
                    {
                        ViewData["InsertCase"] = "Client Already Exisit. ";
                        return View(model);
                    }




                }
                else
                {
                    ViewData["InsertCase"] = "Error Data. ";
                    return View(model);
                }

                //---------

                var UserId = User.Identity.GetUserId();

                var client = new Client_CodeMap
                {
                    Code = model.Code,
                    ICproCID = model.ClientNo,
                    CoreCID = model.ClientNoV8,
                    auth = 0,
                    Chk = false,
                    StartDate = DateTime.Now,
                    Maker = UserId

                };
                var currentUser = dbContext.Users.Where(u => u.Id == UserId).FirstOrDefault();

                var ClientLog = new Client_CodeMap_LOG
                {
                    Code = model.Code,
                    ICproCID = model.ClientNo,
                    CoreCID = model.ClientNoV8,
                    Maker = UserId,
                    StartDate = DateTime.Now,
                    V8branch = dbContext.Branches.Where(b => b.BranchID == currentUser.BranchId).FirstOrDefault().BranchID.ToString(),
                    V8City = model.CityV8.ToString(),
                    V8cboType = model.ClientTypeV8.ToString(),
                    V8eaddress = model.AddressV8,
                    V8emaddress = model.EMailV8,
                    V8fax = model.FAXV8,
                    V8ename = model.NameV8,
                    V8idnumber = model.IdNumberV8.ToString(),
                    V8idtype = model.IdTypeV8.ToString(),
                    V8nation = model.NationalityIdV8.ToString(),
                    V8tel = model.TelephoneV8.ToString(),


                };

                dbContext.Client_CodeMap.Add(client);
                dbContext.Client_CodeMap_Log.Add(ClientLog);
                try
                {

                    dbContext.SaveChanges();
                    var Clients = dbContext.Client_CodeMap_Log.Where(c => c.DeleteFlag == DeleteFlag.NotDeleted).ToList();

                    int count = Clients.ToList().Count();
                    IDs = new int[count];
                    IDs = Clients.Select(s => s.Code).ToArray();
                    //int[] myInts = Array.ConvertAll(IDs, s => int.Parse(s));
                    Array.Sort(IDs);
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

                    return View(model);
                }

                return RedirectToAction("Details", new { Code = client.Code });
            }
        }

        public ActionResult Edit(int Code)
        {
          var ClientV8 =  dbContext.Client_CodeMap_Log.Where(c => c.Code == Code).FirstOrDefault();
            var Client = dbContext.Client_CodeMap.Where(c => c.Code == Code).FirstOrDefault();
            var customer = dbContext.Customers.Where(c => c.CustomerID == Client.ICproCID).FirstOrDefault();
            var model = new CreateClient
            {
                //Code = Client.Code,
                //CityV8=int.Parse( ClientV8.V8City),
                //AddressV8 =ClientV8.V8eaddress,
                //BranchId = int.Parse(ClientV8.V8branch),
                //ClientTypeV8 = int.Parse(ClientV8.V8cboType),
                //CRNumberV8 =int.Parse( ClientV8.V8cboType),
                ////CodeV8 = int.Parse( ClientV8.CoreCID),
                //EMailV8 = ClientV8.V8emaddress,
                //FAXV8 = ClientV8.V8fax,
                //TelephoneV8 = ClientV8.V8tel,
                ////TelephoneV8 =int.Parse( ClientV8.V8tel),
                //NationalityIdV8 =int.Parse( ClientV8.V8nation),
                //IdTypeV8 = int.Parse(ClientV8.V8idtype),
                //ClientNoV8 = Client.CoreCID,
                //ClientNo = Client.ICproCID,
                //Address = customer.EnAddress1,
                //Name = customer.EnName,
                //EMail = customer.Email1,
                //NationalityId = customer.NationalityId,
                //Telephone =  customer.tel1.ToString(),
                //CRNumber =int.Parse( customer.CRNumber)


                 Code = Client.Code,

                CityV8 = int.Parse(ClientV8.V8City),
                City = customer.CityId,

                AddressV8 = ClientV8.V8eaddress,
                Address = customer.EnAddress1,

                BranchId = int.Parse(ClientV8.V8branch),
                BranchIdV8 = int.Parse(ClientV8.V8branch),

                ClientTypeV8 = int.Parse(ClientV8.V8cboType),
                ClientType = customer.CustTypeId,

                CRNumberV8 = int.Parse(ClientV8.V8cboType),
                CRNumber = int.Parse(customer.CRNumber),
                //CodeV8 = int.Parse( ClientV8.CoreCID),
                EMailV8 = ClientV8.V8emaddress,
                EMail = customer.Email1,

                FAXV8 = ClientV8.V8fax,
                FAX = customer.Fax1.ToString(),

                //TelephoneV8 = int.Parse(ClientV8.V8tel),
                NationalityIdV8 = int.Parse(ClientV8.V8nation),
                NationalityId = customer.NationalityId,

                IdTypeV8 = int.Parse(ClientV8.V8idtype),
                IdType = customer.idType,

                ClientNoV8 = Client.CoreCID,
                ClientNo = Client.ICproCID,

                Name = customer.EnName,
                NameV8 = ClientV8.V8ename,

                IdNumber = int.Parse(customer.IdNumber),
                IdNumberV8 = int.Parse(ClientV8.V8idnumber),

                TelephoneV8 = ClientV8.V8tel,
                Telephone = customer.tel1.Replace(" ", String.Empty)

            };
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(CreateClient model)
        {

           
            var client = new Client_CodeMap
            {
                Code = model.Code,
                ICproCID = model.ClientNo,
                CoreCID = model.ClientNoV8,
                auth = 0,
                Chk = false,

            };
           // var currentUser = dbContext.Users.Where(u => u.Id == UserId).FirstOrDefault();

            var ClientLog = new Client_CodeMap_LOG
            {
                Code = model.Code,
                ICproCID = model.ClientNo,
                CoreCID = model.ClientNoV8,
                StartDate = DateTime.Now,
                V8City = model.CityV8.ToString(),
                V8cboType = model.ClientTypeV8.ToString(),
                V8eaddress = model.AddressV8,
                V8emaddress = model.EMailV8,
                V8fax = model.FAXV8,
                V8ename = model.NameV8,
                V8idnumber = model.IdNumberV8.ToString(),
                V8idtype = model.IdTypeV8.ToString(),
                V8nation = model.NationalityIdV8.ToString(),
                V8tel = model.TelephoneV8.ToString(),


            };

            dbContext.Client_CodeMap.Add(client);
            dbContext.Client_CodeMap_Log.Add(ClientLog);
            try
            {

                dbContext.SaveChanges();
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
                    string[] errors = new string[] { eve.ToString() };
                    IdentityResult EditResult = new IdentityResult(errors);
                    AddErrors(EditResult);
                }

                
                return View();
            }
            return View(model);

        }

        public ViewResult Search(string sortOrder, string currentFilter, int? page ,string searchString,string Code,string RadioCHeck)
        {
            int seculevel = (int)dbContext.UserSecurities.FirstOrDefault().Levels;
            ViewBag.SecuLevel = seculevel;

            if (page==null&&sortOrder == null && currentFilter == null && searchString == null && Code == null && RadioCHeck==null)
            {
                List<SearchViewModel> ListData = new List<SearchViewModel>();
                int pageSize = 4;
                int pageNumber = (page ?? 1);
                //var Users = dbContext.Users.ToList().Take(0);
                //var userGroups = dbContext.UserGroups.ToList().Take(0);
                //ViewData["Groups"] = new SelectList(dbContext.UserGroups.ToList(), "GroupID", "Name");
                //ViewData["Branches"] = new SelectList(dbContext.Branches.ToList(), "BranchID", "Name");
                return View(ListData.ToPagedList(pageNumber, pageSize));

            }
            else {
                var Clients = dbContext.Client_CodeMap_Log.Where(c => c.DeleteFlag == DeleteFlag.NotDeleted).ToList();

                List<SearchViewModel> ListData = new List<SearchViewModel>();
                if (Clients != null)
                {
                    if (!String.IsNullOrEmpty(Code))
                    {

                        Clients = Clients.Where(s => s.ICproCID == Code).ToList();
                        ViewData["Code"] = Code;
                    }
                    if (!String.IsNullOrEmpty(searchString))
                    {

                        Clients = Clients.Where(s => s.CoreCID == Code).ToList();
                        ViewData["searchString"] = searchString;
                    }

                    //checkRadio 
                    if (!String.IsNullOrEmpty(RadioCHeck))
                    {
                        //Case Auth
                        if (RadioCHeck == "1")
                        {
                            Clients = Clients.Where(s => s.auth == 1).ToList();

                        }
                        //Case Checker
                        if (RadioCHeck == "2")
                        {
                            Clients = Clients.Where(s => s.Chk == true && s.auth == 0).ToList();

                        }
                        //Case maker
                        if (RadioCHeck == "3")
                        {
                            Clients = Clients.Where(s => s.auth == 0 && s.Chk == false).ToList();

                        }
                        //Case All
                        if (RadioCHeck == "4")
                        {
                            Clients = Clients.Where(c => c.DeleteFlag == DeleteFlag.NotDeleted).ToList();

                        }


                    }
                    //

                    var CustomerIDList = Clients.Select(s => s.ICproCID).ToList();
                    var Customers = dbContext.Customers.Where(c => c.DeletFlag == DeleteFlag.NotDeleted).ToList();
                    Customers = Customers.Where(c => CustomerIDList.Contains(c.CustomerID)).ToList();
                    if(Customers.Count > 0) { 
                    //var CustomerID = Clients.Select(s => s.ICproCID).ToList();
                    for (int i = 0; i < Clients.Count; i++)
                    {
                        var Model = new SearchViewModel
                        {
                            Code = Clients[i].Code,
                            CustomerName = Customers[i].EnName,
                            CoreCid = Clients[i].CoreCID,
                            ICProCid = Clients[i].ICproCID
                        };
                        ListData.Add(Model);
                    }
                    }
                    TempData["ClientExc"] = Clients.Select(x => x.Code).ToList();
                    switch (sortOrder)
                    {
                        case "name_desc":
                            Clients = Clients.OrderByDescending(s => s.Code).ToList();
                            break;
                        case "Code":
                            Clients = Clients.OrderBy(s => s.Code).ToList();
                            break;
                        case "code_desc":
                            Clients = Clients.OrderByDescending(s => s.Code).ToList();
                            break;
                        default:  // Name ascending 
                            Clients = Clients.OrderBy(s => s.Code).ToList();
                            break;
                    }
                }
                

                switch (sortOrder)
                {
                    case "name_desc":
                        Clients = Clients.OrderByDescending(s => s.Code).ToList();
                        break;
                    case "Code":
                        Clients = Clients.OrderBy(s => s.Code).ToList();
                        break;
                    case "code_desc":
                        Clients = Clients.OrderByDescending(s => s.Code).ToList();
                        break;
                    default:  // Name ascending 
                        Clients = Clients.OrderBy(s => s.Code).ToList();
                        break;
                }

                int pageSize = 4;
                int pageNumber = (page ?? 1);
                ViewData["radioCHeck"] = RadioCHeck;
                int count = Clients.ToList().Count();
                IDs = new int[count];
                IDs = Clients.Select(s => s.Code).ToArray();
                //int[] myInts = Array.ConvertAll(IDs, s => int.Parse(s));
                Array.Sort(IDs);
                //IDs = myInts.Select(x => x.ToString()).ToArray();

                //return View(Clients.ToPagedList(pageNumber, pageSize));
                return View(ListData.ToPagedList(pageNumber, pageSize));
            }
            
            //ViewBag.CurrentSort = sortOrder;
            //ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            //ViewBag.DateSortParm = sortOrder == "Code" ? "code_desc" : "Code";

            //if (CustId != null)
            //{
            //    page = 1;
            //}
            //else
            //{
            //    CustId = currentFilter;
            //}

            //ViewBag.CurrentFilter = CustId;

            //var Clients = dbContext.Client_CodeMap_Log.Select(s => s).Where(c => c.DeleteFlag == DeleteFlag.NotDeleted); 
           
        }


        public ActionResult Details(int Code)
        {

            var ClientV8 = dbContext.Client_CodeMap_Log.Where(c => c.Code == Code).FirstOrDefault();
            var Client = dbContext.Client_CodeMap.Where(c => c.Code == Code).FirstOrDefault();
            var customer = dbContext.Customers.Where(c => c.CustomerID == Client.ICproCID).FirstOrDefault();


            var Model = dbContext.Client_CodeMap_Log.Where(c => c.Code == Code).FirstOrDefault();
            var Actualy_auth = Model.auth;
            var ThisUser = User.Identity.GetUserId();
            if (Model.Maker == ThisUser)
            {
                if (Model.Chk == true && Model.auth == 0)
                {
                    Model.Chk = true;
                    Model.auth = 0;
                }
                if (Model.Chk == false && Model.auth == 0)
                {
                    Model.Chk = true;
                    Model.auth = 1;

                }
            }
            else
            {
                if (Model.Chk == true && Model.auth == 1)
                {
                    Model.Chk = true;
                    Model.auth = 1;

                }

                else if (Model.Chk == true && Model.auth == 0)
                {
                    if (Model.Checker != ThisUser)
                    {
                        Model.Chk = true;
                        Model.auth = 0;

                    }
                    else
                    {
                        Model.Chk = true;
                        Model.auth = 1;

                    }
                }
                else if (Model.Chk == false && Model.auth == 0)
                {
                    Model.Chk = false;
                    Model.auth = 1;

                }

            }


            //Fill Model
            var model = new DetailsviewModel
            {
                Code = Client.Code,
                
                CityV8 = int.Parse(ClientV8.V8City),
                City= customer.CityId,

                AddressV8 = ClientV8.V8eaddress,
                Address = customer.EnAddress1,

                BranchId = int.Parse(ClientV8.V8branch),
                BranchIdV8 = int.Parse(ClientV8.V8branch),

                ClientTypeV8 = int.Parse(ClientV8.V8cboType),
                ClientType = customer.CustTypeId,

                CRNumberV8 = int.Parse(ClientV8.V8cboType),
                CRNumber = int.Parse(customer.CRNumber),
                //CodeV8 = int.Parse( ClientV8.CoreCID),
                EMailV8 = ClientV8.V8emaddress,
                EMail = customer.Email1,

                FAXV8 = ClientV8.V8fax,
                FAX = customer.Fax1.ToString(),
             
                //TelephoneV8 = int.Parse(ClientV8.V8tel),
                NationalityIdV8 = int.Parse(ClientV8.V8nation),
                NationalityId = customer.NationalityId,

                IdTypeV8 = int.Parse(ClientV8.V8idtype),
                IdType = customer.idType,

                ClientNoV8 = Client.CoreCID,
                ClientNo = Client.ICproCID,
                
                Name = customer.EnName,
                NameV8 = ClientV8.V8ename,

                IdNumber = int.Parse(customer.IdNumber),
                IdNumberV8 = int.Parse(ClientV8.V8idnumber),

                TelephoneV8 = ClientV8.V8tel,
                Telephone = customer.tel1.Replace(" ", String.Empty),

                //-----------
                Checker = Model.Checker,
                Auther = Model.Auther,
                Chk = Model.Chk,
                auth = Model.auth,
                AuthForEditAndDelete = Actualy_auth


            };

            if (IDs != null)
            {
                if (model.Code == IDs.Last())
                {
                    TempData["Last"] = "Last";
                }
                if (model.Code == IDs.First())
                {
                    TempData["First"] = "First";
                }
            }
            else
            {
                TempData["Last"] = "Last";
                TempData["First"] = "First";
            }
            return View(model);
        }

        [AuthorizedRights(Screen = "Customers", Right = "Read")]
        public ActionResult Next(int id)
        {
            var arrlenth = IDs.Count();
            var LastObj = dbContext.Client_CodeMap_Log.OrderBy(s => s.Code).ToList().LastOrDefault(s => s.Code == IDs[arrlenth - 1]);
            if (id == LastObj.Code)
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

                    if (item == id)
                    {
                        next = counter + 1;

                    }
                    counter++;
                }
                id = IDs[next];
                var Code = dbContext.Client_CodeMap_Log.Where(u => u.Code == id).Select(s => s.Code).FirstOrDefault();
                if (Code == LastObj.Code)
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
        [AuthorizedRights(Screen = "Customers", Right = "Read")]
        public ActionResult Previous(int id)
        {
            var arrindex = IDs[0];
            var FirstObj = dbContext.Client_CodeMap_Log.OrderBy(s => s.Code).FirstOrDefault(s => s.Code == arrindex);
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
                var Code = dbContext.Client_CodeMap_Log.Where(u => u.Code == id).Select(s => s.Code).FirstOrDefault();
                return RedirectToAction("Details", new { Code = Code });
            }

        }

        [AuthorizedRights(Screen = "Customers", Right = "Read")]
        public ActionResult ExportToExcel()
        {
            string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");
            var gv = new GridView();
            var Lists = (List<int>)TempData["ClientExc"];

            //gv.DataSource = (from s in dbContext.Client_CodeMap_Log
            //                 select new
            //                 {
            //                     s.ICproCID,
            //                     s.CoreCID

            //                 })
            //                     .ToList();

             gv.DataSource = (from s in dbContext.Client_CodeMap_Log
                              join e in dbContext.Customers on s.ICproCID equals e.CustomerID
                              where  (Lists.Contains(s.Code))
                              select new
                              {
                                  e.EnName,
                                  s.ICproCID,
                                  s.CoreCID
                               
                              }).ToList();


            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Client" + NowTime + ".xls");
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
            //var branches = dbContext.Client_CodeMap_Log.Select(s => s).Where(s => IDs.Contains(s.Code)).OrderBy(u => u.Code).ToList();
            var Lists = (List<int>)TempData["ClientExc"];
            var Clients = (from s in dbContext.Client_CodeMap_Log
                             join e in dbContext.Customers on s.ICproCID equals e.CustomerID
                           where (Lists.Contains(s.Code))
                             select new
                             {
                                 e.EnName,
                                 s.ICproCID,
                                 s.CoreCID

                             }).ToList();
            List<SearchViewModel> Data = new List<SearchViewModel>();
            foreach (var item in Clients)
            {
                SearchViewModel Row = new SearchViewModel
                {
                    CoreCid= item.CoreCID,
                    ICProCid= item.ICproCID,
                    CustomerName = item.EnName
                };
                Data.Add(Row);

            }
            //var CustomerNAme = dbContext.Customers.Where(s => IDs.Contains(s.Code)).OrderBy(u => u.Code).ToList();
            string NowTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");

            var report = new PartialViewAsPdf("~/Areas/Clients/Views/Client/ExportToPDF.cshtml", Data);
            return report;

        }

        [AuthorizedRights(Screen = "Customers", Right = "Authorized")]
        public ActionResult AuthorizeCustomers(int Code)
        {
            var block = dbContext.Client_CodeMap_Log.Where(f => f.Code == Code).FirstOrDefault();
            block.auth = 1;
            block.Auther = User.Identity.GetUserId();
            dbContext.SaveChanges();
            return RedirectToAction("Details", new { Code = Code });
        }

        [AuthorizedRights(Screen = "Customers", Right = "Check")]
        public ActionResult CheckCustomers(int Code)
        {
            var Client = dbContext.Client_CodeMap_Log.Where(f => f.Code == Code).FirstOrDefault();
            Client.Chk = true;
            Client.Checker = User.Identity.GetUserId();
            dbContext.SaveChanges();
            return RedirectToAction("Details", new { Code = Code });
        }

        [AuthorizedRights(Screen = "Customers", Right = "Delete")]
        public ActionResult Delete(int Code)
        {
            var Client = dbContext.Client_CodeMap_Log.Where(f => f.Code == Code).FirstOrDefault();
            Client.DeleteFlag = DeleteFlag.Deleted;
            dbContext.SaveChanges();
            return RedirectToAction("Search");
        }
        [HttpPost]
        public JsonResult ICproGetUserInfo(string CustId)
        {
            var customer = dbContext.Customers.Where(c => c.CustomerID == CustId).FirstOrDefault();
            if (customer == null)
            {
                customer.EnName = "No Customer Found";
            }
            return Json(new { customerName = customer.EnName, BranchName = customer.Branch.BName });
        }

        public ActionResult CoreGetUserInfo(string ICproCID, string  CoreCID, string V8ename ,string V8idnumber ,string V8branch ,string V8tel)
        {
            
            string parameters ="";// = "select department_name from departments where department_id = 10";
            if (!String.IsNullOrEmpty(ICproCID))
            {
                ICproCID = ICproCID.ToUpper();
                parameters =  " and OLD_CLIENT_NO ="+ICproCID;
            }
            if (!String.IsNullOrEmpty(CoreCID))
            {
                CoreCID = CoreCID.ToUpper();
                parameters = parameters + " and CLIENT_NO=" + CoreCID;

            }
            if (!String.IsNullOrEmpty(V8ename))
            {
                parameters = parameters + " and CLIENT_NAME=" + V8ename;
            }
            if (!String.IsNullOrEmpty(V8idnumber))
            {
                parameters = parameters + " and GLOBAL_ID=" + V8idnumber;
            }
            if (!String.IsNullOrEmpty(V8branch))
            {
                parameters = parameters + " and CTRL_BRANCH=" + V8branch;
            }
            if (!String.IsNullOrEmpty(V8tel))
            {
                parameters = parameters + " and  PHONE_1=" + V8tel;
            }

            UpdateClient(parameters);
           

            return View();
        }
        private string UpdateClient(string QueryParameters)
        {
            string ret_value = "Failled";

            var cipher = new Cipher();
            string oradb = cipher.Decryption(ConfigurationManager.AppSettings["ORCL_ConStr"]);
                //"Data Source=ORCL;User Id=hr;Password=hr;";
            OracleConnection conn = new OracleConnection(oradb);
            DateTime time = DateTime.Now.AddSeconds(30);
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                string[] errors = new string[] { e.ToString() };
                IdentityResult EditResult = new IdentityResult(errors);
                AddErrors(EditResult);
                return ret_value;
            }
           
            
            
            while (DateTime.Now < time)
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select * from IF_EGYCBS.icpro_client WHERE 1=1 " + QueryParameters;
                cmd.CommandType = CommandType.Text;
                OracleDataReader dr = cmd.ExecuteReader();
                dr.Read();
           


                if (dr.HasRows)
                {
                    ret_value = "Succeeded";
                        //insert data based on retreved data
                        break;
                }
               
                conn.Dispose();
            }
            return ret_value;

        }

        private void AddErrors(IdentityResult result) 
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        //---------------------------
        [HttpPost]
        public ActionResult GetCustomerName(int Code)
        {
           
            var Name = dbContext.Customers.Where(f => f.CustomerID == Code.ToString()).Select(x=>x.EnName).FirstOrDefault();
            if(Name != null)
            {
                return Json(new { CustomerName = Name });

            }
            else
            {
                return Json(new { Message="Client Code not exist, Try Again." });
            }
        
            //dbContext.SaveChanges();
            //return RedirectToAction("Details", new { Code = Code });
        }
        [HttpPost]
        public ActionResult GetCustomerData(int Code)
        {
            //,string Name,string IdNumber, string BranchId, string Telephone
            var CustomerData = dbContext.Customers.Where(f => f.CustomerID == Code.ToString()).FirstOrDefault();


            //Get Names
            //var CustomerBranchName = dbContext.Branches.Where(f => f.BranchID == CustomerData.BranchId).Select(n=>n.BName).FirstOrDefault();

            //var CustomerCustomerCity = dbContext.Cities.Where(f => f.CityID == CustomerData.CityId).Select(n=>n.Name).FirstOrDefault();

            //var CustomerClientType = dbContext.CustTypes.Where(f => f.CustTypeID == CustomerData.CustTypeId).Select(n=>n.Name).FirstOrDefault();

            //var CustomerNationality = dbContext.Nationalities.Where(f => f.NationalityID == CustomerData.NationalityId).Select(n=>n.Name).FirstOrDefault();

            //var CustomerCustomerIdType = dbContext.UserIdentityTypes.Where(f => f.UserIdentityTypeID == CustomerData.idType).Select(n=>n.Name).FirstOrDefault();

           
           
            
            if (CustomerData !=null)
            {
                return Json(
                    new
                    {
                        CustomerCode = CustomerData.CustomerID,
                        CustomerName = CustomerData.EnName,
                        CustomerTelephone = CustomerData.tel1.Replace(" ", String.Empty),
                        CustomerBranchId = CustomerData.BranchId,
                        //CustomerIdNumber = 90,
                        CustomerIdNumber = CustomerData.IdNumber,
                        CustomerAddress = CustomerData.EnAddress1,
                        CustomerCity = CustomerData.CityId,
                        CustomerCRNumber = CustomerData.CRNumber,
                        CustomerFax = CustomerData.Fax1,
                        CustomerClientType = CustomerData.CustTypeId,
                        CustomerNationality = CustomerData.NationalityId,
                        CustomerIdType = CustomerData.idType,
                        CustomerEmail = CustomerData.Email1
                    });
                // byNameOfFiled
                //return Json(
                //    new
                //    {
                //        CustomerCode = CustomerData.Code,
                //        CustomerName = CustomerData.EnName,
                //        CustomerTelephone = CustomerData.tel1,
                //        CustomerBranchId = CustomerBranchName,
                //        CustomerIdNumber = CustomerData.IdNumber,
                //        CustomerAddress = CustomerData.EnAddress1,
                //        CustomerCity = CustomerCustomerCity,
                //        CustomerCRNumber = CustomerData.CRNumber,
                //        CustomerFax = CustomerData.Fax1,
                //        CustomerClientType = CustomerClientType,
                //        CustomerNationality = CustomerNationality,
                //        CustomerIdType = CustomerCustomerIdType,
                //        CustomerEmail = CustomerData.Email1
                //    });

            }
            else
            {
                return Json(new { Message="Client Code not exist, Try Again." });
            }
        
            //dbContext.SaveChanges();
            //return RedirectToAction("Details", new { Code = Code });
        }
        [HttpPost]
        public ActionResult GetCoreTESTData(int Code, string CoreCID, string V8ename, string V8idnumber, string V8branch, string V8tel)
        {
            //string ICproCID
            //,string Name,string IdNumber, string BranchId, string Telephone

            //GetName
            var CustomerData = dbContext.Customers.Where(f => f.CustomerID == Code.ToString()).FirstOrDefault();
            //var CustomerBranchName = dbContext.Branches.Where(f => f.BranchID == CustomerData.BranchId).Select(n=>n.BName).FirstOrDefault();
            //var CustomerCustomerCity = dbContext.Cities.Where(f => f.CityID == CustomerData.CityId).Select(n=>n.Name).FirstOrDefault();
            //var CustomerClientType = dbContext.CustTypes.Where(f => f.CustTypeID == CustomerData.CustTypeId).Select(n=>n.Name).FirstOrDefault();
            //var CustomerNationality = dbContext.Nationalities.Where(f => f.NationalityID == CustomerData.NationalityId).Select(n=>n.Name).FirstOrDefault();
            //var CustomerCustomerIdType = dbContext.UserIdentityTypes.Where(f => f.UserIdentityTypeID == CustomerData.idType).Select(n=>n.Name).FirstOrDefault();
            
            if (CustomerData !=null)
            {
                return Json(
                    new
                    {
                        CustomerCoreCode = CustomerData.CustomerID,
                        CustomerCoreName = CustomerData.EnName,
                        CustomerCoreTelephone = CustomerData.tel1.Replace(" ", String.Empty),
                        CustomerCoreBranchId = CustomerData.BranchId,
                        CustomerCoreIdNumber = CustomerData.IdNumber,
                        CustomerCoreAddress = CustomerData.EnAddress1,
                        CustomerCoreCity = CustomerData.CityId,
                        CustomerCoreCRNumber = CustomerData.CRNumber,
                        CustomerCoreFax = CustomerData.Fax1,
                        CustomerCoreClientType = CustomerData.CustTypeId,
                        CustomerCoreNationality = CustomerData.NationalityId,
                        CustomerCoreIdType = CustomerData.idType,
                        CustomerCoreEmail = CustomerData.Email1
                    });
                // byNameOfFiled
                //return Json(
                //    new
                //    {
                //        CustomerCoreCode = CustomerData.Code,
                //        CustomerCoreName = CustomerData.EnName,
                //        CustomerCoreTelephone = CustomerData.tel1,
                //        CustomerCoreBranchId = CustomerBranchName,
                //        CustomerCoreIdNumber = CustomerData.IdNumber,
                //        CustomerCoreAddress = CustomerData.EnAddress1,
                //        CustomerCoreCity = CustomerCustomerCity,
                //        CustomerCoreCRNumber = CustomerData.CRNumber,
                //        CustomerCoreFax = CustomerData.Fax1,
                //        CustomerCoreClientType = CustomerClientType,
                //        CustomerCoreNationality = CustomerNationality,
                //        CustomerCoreIdType = CustomerCustomerIdType,
                //        CustomerCoreEmail = CustomerData.Email1
                //    });

            }
            else
            {
                return Json(new { Message="Client Code not exist, Try Again." });
            }
        
            //dbContext.SaveChanges();
            //return RedirectToAction("Details", new { Code = Code });
        }
        [HttpPost]
        public ActionResult CheckExisit(string ClientCID, string CoreCID)
        {
            if(ClientCID !="" | ClientCID != null &&  CoreCID != "" | ClientCID != null)
            {
                var CheckClientCID = dbContext.Client_CodeMap.Where(x => x.ICproCID == ClientCID).FirstOrDefault();
                var CheckCoreCID = dbContext.Client_CodeMap.Where(x => x.CoreCID == CoreCID).FirstOrDefault();
                var CheckCoreCIDAndClientCID = dbContext.Client_CodeMap.Where(x => x.ICproCID == ClientCID && x.CoreCID == CoreCID).FirstOrDefault();
                if (CheckCoreCIDAndClientCID != null)
                {
                    return Json(new { Value = 1 });
                }
                else if (CheckCoreCID != null)
                {
                    return Json(new { Value = 3 });
                }
                else if (CheckClientCID != null)
                {
                    return Json(new { Value = 2 });
                }
                else
                {
                    return Json(new { Value = 0 });
                }


            }
            else
            {
                return Json(new { Value = 0 });
            }




        }
    }
}