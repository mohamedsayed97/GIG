using Microsoft.ApplicationBlocks.Data;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ICP_ABC.Models;
using ICP_ABC.Reports.frmDeleteSub_Red.Model;
using Microsoft.AspNet.Identity;
using System.Web.Configuration;
namespace ICP_ABC.Reports.frmDeleteSub_Red.Controllers
{
    public class frmDeleteSub_RedController : Controller
    {
        static string servername = WebConfigurationManager.AppSettings["servername"];


        DataSet dataSet = new DataSet();

        //Context _db = new Context();
        ApplicationDbContext _db = new ApplicationDbContext();
        static string DataBaseName = WebConfigurationManager.AppSettings["DataBaseName"];
        private string SqlCon = "data source=" + servername + ";initial catalog=" + DataBaseName + ";persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework";

        //private string SqlCon = "data source=" + servername + ";initial catalog=LastUpdateDB;persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework";
        // GET: FundBalancefrm
        public ActionResult Index()
        {
            var frmDeleteSub_RedVM = new frmDeleteSub_RedVM
            {
                Types = new List<Sub_RedType>
                {
                    new Sub_RedType{ID=0,Name="Both"},
                    new Sub_RedType{ID=1,Name="Subscription"},
                    new Sub_RedType{ID=2,Name="Redemeption"}
                }
            };

            return View(frmDeleteSub_RedVM);
        }

