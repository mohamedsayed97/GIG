using Microsoft.ApplicationBlocks.Data;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ICP_ABC.Models;
using ICP_ABC.Reports.Balancerptfrm2.Model;
using ICP_ABC.Reports.FundBallance.Model;
using Microsoft.AspNet.Identity;
using System.Web.Configuration;
namespace ICP_ABC.Reports.Balancerptfrm2.Controllers
{
    public class Balancerptfrm2Controller : Controller
    {
        // GET: Balancerptfrm
        static string servername = WebConfigurationManager.AppSettings["servername"];

        static string DataBaseName = WebConfigurationManager.AppSettings["DataBaseName"];
        private string SqlCon = "data source=" + servername + ";initial catalog=" + DataBaseName + ";persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework";

        //private string SqlCon = "data source=" + servername + ";initial catalog=LastUpdateDB;persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework";
        ApplicationDbContext _db = new ApplicationDbContext();

        DataSet dataSet = new DataSet();
        //Context _db = new Context();
        //private string SqlCon = "data source=SERVER2;initial catalog=IC_AWB2;persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework";
        // GET: clientstatfrm
        public ActionResult Index()
        {
            var BalancerptfrmVM = new Balancerptfrm2VM
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
        public ActionResult Balancerptfrm2(Balancerptfrm2VM VM)
        {
            var Data = GetData(VM);

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/Balancerptfrm2/Balancerptfrm2Report.rdlc");
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
        private List<Balancerptfrm2Data> GetData(Balancerptfrm2VM VM)
        {
            var Data = new List<Balancerptfrm2Data>();
            var cond = "";
            Session["fundstatcust"] = VM.Code;

            var this_User_id = User.Identity.GetUserId();
            var this_User_data = _db.Users.Where(x => x.Id == this_User_id).FirstOrDefault();
            Session["groupid"] = this_User_data.GroupId;
            Session["userno"] = this_User_data.Code;

            //Session["groupid"] = 9;
            //Session["userno"] = 11;
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
                var fname = FundDS.Tables[0].Rows[j].ItemArray[1];
                repDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select  t.cust_id,t.fund_id,sum(t.quantity) as quantity from (SELECT a.fund_id,a.cust_id,a.quantity - isnull(b.quantity,0) as quantity from (SELECT fund_id,cust_id,sum(quantity) as quantity from  ReportView  " + Session["userid21"] + " and fund_id=" + FundDS.Tables[0].Rows[j].ItemArray[0] + " and pur_sal=0 group by cust_id, fund_id)a full outer join  (SELECT fund_id,cust_id,sum(quantity) as quantity from  ReportView " + Session["userid21"] + " and fund_id=" + FundDS.Tables[0].Rows[j].ItemArray[0] + " and pur_sal=1 group by cust_id, fund_id)b on a.cust_id=b.cust_id and a.fund_id=b.fund_id) t   group by cust_id, fund_id  order  by  quantity desc");
                string cust_id, ename, total, branch_id;
                for (int i = 0; i <= repDS.Tables[0].Rows.Count - 1; i++)
                {
                    var Cust_DS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select EnName , isnull (bname,'') as bname from customer full outer join branch on branch.branchcode=customer.BranchId where Customer.Code='" + repDS.Tables[0].Rows[j].ItemArray[0] + "'");
                    cust_id = repDS.Tables[0].Rows[j].ItemArray[0].ToString(); 
                    ename = Cust_DS.Tables[0].Rows[j].ItemArray[0].ToString();
                    branch_id = Cust_DS.Tables[0].Rows[j].ItemArray[1].ToString();
                    total = repDS.Tables[0].Rows[j].ItemArray[2].ToString();
                    Data.Add(new Balancerptfrm2Data
                    {
                        branch_id = branch_id,
                        Cust_id = cust_id,
                        ename = ename,
                        Fname = fname.ToString(),
                        Toatal = total
                    });
                }
            }
            Session["repDS21"] = null;
            return Data;
        }

    }
}