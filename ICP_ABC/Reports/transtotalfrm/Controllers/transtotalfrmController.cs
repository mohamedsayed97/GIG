using Microsoft.ApplicationBlocks.Data;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ICP_ABC.Models;
using ICP_ABC.Reports.transtotalfrm.Model;
using Microsoft.AspNet.Identity;
using System.Web.Configuration;

namespace ICP_ABC.Reports.transtotalfrm.Controllers
{
    public class transtotalfrmController : Controller
    {
       static string servername = WebConfigurationManager.AppSettings["servername"];
        static string DataBaseName = WebConfigurationManager.AppSettings["DataBaseName"];

        private string SqlCon = "data source=" + servername + ";initial catalog=" + DataBaseName + ";persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework";
        ApplicationDbContext _db = new ApplicationDbContext();


        //private string SqlCon = "data source=SERVER2;initial catalog=IC_AWB2;persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework";
        DataSet dataSet = new DataSet();
       // Context _db = new Context();
        // GET: fundposfrm
        public ActionResult Index()
        {
            var transtotalfrmVM = new transtotalfrmVM
            {
                Funds = _db.Funds.ToList(),
                Branches = _db.Branches.ToList()
            };

            return View(transtotalfrmVM);
        }

        public ActionResult transtotalfrm(transtotalfrmVM VM)
        {
            var Data = GetData(VM);

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/transtotalfrm/transtotalfrmReport.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = Data;
            localReport.DataSources.Add(reportDataSource);
            List<ReportParameter> parameters = new List<ReportParameter>();
            //DateTime FDate = (DateTime)Session["transfundfdate"];
            //DateTime TDate = (DateTime)Session["transfundtdate"];
            parameters.Add(new ReportParameter("FDate", Session["transtotalfdate"].ToString()));
            parameters.Add(new ReportParameter("TDate", Session["transtotaltdate"].ToString()));
            parameters.Add(new ReportParameter("TransactionOver", Session["transover"].ToString()));
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

        private List<transtotalfrmData> GetData(transtotalfrmVM VM)
        {
            var Data = new List<transtotalfrmData>();
            string sub_red;
            string totNav;
            var Fund_Name = VM.Fund;
            var cond = "";
            var cond1 = "";
            int Flag = 0;

            var this_User_id = User.Identity.GetUserId();
            var this_User_data = _db.Users.Where(x => x.Id == this_User_id).FirstOrDefault();
            Session["groupid"] = this_User_data.GroupId;
            Session["userno"] = this_User_data.Code;

            //Session["groupid"] = 5;
            //Session["userno"] = 259;
            Session["transtotalfdate"] = VM.FromDate.Value.ToString("yyyy-MM-dd");
            Session["transtotaltdate"] = VM.ToDate.Value.ToString("yyyy-MM-dd");
            Session["repno"] = 18;
            var Branch_Name = VM.Branche;
            int fundid;
            try
            {
                if (Fund_Name == 0)
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
            Session["transtotalfundid"] = fundid;
            if (VM.Auth == "1")
                Session["trantotaltype"] = 1;
            else if (VM.Auth == "2")
                Session["trantotaltype"] = 0;
            else
                Session["trantotaltype"] = 2;
            if (Convert.ToBoolean(Branch_Name) == false)
            {
                Session["transtotalB"] = 0;
                Flag = 1;
            }
            else
            {
                Session["transtotalB"] = Branch_Name;
                Flag = 4;
            }
            if (VM.TransactionOver == null || VM.TransactionOver == "")
                Session["transover"] = 0;
            else
                Session["transover"] = VM.TransactionOver;
            if (Convert.ToInt32(Session["transtotalfundid"]) == 0)
            {
                if ((Convert.ToInt32(Session["transover"])) == 0)
                {
                    if ((Convert.ToInt32(Session["trantotaltype"])) == 0 || (Convert.ToInt32(Session["trantotaltype"])) == 1)
                    {
                        if (Flag == 1)
                            cond = " where TransOver.value_date >= '" + Session["transtotalfdate"] + "' and TransOver.value_date <= '" + Session["transtotaltdate"] + "' and TransOver.auth= " + Session["trantotaltype"];
                        else if (Flag == 2)
                            cond = " where TransOver.value_date >= '" + Session["transtotalfdate"] + "' and TransOver.value_date <= '" + Session["transtotaltdate"] + "'" + " and TransOver.auth= " + Session["trantotaltype"];
                        else if (Flag == 3)
                            cond = " where TransOver.value_date >= '" + Session["transtotalfdate"] + "' and TransOver.value_date <= '" + Session["transtotaltdate"] + "'  and TransOver.auth= " + Session["trantotaltype"];
                        else if (Flag == 4)
                            cond = " where TransOver.value_date >= '" + Session["transtotalfdate"] + "' and TransOver.value_date <= '" + Session["transtotaltdate"] + "'  and TransOver.branch_id=" + Session["transtotalB"] + " and TransOver.auth= " + Session["trantotaltype"];
                    }
                    else
                    {
                        if (Flag == 1)
                            cond = " where TransOver.value_date >= '" + Session["transtotalfdate"] + "' and TransOver.value_date <= '" + Session["transtotaltdate"] + "' ";
                        else if (Flag == 2)
                            cond = " where TransOver.value_date >= '" + Session["transtotalfdate"] + "' and TransOver.value_date <= '" + Session["transtotaltdate"] + "' ";
                        else if (Flag == 3)
                            cond = " where TransOver.value_date >= '" + Session["transtotalfdate"] + "' and TransOver.value_date <= '" + Session["transtotaltdate"] + "' ";
                        else if (Flag == 4)
                            cond = " where TransOver.value_date >= '" + Session["transtotalfdate"] + "' and TransOver.value_date <= '" + Session["transtotaltdate"] + "' " + " and TransOver.branch_id=" + Session["transtotalB"];
                    }
                }
                else
                {
                    if ((Convert.ToInt32(Session["trantotaltype"])) == 0 || (Convert.ToInt32(Session["trantotaltype"])) == 1)
                    {
                        if (Flag == 1)
                            cond = " where TransOver.value_date >= '" + Session["transtotalfdate"] + "' and TransOver.value_date <= '" + Session["transtotaltdate"] + "' and TransOver.auth= " + Session["trantotaltype"] + " and TransOver.total_value >=" + Session["transover"] + " ";
                        else if (Flag == 2)
                            cond = " where TransOver.value_date >= '" + Session["transtotalfdate"] + "' and TransOver.value_date <= '" + Session["transtotaltdate"] + "' " + " and TransOver.auth= " + Session["trantotaltype"] + " and TransOver.total_value >=" + Session["transover"];
                        else if (Flag == 3)
                            cond = " where TransOver.value_date >= '" + Session["transtotalfdate"] + "' and TransOver.value_date <= '" + Session["transtotaltdate"] + "' " + " and TransOver.auth= " + Session["trantotaltype"] + " and TransOver.total_value >=" + Session["transover"];
                        else if (Flag == 4)
                            cond = " where TransOver.value_date >= '" + Session["transtotalfdate"] + "' and TransOver.value_date <= '" + Session["transtotaltdate"] + "' " + " and TransOver.branch_id=" + Session["transtotalB"] + " and TransOver.auth= " + Session["trantotaltype"] + " and TransOver.total_value >=" + Session["transover"];
                    }
                    else
                    {
                        if (Flag == 1)
                            cond = " where TransOver.value_date >= '" + Session["transtotalfdate"] + "' and TransOver.value_date <= '" + Session["transtotaltdate"] + "' and TransOver.total_value >=" + Session["transover"] + " ";
                        else if (Flag == 2)
                            cond = " where TransOver.value_date >= '" + Session["transtotalfdate"] + "' and TransOver.value_date <= '" + Session["transtotaltdate"] + "' " + " and TransOver.total_value >=" + Session["transover"];
                        else if (Flag == 3)
                            cond = " where TransOver.value_date >= '" + Session["transtotalfdate"] + "' and TransOver.value_date <= '" + Session["transtotaltdate"] + "' " + " and TransOver.total_value >=" + Session["transover"];
                        else if (Flag == 4)
                            cond = " where TransOver.value_date >= '" + Session["transtotalfdate"] + "' and TransOver.value_date <= '" + Session["transtotaltdate"] + "'  " + " and TransOver.branch_id=" + Session["transtotalB"] + " and TransOver.total_value >=" + Session["transover"];
                    }
                }
            }
            else
            {
                if ((Convert.ToInt32(Session["transover"])) == 0)
                {
                    if ((Convert.ToInt32(Session["trantotaltype"])) == 0 || (Convert.ToInt32(Session["trantotaltype"])) == 1)
                    {
                        if (Flag == 1)
                            cond = " where TransOver.value_date >= '" + Session["transtotalfdate"] + "' and TransOver.value_date <= '" + Session["transtotaltdate"] + "' and TransOver.fund_id=" + Session["transtotalfundid"] + " and TransOver.auth= " + Session["trantotaltype"] + "  ";
                        else if (Flag == 2)
                            cond = " where TransOver.value_date >= '" + Session["transtotalfdate"] + "' and TransOver.value_date <= '" + Session["transtotaltdate"] + "'  " + " and TransOver.fund_id=" + Session["transtotalfundid"] + " and TransOver.auth= " + Session["trantotaltype"];
                        else if (Flag == 3)
                            cond = " where TransOver.value_date >= '" + Session["transtotalfdate"] + "' and TransOver.value_date <= '" + Session["transtotaltdate"] + "'  " + " and TransOver.fund_id=" + Session["transtotalfundid"] + " and TransOver.auth= " + Session["trantotaltype"];
                        else if (Flag == 4)
                            cond = " where TransOver.value_date >= '" + Session["transtotalfdate"] + "' and TransOver.value_date <= '" + Session["transtotaltdate"] + "'  " + " and TransOver.branch_id=" + Session["transtotalB"] + " and TransOver.fund_id=" + Session["transtotalfundid"] + " and TransOver.auth= " + Session["trantotaltype"];
                    }
                    else
                    {
                        if (Flag == 1)
                            cond = " where TransOver.value_date >= '" + Session["transtotalfdate"] + "' and TransOver.value_date <= '" + Session["transtotaltdate"] + "' and TransOver.fund_id=" + Session["transtotalfundid"] + " ";
                        else if (Flag == 2)
                            cond = " where TransOver.value_date >= '" + Session["transtotalfdate"] + "' and TransOver.value_date <= '" + Session["transtotaltdate"] + "'  " + " and TransOver.fund_id=" + Session["transtotalfundid"];
                        else if (Flag == 3)
                            cond = " where TransOver.value_date >= '" + Session["transtotalfdate"] + "' and TransOver.value_date <= '" + Session["transtotaltdate"] + "'  " + " and TransOver.fund_id=" + Session["transtotalfundid"];
                        else if (Flag == 4)
                            cond = " where TransOver.value_date >= '" + Session["transtotalfdate"] + "' and TransOver.value_date <= '" + Session["transtotaltdate"] + "'  " + " and TransOver.branch_id=" + Session["transtotalB"] + " and TransOver.fund_id=" + Session["transtotalfundid"];
                    }
                }
                else
                {
                    if ((Convert.ToInt32(Session["trantotaltype"])) == 0 || (Convert.ToInt32(Session["trantotaltype"])) == 1)
                    {
                        if (Flag == 1)
                            cond = " where TransOver.value_date >= '" + Session["transtotalfdate"] + "' and TransOver.value_date <= '" + Session["transtotaltdate"] + "' and TransOver.fund_id=" + Session["transtotalfundid"] + " and TransOver.auth= " + Session["trantotaltype"] + " and TransOver.total_value >=" + Session["transover"] + "  ";
                        else if (Flag == 2)
                            cond = " where TransOver.value_date >= '" + Session["transtotalfdate"] + "' and TransOver.value_date <= '" + Session["transtotaltdate"] + "'  " + " and TransOver.fund_id=" + Session["transtotalfundid"] + " and TransOver.auth= " + Session["trantotaltype"] + " and TransOver.total_value >=" + Session["transover"];
                        else if (Flag == 3)
                            cond = " where TransOver.value_date >= '" + Session["transtotalfdate"] + "' and TransOver.value_date <= '" + Session["transtotaltdate"] + "'  " + " and TransOver.fund_id=" + Session["transtotalfundid"] + " and TransOver.auth= " + Session["trantotaltype"] + " and TransOver.total_value >=" + Session["transover"];
                        else if (Flag == 4)
                            cond = " where TransOver.value_date >= '" + Session["transtotalfdate"] + "' and TransOver.value_date <= '" + Session["transtotaltdate"] + "' " + " and TransOver.branch_id=" + Session["transtotalB"] + " and TransOver.fund_id=" + Session["transtotalfundid"] + " and TransOver.auth= " + Session["trantotaltype"] + " and TransOver.total_value >=" + Session["transover"];
                    }
                    else
                    {
                        if (Flag == 1)
                            cond = " where TransOver.value_date >= '" + Session["transtotalfdate"] + "' and TransOver.value_date <= '" + Session["transtotaltdate"] + "' and TransOver.fund_id=" + Session["transtotalfundid"] + " and TransOver.total_value >=" + Session["transover"] + " ";
                        else if (Flag == 2)
                            cond = " where TransOver.value_date >= '" + Session["transtotalfdate"] + "' and TransOver.value_date <= '" + Session["transtotaltdate"] + "'  " + " and TransOver.fund_id=" + Session["transtotalfundid"] + " and TransOver.total_value >=" + Session["transover"];
                        else if (Flag == 3)
                            cond = " where TransOver.value_date >= '" + Session["transtotalfdate"] + "' and TransOver.value_date <= '" + Session["transtotaltdate"] + "' " + " and TransOver.fund_id=" + Session["transtotalfundid"] + " and TransOver.total_value >=" + Session["transover"];
                        else if (Flag == 4)
                            cond = " where TransOver.value_date >= '" + Session["transtotalfdate"] + "' and TransOver.value_date <= '" + Session["transtotaltdate"] + "' " + " and TransOver.branch_id=" + Session["transtotalB"] + " and TransOver.fund_id=" + Session["transtotalfundid"] + " and TransOver.total_value >=" + Session["transover"];
                    }
                }
            }
            Flag = 0;
            var FundRight = SqlHelper.ExecuteDataset(SqlCon, "Sp_FundRight1", Session["groupid"]);
            for (int i = 0; i <= FundRight.Tables[0].Rows.Count - 1; i++)
            {
                cond1 = "  TransOver.fund_id=" +Convert.ToInt32( FundRight.Tables[0].Rows[i].ItemArray[2]) + " or " + cond1;
            }
            var xx = 0;
            xx = cond1.Length;
            cond1 = Microsoft.VisualBasic.Strings.Left(cond1, xx - 3);
            cond1 = " and (" + cond1 + ")";
            Session["CondFund8"] = cond1;
            Session["userid18"] = cond;
            var FundDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select distinct TransOver.fund_id, fname from TransOver " + Session["userid18"]);
            for (int j = 0; j <= FundDS.Tables[0].Rows.Count - 1; j++)
            {
                var Fname = FundDS.Tables[0].Rows[j].ItemArray[1];
                var Price_DS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select Icprice.Price from Icprice where Icprice.icdate=(select max(icdate) from icprice where fund_id=" + FundDS.Tables[0].Rows[j].ItemArray[0] + ") and fund_id=" + FundDS.Tables[0].Rows[j].ItemArray[0]);
                var Price = Price_DS.Tables[0].Rows[0].ItemArray[0];
                var repDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "SELECT isnull(transid,'') as transid,isnull(Bname,'') as Bname,isnull(ename,'') as ename,isnull(cust_id,'') as cust_id,isnull(quantity,0) as quantity,isnull(pur_sal,'') as pur_sal,isnull( total_value,0) as total_value,isnull(inputer,'') as inputer,isnull(auther,'') as auther from TransOver " + Session["userid18"] + " and fund_id=" + FundDS.Tables[0].Rows[j].ItemArray[0] + " order by transid");
                for (int k = 0; k <= repDS.Tables[0].Rows.Count - 1; k++)
                {
                    if (Convert.ToInt32(repDS.Tables[0].Rows[k].ItemArray[5]) == 0)
                        sub_red = repDS.Tables[0].Rows[k].ItemArray[4].ToString();
                    else
                        sub_red = "-" + repDS.Tables[0].Rows[k].ItemArray[4].ToString();
                    totNav = (Convert.ToDouble(Price) * Convert.ToDouble(sub_red)).ToString();
                    Data.Add(new transtotalfrmData
                    {
                        Fname = Fname.ToString(),
                        totNav = totNav,
                        sub_red = sub_red,
                        auther = repDS.Tables[0].Rows[k].ItemArray[8].ToString(),
                        Bname = repDS.Tables[0].Rows[k].ItemArray[1].ToString(),
                        cust_id = repDS.Tables[0].Rows[k].ItemArray[3].ToString(),
                        ename = repDS.Tables[0].Rows[k].ItemArray[2].ToString(),
                        inputer = repDS.Tables[0].Rows[k].ItemArray[7].ToString(),
                        Price = Price.ToString(),
                        total_value = repDS.Tables[0].Rows[k].ItemArray[6].ToString(),
                        transid = repDS.Tables[0].Rows[k].ItemArray[0].ToString(),
                        //repDS.Tables[0].Rows[k].ItemArray[].ToString();
                    });
                }
            }
            return Data;
        }
    }
}