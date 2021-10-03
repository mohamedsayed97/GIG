using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using ICP_ABC.Models;
using ICP_ABC.Reports.Balancerptfrm.Model;
using ICP_ABC.Reports.FundBallance.Model;
using Microsoft.Reporting.WebForms;
using Microsoft.ApplicationBlocks.Data;
using Microsoft.AspNet.Identity;
using System.Web.Configuration;
namespace ICP_ABC.Reports.Balancerptfrm.Controllers
{
    public class BalancerptfrmController : Controller
    {
        // GET: Balancerptfrm
        // GET: Balancerptfrm

        static string servername = WebConfigurationManager.AppSettings["servername"];


        DataSet dataSet = new DataSet();
        //Context _db = new Context();

        static string DataBaseName = WebConfigurationManager.AppSettings["DataBaseName"];
        private string SqlCon = "data source=" + servername + ";initial catalog=" + DataBaseName + ";persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework";


        //private string SqlCon = "data source=" + servername + ";initial catalog=LastUpdateDB;persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework";
        ApplicationDbContext _db = new ApplicationDbContext();
        // GET: clientstatfrm
        public ActionResult Index()
        {
            var BalancerptfrmVM = new BalancerptfrmVM
            {
                Types = new List<FundTypes>
                {
                    new FundTypes {Id=0,Name="Local/Mony Market" },
                    new FundTypes {Id=1,Name="Local/Equity" },
                    new FundTypes {Id=2,Name="Off Shore" },
                    new FundTypes {Id=3,Name="ALL" }
                },
                Funds = _db.Funds.ToList()
            };

            return View(BalancerptfrmVM);
        }
        public ActionResult Balancerptfrm(BalancerptfrmVM VM)
        {
            var Data = GetData(VM);

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/Balancerptfrm/BalancerptfrmReport.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = Data;
            localReport.DataSources.Add(reportDataSource);
            List<ReportParameter> parameters = new List<ReportParameter>();
            //DateTime FDate = (DateTime)Session["transfundfdate"];
            //DateTime TDate = (DateTime)Session["transfundtdate"];
            //parameters.Add(new ReportParameter("FDate", Session["ref8fdate"].ToString()));
            //parameters.Add(new ReportParameter("TDate", Session["ref8tdate"].ToString()));
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
        private List<BalancerptfrmData> GetData(BalancerptfrmVM VM)
        {
            var Data = new List<BalancerptfrmData>();
            var cond = "";
           
            var this_User_id = User.Identity.GetUserId();
            var this_User_data = _db.Users.Where(x => x.Id == this_User_id).FirstOrDefault();
            Session["groupid"] = this_User_data.GroupId;
            Session["userno"] = this_User_data.Code;


            //this_User_data.GroupId;
            Session["fundstatcust"] = VM.Code;
            //Session["groupid"] = 5;
            //Session["userno"] = 259;
            Session["repno"] = 21;
            Session["fundstatfdate"] = VM.FromDate.Value.ToString("yyyy-MM-dd");
            Session["fundstattdate"] = VM.ToDate.Value.ToString("yyyy-MM-dd");
            var Fund_Name = VM.Fund;
            var fund_type = VM.FundType;
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
            Session["fundstatfund"] = fundid;
            Session["fundstatcust"] = VM.Code;
            if (fund_type == 3)
            {
                Session["fundcustType"] = 0;
            }
            else
            {
                Session["fundcustType"] = fund_type;
            }
            Session["branchno"] = 0;
            if (Convert.ToInt32(Session["fundstatfund"]) == 0)
            {
                if (Session["fundstatcust"].ToString() == "")
                {
                    if (Convert.ToInt32(Session["fundcustType"]) == 0)
                        cond = " where ReportView.value_date >= '" + Session["fundstatfdate"] + "' and ReportView.value_date <= '" + Session["fundstattdate"] + "'";
                    else
                        cond = " where ReportView.value_date >= '" + Session["fundstatfdate"] + "' and ReportView.value_date <= '" + Session["fundstattdate"] + "' and ReportView.CustTypeId =" + Session["fundcustType"];
                }
                else
                {
                    if (Convert.ToInt32(Session["fundcustType"]) == 0)
                        cond = " where ReportView.value_date >= '" + Session["fundstatfdate"] + "' and ReportView.value_date <= '" + Session["fundstattdate"] + "' and ReportView.cust_id='" + Session["fundstatcust"] + "'";
                    else
                        cond = " where ReportView.value_date >= '" + Session["fundstatfdate"] + "' and ReportView.value_date <= '" + Session["fundstattdate"] + "' and ReportView.cust_id='" + Session["fundstatcust"] + "' and ReportView.CustTypeId =" + Session["fundcustType"];
                }
            }
            else
            {
                if (Session["fundstatcust"].ToString() == "")
                {
                    if (Convert.ToInt32(Session["fundcustType"]) == 0)
                        cond = " where ReportView.value_date >= '" + Session["fundstatfdate"] + "' and ReportView.value_date <= '" + Session["fundstattdate"] + "' and ReportView.fund_id=" + Session["fundstatfund"];
                    else
                        cond = " where ReportView.value_date >= '" + Session["fundstatfdate"] + "' and ReportView.value_date <= '" + Session["fundstattdate"] + "' and ReportView.fund_id=" + Session["fundstatfund"] + " and ReportView.CustTypeId =" + Session["fundcustType"];
                }
                else
                {
                    if (Convert.ToInt32(Session["fundcustType"]) == 0)
                        cond = " where ReportView.value_date >= '" + Session["fundstatfdate"] + "' and ReportView.value_date <= '" + Session["fundstattdate"] + "' and ReportView.fund_id=" + Session["fundstatfund"] + " and ReportView.cust_id='" + Session["fundstatcust"] + "'";
                    else
                        cond = " where ReportView.value_date >= '" + Session["fundstatfdate"] + "' and ReportView.value_date <= '" + Session["fundstattdate"] + "' and ReportView.fund_id=" + Session["fundstatfund"] + " and ReportView.cust_id='" + Session["fundstatcust"] + "' and ReportView.CustTypeId =" + Session["fundcustType"];
                }
            }
            Session["userid21"] = cond;
            var repDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "SELECT * from ReportView " + Session["userid21"]);
            Session["repDS21"] = repDS;
            var FundDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select distinct fund_id,fname from ReportView" + Session["userid21"]);
            for (int j = 0; j <= FundDS.Tables[0].Rows.Count - 1; j++)
            {
                var fname = FundDS.Tables[0].Rows[j].ItemArray[1].ToString();
                repDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "SELECT * from ReportView" + Session["userid21"] + " and fund_id=" + FundDS.Tables[0].Rows[j].ItemArray[0]);
                string cust_id, ename, total;
                if (repDS.Tables[0].Rows.Count != 0)
                {
                    cust_id = repDS.Tables[0].Rows[0].ItemArray[0].ToString();
                    ename = repDS.Tables[0].Rows[0].ItemArray[15].ToString();
                    var repDS1 = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "SELECT sum(quantity) from ReportView " + Session["userid21"] + " and auth=1 and pur_sal=0" + " and fund_id=" + FundDS.Tables[0].Rows[0].ItemArray[0]);
                    var repDS2 = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "SELECT sum(quantity) from ReportView " + Session["userid21"] + " and auth=1 and pur_sal=1" + " and fund_id=" + FundDS.Tables[0].Rows[0].ItemArray[0]);
                    if (repDS2.Tables[0].Rows[0].ItemArray[0] == null)
                    {
                        total = (Convert.ToDouble(repDS1.Tables[0].Rows[0].ItemArray[0]) - 0).ToString();
                    }
                    else
                        total = (Convert.ToDouble(repDS1.Tables[0].Rows[0].ItemArray[0]) - Convert.ToDouble(repDS2.Tables[0].Rows[0].ItemArray[0])).ToString();
                }
                else
                {
                    cust_id = "";
                    ename = "";
                    total = "0";
                }
                Data.Add(new BalancerptfrmData
                {
                    Cust_id = cust_id,
                    ename = ename,
                    Fname = fname,
                    Toatal = total
                });
            }
            Session["repDS21"] = null;
            return Data;
        }

    }
}
