using Microsoft.ApplicationBlocks.Data;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ICP_ABC.Models;
using ICP_ABC.Reports.MovementRpt.Model;
using Microsoft.AspNet.Identity;
using System.Web.Configuration;

namespace ICP_ABC.Reports.MovementRpt.Controllers
{
    public class MovementRptController : Controller
    {
        static string servername = WebConfigurationManager.AppSettings["servername"];
        static string DataBaseName = WebConfigurationManager.AppSettings["DataBaseName"];
        private string SqlCon = "data source=" + servername + ";initial catalog=" + DataBaseName + ";persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework";
        ApplicationDbContext _db = new ApplicationDbContext();

        // GET: MovementRpt
        //Context _db = new Context();
        //private string SqlCon = "data source=SERVER2;initial catalog=IC_AWB2;persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework";
        //// GET: Controllers
        public ActionResult Index()
        {
            var MovementRptVM = new MovementRptVM
            {
                Funds = _db.Funds.ToList()
            };

            return View(MovementRptVM);
        }

        public ActionResult MovementRpt(MovementRptVM VM)
        {
            var Data = GetData(VM);

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/MovementRpt/MovementRptReport.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = Data;
            localReport.DataSources.Add(reportDataSource);
            List<ReportParameter> parameters = new List<ReportParameter>();
            //DateTime FDate = (DateTime)Session["transfundfdate"];
            //DateTime TDate = (DateTime)Session["transfundtdate"];
            parameters.Add(new ReportParameter("FDate", Session["movefdate"].ToString()));
            parameters.Add(new ReportParameter("TDate", Session["movetdate"].ToString()));
            localReport.SetParameters(parameters);
            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtintion;
            if (reportType == "Excel")
            {
                fileNameExtintion = "xlsx";
            }
            if (reportType == "Word")
            {
                fileNameExtintion = "docx";
            }
            if (reportType == "PDF")
            {
                fileNameExtintion = "pdf";
            }
            if (reportType == "Image")
            {
                fileNameExtintion = "jpg";
            }
            string[] streams;
            Warning[] warnings;
            byte[] renderedByte;
            renderedByte = localReport.Render(reportType, "", out mimeType, out encoding, out fileNameExtintion, out streams, out warnings);
            Response.AddHeader("content-disposition", "inline; filename= Employee_Report." + fileNameExtintion);
            if (reportType == "PDF")
                return File(renderedByte, "application/pdf");
            else
                return File(renderedByte, fileNameExtintion);
        }


