using Microsoft.ApplicationBlocks.Data;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ICP_ABC.Models;
using ICP_ABC.Reports.branchpos.Model;
using ICP_ABC.Reports.fundposfrm.Model;
using Microsoft.AspNet.Identity;
using System.Web.Configuration;
namespace ICP_ABC.Reports.branchpos.Controllers
{
    public class branchposController : Controller
    {
        DataSet dataSet = new DataSet();
        static string servername = WebConfigurationManager.AppSettings["servername"];

        static string DataBaseName = WebConfigurationManager.AppSettings["DataBaseName"];
        private string SqlCon = "data source=" + servername + ";initial catalog=" + DataBaseName + ";persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework";

        //private string SqlCon = "data source=" + servername + ";initial catalog=LastUpdateDB;persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework";
        ApplicationDbContext _db = new ApplicationDbContext();

        //private string SqlCon = "data source=SERVER2;initial catalog=IC_AWB2;persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework";
        //Context _db = new Context();
        //// GET: branchpos
        public ActionResult Index()
        {
            var branchposVM = new branchposVM
            {
                Funds = _db.Funds.ToList(),
                Branches = _db.Branches.ToList()
            };

            return View(branchposVM);
        }
        public ActionResult branchpos(branchposVM VM)
        {
            var Data = GetData(VM);

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/fundposfrm/fundposfrmreport.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = Data;
            localReport.DataSources.Add(reportDataSource);
            List<ReportParameter> parameters = new List<ReportParameter>();
            //DateTime FDate = (DateTime)Session["transfundfdate"];
            //DateTime TDate = (DateTime)Session["transfundtdate"];
            parameters.Add(new ReportParameter("FDate", Session["report1fdate"].ToString()));
            parameters.Add(new ReportParameter("TDate", Session["report1tdate"].ToString()));
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
        private List<fundposfrmData> GetData(branchposVM VM)
        {
            List<fundposfrmData> Data = new List<fundposfrmData>();
            string t1 = "0";
            string t2 = "0";
            string t3 = "0";
            string t4 = "0";
            string noShares = "";
            string navToday = "";
            var cond = "";
            int Flag = 0;

            var this_User_id = User.Identity.GetUserId();
            var this_User_data = _db.Users.Where(x => x.Id == this_User_id).FirstOrDefault();
            Session["groupid"] = this_User_data.GroupId;
            Session["userno"] = this_User_data.Code;
            //Session["groupid"] = 5;
            //Session["userno"] = 259;

            //Session["groupid"] = 9;
            //Session["userno"] = 11;
            Session["report1fdate"] = VM.FromDate.Value.ToString("yyyy-MM-dd");
            Session["report1tdate"] = VM.ToDate.Value.ToString("yyyy-MM-dd");
            Session["repno"] = 4;
            var Fund_Name = VM.Fund;
            var Branch_Name = VM.Branche;
            int fundid;
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
            Session["report1fund"] = fundid;
            if (Convert.ToBoolean(Branch_Name) == false)
            {
                Session["report1branch"] = 0;
                Flag = 1;
            }
            else
            {
                Session["report1branch"] = Branch_Name;
                Flag = 4;
            }
            if (Convert.ToInt32(Session["report1fund"]) == 0)
            {
                if (Flag == 1)
                    cond = " where BranchPosition.value_date >= '" + Session["report1fdate"] + "' and BranchPosition.value_date <= '" + Session["report1tdate"] + "' ";
                else if (Flag == 2)
                    cond = " where BranchPosition.value_date >= '" + Session["report1fdate"] + "' and BranchPosition.value_date <= '" + Session["report1tdate"] + "' ";
                else if (Flag == 3)
                    cond = " where BranchPosition.value_date >= '" + Session["report1fdate"] + "' and BranchPosition.value_date <= '" + Session["report1tdate"] + "'";
                else if (Flag == 4)
                    cond = " where BranchPosition.value_date >= '" + Session["report1fdate"] + "' and BranchPosition.value_date <= '" + Session["report1tdate"] + "' " + " and BranchPosition.BranchId=" + Session["report1branch"];
            }
            else
            {
                if (Flag == 1)
                    cond = " where BranchPosition.value_date >= '" + Session["report1fdate"] + "' and BranchPosition.value_date <= '" + Session["report1tdate"] + "' and BranchPosition.fund_id=" + Session["report1fund"] + " ";
                else if (Flag == 2)
                    cond = " where BranchPosition.value_date >= '" + Session["report1fdate"] + "' and BranchPosition.value_date <= '" + Session["report1tdate"] + "' " + " and BranchPosition.fund_id=" + Session["report1fund"];
                else if (Flag == 3)
                    cond = " where BranchPosition.value_date >= '" + Session["report1fdate"] + "' and BranchPosition.value_date <= '" + Session["report1tdate"] + "' " + " and BranchPosition.fund_id=" + Session["report1fund"];
                else if (Flag == 4)
                    cond = " where BranchPosition.value_date >= '" + Session["report1fdate"] + "' and BranchPosition.value_date <= '" + Session["report1tdate"] + "' " + " and BranchPosition.BranchId=" + Session["report1branch"] + " and BranchPosition.fund_id=" + Session["report1fund"];
            }
            Flag = 0;
            string cond1 = "";
            var FundRight = SqlHelper.ExecuteDataset(SqlCon, "Sp_FundRight1", Session["groupid"]);
            for (int i = 0; i <= FundRight.Tables[0].Rows.Count - 1; i++)
            {
                cond1 = "  BranchPosition.fund_id=" + FundRight.Tables[0].Rows[i].ItemArray[0] + " or " + cond1;
            }
            var xx = 0;
            xx = cond1.Length;
            cond1 = Microsoft.VisualBasic.Strings.Left(cond1, xx - 3);
            cond1 = " and (" + cond1 + ")";
            Session["CondFund6"] = cond1;
            Session["userid4"] = cond;
            var repDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "SELECT * from BranchPosition " + Session["userid4"]);
            Session["repDS4"] = repDS;
            var FundDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select distinct BranchPosition.fund_id, fname from BranchPosition " + Session["userid4"]);
            var fname = "";
            for (int j = 0; j <= FundDS.Tables[0].Rows.Count - 1; j++)
            {
                var t11 = "0";
                var t22 = "0";
                var t33 = "0";
                var t44 = "0";
                fname = FundDS.Tables[0].Rows[j].ItemArray[1].ToString();
                var Price_DS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select Icprice.Price from Icprice where Icprice.icdate=(select max(icdate) from icprice where FundId=" + FundDS.Tables[0].Rows[j].ItemArray[0] + ") and fund_id=" + FundDS.Tables[0].Rows[j].ItemArray[0]);
                var Price = Price_DS.Tables[0].Rows[0].ItemArray[0];
                var brchDs = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select distinct BranchId,BName from BranchPosition" + Session["userid4"] + " and fund_id=" + FundDS.Tables[0].Rows[j].ItemArray[0] + " order by BranchId");
                for (int n = 0; n <= brchDs.Tables[0].Rows.Count - 1; n++)
                {
                    repDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select distinct (isnull(BranchPosition.cust_id,''))as cust_id,isnull(BranchId,'')as branch_id,isnull(bname,'') as bname,isnull(BranchPosition.EnName,'')as ename,isnull(a.total_sub,0) as total_sub,isnull(b.total_red,0)as total_red,isnull(a.total_sub,0) - isnull(b.total_red,0) as tot,(isnull(a.total_sub,0) - isnull(b.total_red,0)) * " + Price + " as nav from (select sum(quantity)as total_sub,cust_id  from BranchPosition " + Session["userid4"] + " and pur_sal=0 and fund_id=" + FundDS.Tables[0].Rows[j].ItemArray[0] + " and BranchId=" + brchDs.Tables[0].Rows[n].ItemArray[0] + "  group by cust_id  )a full outer join (select sum(quantity)as total_red,cust_id from BranchPosition " + Session["userid4"] + " and pur_sal=1 and fund_id=" + FundDS.Tables[0].Rows[j].ItemArray[0] + " and BranchId=" + brchDs.Tables[0].Rows[n].ItemArray[0] + "  group by cust_id)b on a.cust_id=b.cust_id full outer join BranchPosition on a.cust_id=BranchPosition.cust_id or b.cust_id=BranchPosition.cust_id " + Session["userid4"] + " and fund_id=" + FundDS.Tables[0].Rows[j].ItemArray[0] + " and BranchId=" + brchDs.Tables[0].Rows[n].ItemArray[0]);
                                                                                        
                    t1 = "0";
                    t2 = "0";
                    t3 = "0";
                    t4 = "0";
                    string branch = "0";
                    for (int z = 0; z <= repDS.Tables[0].Rows.Count - 1; z++)
                    {
                        noShares = (Convert.ToDouble(repDS.Tables[0].Rows[z].ItemArray[4]) - Convert.ToDouble(repDS.Tables[0].Rows[z].ItemArray[5])).ToString();
                        navToday = (Convert.ToDouble(Price) * Convert.ToDouble(noShares)).ToString();

                        t1 = (Convert.ToDouble(t1) + Convert.ToDouble(repDS.Tables[0].Rows[z].ItemArray[4])).ToString();
                        t2 = (Convert.ToDouble(t2) + Convert.ToDouble(repDS.Tables[0].Rows[z].ItemArray[5])).ToString();
                        t3 = (Convert.ToDouble(t3) + Convert.ToDouble(noShares)).ToString();
                        t4 = (Convert.ToDouble(t4) + Convert.ToDouble(navToday)).ToString();

                        t11 = (Convert.ToDouble(t11) + Convert.ToDouble(repDS.Tables[0].Rows[z].ItemArray[4])).ToString();
                        t22 = (Convert.ToDouble(t22) + Convert.ToDouble(repDS.Tables[0].Rows[z].ItemArray[5])).ToString();
                        t33 = (Convert.ToDouble(t33) + Convert.ToDouble(noShares)).ToString();
                        t44 = (Convert.ToDouble(t44) + Convert.ToDouble(navToday)).ToString();
                        Data.Add(new fundposfrmData
                        {
                            fname = fname,
                            price = Price.ToString(),
                            bname = brchDs.Tables[0].Rows[n].ItemArray[1].ToString(),
                            branch_id = brchDs.Tables[0].Rows[n].ItemArray[0].ToString(),
                            cust_id = repDS.Tables[0].Rows[z].ItemArray[0].ToString(),
                            ename = repDS.Tables[0].Rows[z].ItemArray[3].ToString(),
                            total_sub = Convert.ToDouble(repDS.Tables[0].Rows[z].ItemArray[4]),
                            total_red = Convert.ToDouble(repDS.Tables[0].Rows[z].ItemArray[5]),
                            noshares = Convert.ToDouble(noShares),
                            natoday = Convert.ToDouble(navToday)
                        });
                    }
                    t1 = "0";
                    t2 = "0";
                    t3 = "0";
                    t4 = "0";
                }
            }
            return Data;
        }
    }
}