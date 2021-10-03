using Microsoft.ApplicationBlocks.Data;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ICP_ABC.Models;
using ICP_ABC.Reports.NavRpt.Model;
using Microsoft.AspNet.Identity;
using System.Web.Configuration;
namespace ICP_ABC.Reports.NavRpt.Controllers
{
    public class NavRptController : Controller
    {
        static string servername = WebConfigurationManager.AppSettings["servername"];
        static string DataBaseName = WebConfigurationManager.AppSettings["DataBaseName"];
        private string SqlCon = "data source=" + servername + ";initial catalog="+ DataBaseName + ";persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework";
        ApplicationDbContext _db = new ApplicationDbContext();

        //private string SqlCon = "data source=SERVER2;initial catalog=IC_AWB2;persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework";
        //Context _db = new Context();
        DataSet dataSet = new DataSet();
       
        // GET: NavRpt
        public ActionResult Index()
        {
            var NavRptVM = new NavRptVM1
            {
                Funds = _db.Funds.ToList()
            };

            return View(NavRptVM);
        }

        public ActionResult NavRpt(NavRptVM1 VM)
        {
            var Data = GetData(VM);

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/NavRpt/NavRptReport.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = Data;
            localReport.DataSources.Add(reportDataSource);
            List<ReportParameter> parameters = new List<ReportParameter>();
            //DateTime FDate = (DateTime)Session["transfundfdate"];
            //DateTime TDate = (DateTime)Session["transfundtdate"];
            //parameters.Add(new ReportParameter("FDate", Session["navfdate"].ToString()));
            //parameters.Add(new ReportParameter("TDate", Session["navtdate"].ToString()));
            //localReport.SetParameters(parameters);
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

        public List<NavRptData> GetData(NavRptVM1 VM)
        {
            List<NavRptData> Data = new List<NavRptData>();
            int Flag = 0;
            Session["repno"] = 8;
            var this_User_id = User.Identity.GetUserId();
            var this_User_data = _db.Users.Where(x => x.Id == this_User_id).FirstOrDefault();
            Session["groupid"] = this_User_data.GroupId;
            Session["userno"] = this_User_data.Code;

            //Session["groupid"] = 5;
            //Session["userno"] = 259;
            Session["navfdate"] = VM.FromDate.Value.ToString("yyyy-MM-dd");
            Session["navtdate"] = VM.ToDate.Value.ToString("yyyy-MM-dd");
            var Fund_Name = VM.Fund;
            var cond = "";
            var cond1 = "";
            int fundid;
            string fname;
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
            Session["navfundid"] = fundid;
            if (Convert.ToInt32(Session["navfundid"]) == 0)
            {
                cond = " where Icprice.icdate >= '" + Session["navfdate"] + "' and Icprice.icdate <= '" + Session["navtdate"] + "'";
            }
            else
            {
                cond = " where Icprice.icdate >= '" + Session["navfdate"] + "' and Icprice.icdate <= '" + Session["navtdate"] + "' and Icprice.FundId=" + Session["navfundid"];
            }
            Flag = 0;
            var FundRight = SqlHelper.ExecuteDataset(SqlCon, "Sp_FundRight1", Session["groupid"]);
            for (int i = 0; i <= FundRight.Tables[0].Rows.Count - 1; i++)
            {
                cond1 = "  Icprice.FundId=" +Convert.ToInt32( FundRight.Tables[0].Rows[i].ItemArray[2] )+ " or " + cond1;
            }
            var xx = 0;
            xx = cond1.Length;
            cond1 = Microsoft.VisualBasic.Strings.Left(cond1, xx - 3);
            cond1 = " and (" + cond1 + ")";
            Session["CondFund5"] = cond1;
            Session["userid8"] = cond;
            var FundDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select distinct(fund.code),fname from icprice inner join fund on fund.code=Icprice.FundId  " + Session["userid8"] + Session["CondFund5"]);
            for (int j = 0; j <= FundDS.Tables[0].Rows.Count - 1; j++)
            {
                fname = FundDS.Tables[0].Rows[j].ItemArray[1].ToString();
                var repDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select distinct(fname),price,icdate from icprice inner join fund on fund.code=Icprice.FundId " + Session["userid8"] + " and fund.code=" + FundDS.Tables[0].Rows[j].ItemArray[0] + " order by icdate");
                for (int i = 0; i <= repDS.Tables[0].Rows.Count - 1; i++)
                {
                    var ICdate = Convert.ToDateTime(repDS.Tables[0].Rows[i].ItemArray[2]);
                    var price = repDS.Tables[0].Rows[i].ItemArray[1].ToString();
                    Data.Add(new NavRptData
                    {
                        fname = fname,
                        ICdate = ICdate,
                        price = price
                    });
                }
            }
            return Data;
        }
    }
}