        public List<MovementRptData> GetData(MovementRptVM VM)
        {
            List<MovementRptData> Data = new List<MovementRptData>();
            var cond = "";
            var Flag = 0;
            int Flag1;
            Session["repno"] = 6;
            Session["movefdate"] = VM.FromDate.Value.ToString("yyyy-MM-dd");
            Session["movetdate"] = VM.ToDate.Value.ToString("yyyy-MM-dd");
            var Fund_Name = VM.Fund;

            var this_User_id = User.Identity.GetUserId();
            var this_User_data = _db.Users.Where(x => x.Id == this_User_id).FirstOrDefault();
            Session["groupid"] = this_User_data.GroupId;
            Session["userno"] = this_User_data.Code;

            //Session["groupid"] = 5;
            //Session["userno"] = 259;
            var druser = SqlHelper.ExecuteDataset(SqlCon, "Sp_druser", Convert.ToInt32(Session["userno"]));
            var branch_id = druser.Tables[0].Rows[0].ItemArray[0];
            var branch_right = druser.Tables[0].Rows[0].ItemArray[1];
            if (Convert.ToBoolean(branch_right) == true)
            {
                Session["branchno12"] = 0;
                Flag = 2;
            }
            else
            {
                Session["branchno12"] = Convert.ToInt32(branch_id);
                Flag = 3;
            }
            int fundid = 0;
            try
            {
                if (Fund_Name == 0 || Fund_Name == 0)
                {
                    fundid = 0;
                }
                else
                {
                    fundid = Convert.ToInt32(Fund_Name);
                }
            }
            catch (Exception)
            {

                fundid = 0;
            }
            Session["movefund"] = fundid;
            if (VM.CustomerCode == "" || VM.CustomerCode == null)
            {
                Session["movecust"] = "";
                Flag1 = 1;
            }
            else
            {
                Session["movecust"] = VM.CustomerCode;
                Flag1 = 4;
            }
            if (Convert.ToInt16(Session["movefund"]) == 0)
            {
                if (Flag1 == 1)
                {
                    if (Flag == 1)
                        cond = " where ClientMovement.value_date >= '" + Session["movefdate"] + "' and ClientMovement.value_date <= '" + Session["movetdate"] + "' ";
                    else if (Flag == 2)
                        cond = " where ClientMovement.value_date >= '" + Session["movefdate"] + "' and ClientMovement.value_date <= '" + Session["movetdate"] + "'  ";
                    else if (Flag == 3)
                        cond = " where ClientMovement.value_date >= '" + Session["movefdate"] + "' and ClientMovement.value_date <= '" + Session["movetdate"] + "'  " + " and ClientMovement.branch_id=" + Session["branchno9"];
                }
                else if (Flag1 == 2)
                {
                    if (Flag == 1)
                        cond = " where ClientMovement.value_date >= '" + Session["movefdate"] + "' and ClientMovement.value_date <= '" + Session["movetdate"] + "'  ";
                    else if (Flag == 2)
                        cond = " where ClientMovement.value_date >= '" + Session["movefdate"] + "' and ClientMovement.value_date <= '" + Session["movetdate"] + "'  ";
                    else if (Flag == 3)
                        cond = " where ClientMovement.value_date >= '" + Session["movefdate"] + "' and ClientMovement.value_date <= '" + Session["movetdate"] + "'  " + " and ClientMovement.branch_id=" + Session["branchno9"];
                }
                else if (Flag1 == 3)
                {
                    if (Flag == 1)
                        cond = " where ClientMovement.value_date >= '" + Session["movefdate"] + "' and ClientMovement.value_date <= '" + Session["movetdate"] + "'  ";
                    else if (Flag == 2)
                        cond = " where ClientMovement.value_date >= '" + Session["movefdate"] + "' and ClientMovement.value_date <= '" + Session["movetdate"] + "'  ";
                    else if (Flag == 3)
                        cond = " where ClientMovement.value_date >= '" + Session["movefdate"] + "' and ClientMovement.value_date <= '" + Session["movetdate"] + "' " + " and ClientMovement.branch_id=" + Session["branchno9"];
                }
                else if (Flag1 == 4)
                {
                    if (Flag == 1)
                        cond = " where ClientMovement.value_date >= '" + Session["movefdate"] + "' and ClientMovement.value_date <= '" + Session["movetdate"] + "' and ClientMovement.cust_id='" + Session["movecust"] + "'  ";
                    else if (Flag == 2)
                        cond = " where ClientMovement.value_date >= '" + Session["movefdate"] + "' and ClientMovement.value_date <= '" + Session["movetdate"] + "'  " + " and ClientMovement.cust_id='" + Session["movecust"] + "'";
                    else if (Flag == 3)
                        cond = " where ClientMovement.value_date >= '" + Session["movefdate"] + "' and ClientMovement.value_date <= '" + Session["movetdate"] + "'  " + " and ClientMovement.branch_id=" + Session["branchno9"] + " and ClientMovement.cust_id='" + Session["movecust"] + "'";
                }
            }
            else
            {
                if (Flag1 == 1)
                {
                    if (Flag == 1)
                        cond = " where ClientMovement.value_date >= '" + Session["movefdate"] + "' and ClientMovement.value_date <= '" + Session["movetdate"] + "' and ClientMovement.fund_id=" + Session["movefund"] + "  ";
                    else if (Flag == 2)
                        cond = " where ClientMovement.value_date >= '" + Session["movefdate"] + "' and ClientMovement.value_date <= '" + Session["movetdate"] + "'  " + " and ClientMovement.fund_id=" + Session["movefund"];
                    else if (Flag == 3)
                        cond = " where ClientMovement.value_date >= '" + Session["movefdate"] + "' and ClientMovement.value_date <= '" + Session["movetdate"] + "' " + " and ClientMovement.branch_id=" + Session["branchno9"] + " and ClientMovement.fund_id=" + Session["movefund"];
                }
                else if (Flag1 == 2)
                {
                    if (Flag == 1)
                        cond = " where ClientMovement.value_date >= '" + Session["movefdate"] + "' and ClientMovement.value_date <= '" + Session["movetdate"] + "'  " + " and ClientMovement.fund_id=" + Session["movefund"];
                    else if (Flag == 2)
                        cond = " where ClientMovement.value_date >= '" + Session["movefdate"] + "' and ClientMovement.value_date <= '" + Session["movetdate"] + "'  " + " and ClientMovement.fund_id=" + Session["movefund"];
                    else if (Flag == 3)
                        cond = " where ClientMovement.value_date >= '" + Session["movefdate"] + "' and ClientMovement.value_date <= '" + Session["movetdate"] + "'  " + " and ClientMovement.branch_id=" + Session["branchno9"] + " and ClientMovement.fund_id=" + Session["movefund"];
                }
                else if (Flag1 == 3)
                {
                    if (Flag == 1)
                        cond = " where ClientMovement.value_date >= '" + Session["movefdate"] + "' and ClientMovement.value_date <= '" + Session["movetdate"] + "'  " + " and ClientMovement.fund_id=" + Session["movefund"];
                    else if (Flag == 2)
                        cond = " where ClientMovement.value_date >= '" + Session["movefdate"] + "' and ClientMovement.value_date <= '" + Session["movetdate"] + "'  " + " and ClientMovement.fund_id=" + Session["movefund"];
                    else if (Flag == 3)
                        cond = " where ClientMovement.value_date >= '" + Session["movefdate"] + "' and ClientMovement.value_date <= '" + Session["movetdate"] + "' " + " and ClientMovement.branch_id=" + Session["branchno9"] + " and ClientMovement.fund_id=" + Session["movefund"];
                }
                else if (Flag1 == 4)
                {
                    if (Flag == 1)
                        cond = " where ClientMovement.value_date >= '" + Session["movefdate"] + "' and ClientMovement.value_date <= '" + Session["movetdate"] + "' and ClientMovement.cust_id='" + Session["movecust"] + "' and ClientMovement.fund_id=" + Session["movefund"] + " ";
                    else if (Flag == 2)
                        cond = " where ClientMovement.value_date >= '" + Session["movefdate"] + "' and ClientMovement.value_date <= '" + Session["movetdate"] + "'  " + " and ClientMovement.cust_id='" + Session["movecust"] + "' and ClientMovement.fund_id=" + Session["movefund"];
                    else if (Flag == 3)
                        cond = " where ClientMovement.value_date >= '" + Session["movefdate"] + "' and ClientMovement.value_date <= '" + Session["movetdate"] + "'  " + " and ClientMovement.branch_id=" + Session["branchno9"] + " and ClientMovement.cust_id='" + Session["movecust"] + "' and ClientMovement.fund_id=" + Session["movefund"];
                }

            }
            Flag = 0;
            Flag1 = 0;
            Session["userid6"] = cond;
            var repDS = SqlHelper.ExecuteDataset(SqlCon, System.Data.CommandType.Text, "SELECT * from ClientMovement" + Session["userid6"]);
            Session["repDS6"] = repDS;
            var CMDS = repDS;
            string aname = CMDS.Tables[0].Rows[0].ItemArray[18].ToString();
            string aaddress1 = CMDS.Tables[0].Rows[0].ItemArray[21].ToString();
            string Bname = CMDS.Tables[0].Rows[0].ItemArray[25].ToString();
            string cust_id = CMDS.Tables[0].Rows[0].ItemArray[0].ToString();
            string short_name = CMDS.Tables[0].Rows[0].ItemArray[28].ToString();
            string nav_today = CMDS.Tables[0].Rows[0].ItemArray[26].ToString();
            var FundDS = SqlHelper.ExecuteDataset(SqlCon, System.Data.CommandType.Text, "SELECT distinct ClientMovement.fund_id,fname from ClientMovement  " + Session["userid6"]);
            for (int j = 0; j <= FundDS.Tables[0].Rows.Count - 1; j++)
            {
                string Fname = FundDS.Tables[0].Rows[j].ItemArray[1].ToString();
                var Price_DS = SqlHelper.ExecuteDataset(SqlCon, System.Data.CommandType.Text, "select Icprice.Price from Icprice where Icprice.icdate=(select max(icdate) from icprice where FundId=" + FundDS.Tables[0].Rows[j].ItemArray[0] + ") and FundId=" + FundDS.Tables[0].Rows[j].ItemArray[0]);
                string Price = Price_DS.Tables[0].Rows[0][0].ToString();
                CMDS = SqlHelper.ExecuteDataset(SqlCon, System.Data.CommandType.Text, "select * from ClientMovement " + Session["userid6"] + " and fund_id=" + FundDS.Tables[0].Rows[j].ItemArray[0]);
                for (int i = 0; i <= CMDS.Tables[0].Rows.Count - 1; i++)
                {
                    string value_date = CMDS.Tables[0].Rows[i].ItemArray[4].ToString();
                    string unit_price = CMDS.Tables[0].Rows[i].ItemArray[12].ToString();
                    string qty;
                    if (Convert.ToInt32(CMDS.Tables[0].Rows[i].ItemArray[15]) == 0)
                    {
                        qty = CMDS.Tables[0].Rows[i].ItemArray[14].ToString();
                    }
                    else
                    {
                        qty = "-" + CMDS.Tables[0].Rows[i].ItemArray[14].ToString();
                    }
                    string tot = (Convert.ToDouble(qty) * Convert.ToDouble(unit_price)).ToString();
                    string navToday = (Convert.ToDouble(qty) * Convert.ToDouble(Price_DS.Tables[0].Rows[0][0])).ToString();
                    Data.Add(new MovementRptData
                    {
                        aaddress1 = aaddress1,
                        aname = aname,
                        Bname = Bname,
                        cust_id = cust_id,
                        fname = Fname,
                        navToday = navToday,
                        nav_today = nav_today,
                        price = Price,
                        value_date = Convert.ToDateTime(value_date),
                        unit_price = unit_price,
                        qty = qty,
                        tot = tot,
                        short_name = short_name
                    });
                }
            }
            Session["repDS6"] = null;
            return Data;
        }
    }
}



