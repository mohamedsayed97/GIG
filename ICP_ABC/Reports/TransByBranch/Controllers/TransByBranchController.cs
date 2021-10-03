using Microsoft.ApplicationBlocks.Data;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ICP_ABC.Models;
using ICP_ABC.Reports.TransByBranch.Model;
using Microsoft.AspNet.Identity;
using System.Web.Configuration;
namespace ICP_ABC.Reports.TransByBranch.Controllers
{
    public class TransByBranchController : Controller
    {
        static string servername = WebConfigurationManager.AppSettings["servername"];
        static string DataBaseName = WebConfigurationManager.AppSettings["DataBaseName"];

        private string SqlCon = "data source=" + servername + ";initial catalog="+ DataBaseName + ";persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework";
        ApplicationDbContext _db = new ApplicationDbContext();

        //Context _db = new Context();
        //private string SqlCon = "data source=SERVER2;initial catalog=IC_AWB2;persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework";
        DataSet dataSet = new DataSet();
        
        // GET: fundposfrm
        public ActionResult Index()
        {
            var TransByBranchVM = new TransByBranchVM
            {
                Funds = _db.Funds.ToList(),
                Branches = _db.Branches.ToList()
            };

            return View(TransByBranchVM);
        }
        public ActionResult TransByBranch(TransByBranchVM VM)
        {
            var Data = GetData(VM);

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/TransByBranch/TransByBranchReport.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = Data;
            localReport.DataSources.Add(reportDataSource);
            List<ReportParameter> parameters = new List<ReportParameter>();
            //DateTime FDate = (DateTime)Session["transfundfdate"];
            //DateTime TDate = (DateTime)Session["transfundtdate"];
            parameters.Add(new ReportParameter("FDate", Session["transbranchfdate"].ToString()));
            parameters.Add(new ReportParameter("TDate", Session["transbranchtdate"].ToString()));
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
        private List<TransByBranchData> GetData(TransByBranchVM VM)
        {
            var Data = new List<TransByBranchData>();
            var this_User_id = User.Identity.GetUserId();
            var this_User_data = _db.Users.Where(x => x.Id == this_User_id).FirstOrDefault();
            Session["groupid"] = this_User_data.GroupId;
            Session["userno"] = this_User_data.Code;
            //Session["groupid"] = 5;
            //Session["userno"] = 259;
            string quantity = "0";
            string navToday = "0";
            var cond = "";
            var cond1 = "";
            int Flag = 0;
           
            Session["transbranchfdate"] = VM.FromDate.Value.ToString("yyyy-MM-dd");
            Session["transbranchtdate"] = VM.ToDate.Value.ToString("yyyy-MM-dd");
            Session["repno"] = 17;
            var Fund_Name = VM.Fund;
            var Branch_Name = VM.Branche;
            int fundid;
            if (Convert.ToBoolean(Branch_Name) == false)
            {
                Session["transbranchB"] = 0;
                Flag = 1;
            }
            else
            {
                Session["transbranchB"] = Branch_Name;
                Flag = 4;
            }
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
            Session["transbranchfundid"] = fundid;
            if (Convert.ToInt32(Session["transbranchfundid"]) == 0)
            {
                if (Flag == 1)
                    cond = " where TransByBranch.value_date >= '" + Session["transbranchfdate"] + "' and TransByBranch.value_date <= '" + Session["transbranchtdate"] + "'  ";
                else if (Flag == 2)
                    cond = " where TransByBranch.value_date >= '" + Session["transbranchfdate"] + "' and TransByBranch.value_date <= '" + Session["transbranchtdate"] + "' ";
                else if (Flag == 3)
                    cond = " where TransByBranch.value_date >= '" + Session["transbranchfdate"] + "' and TransByBranch.value_date <= '" + Session["transbranchtdate"] + "' ";
                else if (Flag == 4)
                    cond = " where TransByBranch.value_date >= '" + Session["transbranchfdate"] + "' and TransByBranch.value_date <= '" + Session["transbranchtdate"] + "' " + " and TransByBranch.branch_id=" + Session["transbranchB"];
            }
            else
            {
                if (Flag == 1)
                    cond = " where TransByBranch.value_date >= '" + Session["transbranchfdate"] + "' and TransByBranch.value_date <= '" + Session["transbranchtdate"] + "' and TransByBranch.fund_id=" + Session["transbranchfundid"] + "  ";
                else if (Flag == 2)
                    cond = " where TransByBranch.value_date >= '" + Session["transbranchfdate"] + "' and TransByBranch.value_date <= '" + Session["transbranchtdate"] + "' " + " and TransByBranch.fund_id=" + Session["transbranchfundid"];
                else if (Flag == 3)
                    cond = " where TransByBranch.value_date >= '" + Session["transbranchfdate"] + "' and TransByBranch.value_date <= '" + Session["transbranchtdate"] + "' " + " and TransByBranch.fund_id=" + Session["transbranchfundid"];
                else if (Flag == 4)
                    cond = " where TransByBranch.value_date >= '" + Session["transbranchfdate"] + "' and TransByBranch.value_date <= '" + Session["transbranchtdate"] + "' " + " and TransByBranch.branch_id=" + Session["transbranchB"] + " and TransByBranch.fund_id=" + Session["transbranchfundid"];
            }
            Flag = 0;
            var FundRight = SqlHelper.ExecuteDataset(SqlCon, "Sp_FundRight1", Session["groupid"]);
            for (int i = 0; i <= FundRight.Tables[0].Rows.Count - 1; i++)
            {
                cond1 = "  TransByBranch.fund_id=" + FundRight.Tables[0].Rows[i].ItemArray[2] + " or " + cond1;
            }
            var xx = 0;
            xx = cond1.Length;
            cond1 = Microsoft.VisualBasic.Strings.Left(cond1, xx - 3);
            cond1 = " and (" + cond1 + ")";
            Session["CondFund7"] = cond1;
            Session["userid17"] = cond;
            var repDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "SELECT * from TransByBranch " + Session["userid17"]);
            Session["repDS17"] = repDS;
            var FundDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select  distinct TransByBranch.fund_id, fname  from TransByBranch  " + Session["userid17"]);
            for (int j = 0; j <= FundDS.Tables[0].Rows.Count - 1; j++)
            {
                var t33 = "0";
                var t44 = "0";
                var Fname = FundDS.Tables[0].Rows[j].ItemArray[1];
                var Price_DS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select Icprice.Price from Icprice where Icprice.icdate=(select max(icdate) from icprice where FundId=" + FundDS.Tables[0].Rows[j].ItemArray[0] + ") and FundId=" + FundDS.Tables[0].Rows[j].ItemArray[0]);
                var Price = Price_DS.Tables[0].Rows[0].ItemArray[0];
                repDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select isnull(TransByBranch.branch_id,'') as branch_id,isnull(bname,'') as bname, isnull(transid,'') as transid,isnull(EnName,'') as ename,isnull(TransByBranch.cust_id,'') as cust_id,isnull(convert(char(11),value_date,105),'') as value_date,isnull(inputer,'') as inputer,isnull(auther,'') as auther,isnull(pur_sal,'') as pur_sal,isnull(quantity,0) as quantity from  TransByBranch " + Session["userid17"] + " and fund_id=" + FundDS.Tables[0].Rows[j].ItemArray[0] + " order by branch_id");
                string t3 = "0";
                string t4 = "0";
                string branch = "0";
                for (int z = 0; z <= repDS.Tables[0].Rows.Count - 1; z++)
                {
                    if (Convert.ToInt32(repDS.Tables[0].Rows[z].ItemArray[0]) == Convert.ToInt32(branch))
                    {
                        if (Convert.ToInt32(repDS.Tables[0].Rows[z].ItemArray[8]) == 0)
                            quantity = repDS.Tables[0].Rows[z].ItemArray[9].ToString();
                        else
                            quantity = "-" + repDS.Tables[0].Rows[z].ItemArray[9].ToString();
                        navToday = (Convert.ToDouble(Price) * Convert.ToDouble(quantity)).ToString();
                        t3 = (Convert.ToDouble(t3) + Convert.ToDouble(quantity)).ToString();
                        t4 = (Convert.ToDouble(t4) + Convert.ToDouble(navToday)).ToString();
                        Flag = 1;
                    }
                    else
                    {
                        if (Flag != 0)
                        {
                            t3 = "0";
                            t4 = "0";
                            Flag = 0;
                        }
                        Flag = 1;
                        branch = repDS.Tables[0].Rows[z].ItemArray[0].ToString();
                        if (Convert.ToInt32(repDS.Tables[0].Rows[z].ItemArray[8]) == 0)
                            quantity = repDS.Tables[0].Rows[z].ItemArray[9].ToString();
                        else
                            quantity = "-" + repDS.Tables[0].Rows[z].ItemArray[9].ToString();
                        navToday = (Convert.ToDouble(Price) * Convert.ToDouble(quantity)).ToString();
                        t3 = (Convert.ToDouble(t3) + Convert.ToDouble(quantity)).ToString();
                        t4 = (Convert.ToDouble(t4) + Convert.ToDouble(navToday)).ToString();
                    }
                    t33 = (Convert.ToDouble(t33) + Convert.ToDouble(quantity)).ToString();
                    t44 = (Convert.ToDouble(t44) + Convert.ToDouble(navToday)).ToString();

                    Data.Add(new TransByBranchData
                    {
                        Fname = Fname.ToString(),
                        Price = Price.ToString(),
                        transid = repDS.Tables[0].Rows[z].ItemArray[2].ToString(),
                        ename = repDS.Tables[0].Rows[z].ItemArray[3].ToString(),
                        cust_id = repDS.Tables[0].Rows[z].ItemArray[4].ToString(),
                        quantity = Convert.ToDouble(quantity),
                        value_date = Convert.ToDateTime(repDS.Tables[0].Rows[z].ItemArray[5]),
                        inputer = repDS.Tables[0].Rows[z].ItemArray[6].ToString(),
                        auther = repDS.Tables[0].Rows[z].ItemArray[7].ToString(),
                        bname = repDS.Tables[0].Rows[z].ItemArray[1].ToString(),
                        branch_id = repDS.Tables[0].Rows[z].ItemArray[0].ToString(),
                        navToday = Convert.ToDouble(navToday)
                    });
                }
                if (Flag != 0)
                {
                    t3 = "0";
                    t4 = "0";
                    Flag = 0;
                }
            }
            return Data;
        }
    }
}