        public ActionResult frmDeleteSub_Red(frmDeleteSub_RedVM VM)
        {
            var Data = GetData(VM);

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/frmDeleteSub_Red/frmDeleteSub_RedReport.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = Data;
            localReport.DataSources.Add(reportDataSource);
            List<ReportParameter> parameters = new List<ReportParameter>();
            //DateTime FDate = (DateTime)Session["transfundfdate"];
            //DateTime TDate = (DateTime)Session["transfundtdate"];
            parameters.Add(new ReportParameter("FDate", Session["Delfdate"].ToString()));
            parameters.Add(new ReportParameter("TDate", Session["Deltdate"].ToString()));
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

        public List<frmDeleteSub_RedData> GetData(frmDeleteSub_RedVM VM)
        {
            var Data = new List<frmDeleteSub_RedData>();
            var cond = "";
            var cond1 = "";
            int Flag = 0;
            var this_User_id = User.Identity.GetUserId();
            var this_User_data = _db.Users.Where(x => x.Id == this_User_id).FirstOrDefault();
            Session["groupid"] = this_User_data.GroupId;
            Session["userno"] = this_User_data.Code;
            //Session["groupid"] = 9;
            //Session["userno"] = 11;
            Session["Delfdate"] = VM.FromDate.Value.ToString("yyyy-MM-dd");
            Session["Deltdate"] = VM.ToDate.Value.ToString("yyyy-MM-dd");
            Session["repno"] = 100;
            if (VM.Code != "" && VM.Code != null)
            {
                if (VM.CustomerCode != "" && VM.CustomerCode != null)
                {
                    if (VM.Type == "0")
                        cond = " where DeletedTrans.del_date>='" + Session["Delfdate"] + "' and DeletedTrans.del_date<='" + Session["Deltdate"] + "' and DeletedTrans.code_sub=" + VM.Code + " or DeletedTrans.code_red=" + VM.Code + " and DeletedTrans.cust_id='" + VM.CustomerCode + "'";
                    else if (VM.Type == "1")
                        cond = " where DeletedTrans.del_date>='" + Session["Delfdate"] + "' and DeletedTrans.del_date<='" + Session["Deltdate"] + "' and DeletedTrans.code_sub=" + VM.Code + " and DeletedTrans.cust_id='" + VM.CustomerCode + "' and DeletedTrans.Sub_Flag=1";
                    else if (VM.Type == "2")
                        cond = " where DeletedTrans.del_date>='" + Session["Delfdate"] + "' and DeletedTrans.del_date<='" + Session["Deltdate"] + "' and DeletedTrans.code_red=" + VM.Code + " and DeletedTrans.cust_id='" + VM.CustomerCode + "' and DeletedTrans.Red_Flag=1";
                }
                else
                {
                    if (VM.Type == "0")
                        cond = " where DeletedTrans.del_date>='" + Session["Delfdate"] + "' and DeletedTrans.del_date<='" + Session["Deltdate"] + "' and DeletedTrans.code_sub=" + VM.Code + " or DeletedTrans.code_red=" + VM.Code;
                    else if (VM.Type == "1")
                        cond = " where DeletedTrans.del_date>='" + Session["Delfdate"] + "' and DeletedTrans.del_date<='" + Session["Deltdate"] + "' and DeletedTrans.code_sub=" + VM.Code + "and DeletedTrans.Sub_Flag=1";
                    else if (VM.Type == "2")
                        cond = " where DeletedTrans.del_date>='" + Session["Delfdate"] + "' and DeletedTrans.del_date<='" + Session["Deltdate"] + "' and DeletedTrans.code_red=" + VM.Code + "and DeletedTrans.Red_Flag=1";
                }
            }
            else
            {
                if (VM.CustomerCode != "" && VM.CustomerCode != null)
                {
                    if (VM.Type == "0")
                        cond = " where DeletedTrans.del_date>='" + Session["Delfdate"] + "' and DeletedTrans.del_date<='" + Session["Deltdate"] + "' and DeletedTrans.cust_id='" + VM.CustomerCode + "'";
                    else if (VM.Type == "1")
                        cond = " where DeletedTrans.del_date>='" + Session["Delfdate"] + "' and DeletedTrans.del_date<='" + Session["Deltdate"] + "' and DeletedTrans.cust_id='" + VM.CustomerCode + "' and DeletedTrans.Sub_Flag=1";
                    else if (VM.Type == "2")
                        cond = " where DeletedTrans.del_date>='" + Session["Delfdate"] + "' and DeletedTrans.del_date<='" + Session["Deltdate"] + "' and DeletedTrans.cust_id='" + VM.CustomerCode + "' and DeletedTrans.Red_Flag=1";
                }
                else
                {
                    if (VM.Type == "0")
                        cond = " where DeletedTrans.del_date>='" + Session["Delfdate"] + "' and DeletedTrans.del_date<='" + Session["Deltdate"] + "'";
                    else if (VM.Type == "1")
                        cond = " where DeletedTrans.del_date>='" + Session["Delfdate"] + "' and DeletedTrans.del_date<='" + Session["Deltdate"] + "' and DeletedTrans.Sub_Flag=1";
                    else if (VM.Type == "2")
                        cond = " where DeletedTrans.del_date>='" + Session["Delfdate"] + "' and DeletedTrans.del_date<='" + Session["Deltdate"] + "' and DeletedTrans.Red_Flag=1";
                }
            }
            Session["userid100"] = cond;
            var repDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "SELECT * from DeletedTrans " + Session["userid100"]);
            Session["repDS100"] = repDS;
            repDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "SELECT * from DeletedTrans " + Session["userid100"]);
            string rt = "0";
            string rTotal;
            string custName;
            string custID;
            string userID;
            string bName;
            string total;
            string noUnits;
            for (int i = 0; i <= repDS.Tables[0].Rows.Count - 1; i++)
            {
                rt = (Convert.ToInt32(rt) + 1).ToString();
                rTotal = rt;
                if (repDS.Tables[0].Rows[i].ItemArray[2] != null)
                    custName = repDS.Tables[0].Rows[i].ItemArray[2].ToString();
                else
                    custName = "";
                if (repDS.Tables[0].Rows[i].ItemArray[1] != null)
                    custID = repDS.Tables[0].Rows[i].ItemArray[1].ToString();
                else
                    custID = "";
                if (repDS.Tables[0].Rows[i].ItemArray[0] != null)
                    userID = repDS.Tables[0].Rows[i].ItemArray[0].ToString();
                else
                    userID = "";
                if (repDS.Tables[0].Rows[i].ItemArray[8] != null)
                    bName = repDS.Tables[0].Rows[i].ItemArray[8].ToString();
                else
                    bName = "";
                if (repDS.Tables[0].Rows[i].ItemArray[5] != null)
                    noUnits = repDS.Tables[0].Rows[i].ItemArray[5].ToString();
                else
                    noUnits = "";
                if (repDS.Tables[0].Rows[i].ItemArray[6] != null)
                    total = repDS.Tables[0].Rows[i].ItemArray[6].ToString();
                else
                    total = "";
                Data.Add(new frmDeleteSub_RedData
                {
                    bName = bName,
                    custID = custID,
                    noUnits = noUnits,
                    custName = custName,
                    rTotal = rTotal,
                    total = total,
                    userID = userID
                });
            }
            return Data;
        }
    }
}