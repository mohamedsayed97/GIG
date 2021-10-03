using Microsoft.ApplicationBlocks.Data;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ICP_ABC.Reports.ClientGroupinfo.Model;
using Microsoft.AspNet.Identity;
using ICP_ABC.Models;
using System.Web.Configuration;
namespace ICP_ABC.Reports.ClientGroupinfo.Controllers
{
    public class ClientGroupinfoController : Controller
    {
        //  private string SqlCon = "data source=SERVER2;initial catalog=IC_AWB2;persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework";
        // GET: ClientGroupinfo
        static string servername = WebConfigurationManager.AppSettings["servername"];

        static string DataBaseName = WebConfigurationManager.AppSettings["DataBaseName"];
        private string SqlCon = "data source=" + servername + ";initial catalog=" + DataBaseName + ";persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework";

        //private string SqlCon = "data source=" + servername + ";initial catalog=LastUpdateDB;persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework";
        ApplicationDbContext _db = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ClientGroupinfo(ClientGroupinfoVM VM)
        {
            var Data = GetData(VM);

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/ClientGroupinfo/ClientGroupinfo.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = Data;
            localReport.DataSources.Add(reportDataSource);
            List<ReportParameter> parameters = new List<ReportParameter>();
            //DateTime FDate = (DateTime)Session["transfundfdate"];
            //DateTime TDate = (DateTime)Session["transfundtdate"];
            parameters.Add(new ReportParameter("FDate", Session["clientinfofdate"].ToString()));
            parameters.Add(new ReportParameter("TDate", Session["clientinfotdate"].ToString()));
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
        private List<ClientGroupinfoData> GetData(ClientGroupinfoVM VM)
        {
            if (VM.Code == null)
            {
                VM.Code = "";
            }
            var this_User_id = User.Identity.GetUserId();
            var this_User_data = _db.Users.Where(x => x.Id == this_User_id).FirstOrDefault();
            Session["groupid"] = this_User_data.GroupId;
            Session["userno"] = this_User_data.Code;
            //Session["userno"] = 259;
            //Session["groupid"] = 5;
            var Data = new List<ClientGroupinfoData>();
            var cond = "";
            var Flag = 0;
            var druser = SqlHelper.ExecuteDataset(SqlCon, "Sp_druser", Convert.ToInt32(Session["userno"]));
            var branch_id = druser.Tables[0].Rows[0].ItemArray[0];
            var branch_right = druser.Tables[0].Rows[0].ItemArray[1];
            Session["repno"] = 20;
            Session["clientinfofdate"] = VM.FromDate.Value.ToString("yyyy-MM-dd");
            Session["clientinfotdate"] = VM.ToDate.Value.ToString("yyyy-MM-dd");
            Session["branchno4"] = branch_id;
            Flag = 3;
            var fundid = 0;
            Session["clientinfotype"] = 0;
            if (VM.Code == "" || VM.Code == null)
            {
                Session["clientinfofundid"] = 1;
            }
            else
            {
                Session["clientinfofundid"] = VM.CustomerCode;
            }
            if (Convert.ToInt32(Session["clientinfofundid"]) == 0)
            {
                if (Convert.ToInt32(Session["clientinfotype"]) == 0)
                {
                    if (Flag == 1)
                        cond = " where ClientInformation.value_date >= '" + Session["clientinfofdate"] + "' and ClientInformation.value_date <= '" + Session["clientinfotdate"] + "'  ";
                    else if (Flag == 2)
                        cond = " where ClientInformation.value_date >= '" + Session["clientinfofdate"] + "' and ClientInformation.value_date <= '" + Session["clientinfotdate"] + "' ";
                    else if (Flag == 3)
                        cond = " where ClientInformation.value_date >= '" + Session["clientinfofdate"] + "' and ClientInformation.value_date <= '" + Session["clientinfotdate"] + "' " + " and ClientInformation.branch_id=" + Session["branchno4"];
                }
                else
                {
                    if (Flag == 1)
                        cond = " where ClientInformation.value_date >= '" + Session["clientinfofdate"] + "' and ClientInformation.value_date <= '" + Session["clientinfotdate"] + "' and ClientInformation.CustTypeId=" + Session["clientinfotype"] + " ";
                    else if (Flag == 2)
                        cond = " where ClientInformation.value_date >= '" + Session["clientinfofdate"] + "' and ClientInformation.value_date <= '" + Session["clientinfotdate"] + "' and ClientInformation.CustTypeId=" + Session["clientinfotype"] + " ";
                    else if (Flag == 3)
                        cond = " where ClientInformation.value_date >= '" + Session["clientinfofdate"] + "' and ClientInformation.value_date <= '" + Session["clientinfotdate"] + "' and ClientInformation.CustTypeId=" + Session["clientinfotype"] + " " + " and ClientInformation.branch_id=" + Session["branchno4"];
                }
            }
            else
            {
                if (Convert.ToInt32(Session["clientinfotype"]) == 0)
                {
                    if (Flag == 1)
                        cond = " where ClientInformation.value_date >= '" + Session["clientinfofdate"] + "' and ClientInformation.value_date <= '" + Session["clientinfotdate"] + "' and ClientInformation.fund_id=" + Session["clientinfofundid"] + " ";
                    else if (Flag == 2)
                        cond = " where ClientInformation.value_date >= '" + Session["clientinfofdate"] + "' and ClientInformation.value_date <= '" + Session["clientinfotdate"] + "' " + " and ClientInformation.fund_id=" + Session["clientinfofundid"];
                    else if (Flag == 3)
                    {
                        if (VM.Code == "" || VM.Code == null)
                            cond = " where CustomerSearch.entry_date >= '" + Session["clientinfofdate"] + "' and CustomerSearch.entry_date <= '" + Session["clientinfotdate"] + "' ";
                        else
                            cond = " where CustomerSearch.entry_date >= '" + Session["clientinfofdate"] + "' and CustomerSearch.entry_date <= '" + Session["clientinfotdate"] + "' " + " and CustomerSearch.code='" + Session["clientinfofundid"] + "'";
                    }

                }
                else
                {
                    if (Flag == 1)
                        cond = " where ClientInformation.value_date >= '" + Session["clientinfofdate"] + "' and ClientInformation.value_date <= '" + Session["clientinfotdate"] + "' and ClientInformation.CustTypeId=" + Session["clientinfotype"] + " and ClientInformation.fund_id=" + Session["clientinfofundid"] + " ";
                    else if (Flag == 2)
                        cond = " where ClientInformation.value_date >= '" + Session["clientinfofdate"] + "' and ClientInformation.value_date <= '" + Session["clientinfotdate"] + "' and ClientInformation.CustTypeId=" + Session["clientinfotype"] + " " + " and ClientInformation.fund_id=" + Session["clientinfofundid"];
                    else if (Flag == 3)
                        cond = " where ClientInformation.value_date >= '" + Session["clientinfofdate"] + "' and ClientInformation.value_date <= '" + Session["clientinfotdate"] + "' and ClientInformation.CustTypeId=" + Session["clientinfotype"] + " " + " and ClientInformation.branch_id=" + Session["branchno4"] + " and ClientInformation.fund_id=" + Session["clientinfofundid"];
                }
            }

            Session["userid20"] = cond;
            var repDS = SqlHelper.ExecuteDataset(SqlCon, System.Data.CommandType.Text, "SELECT * from CustomerSearch " + Session["userid20"]);
            Session["repDS20"] = repDS;
            repDS = SqlHelper.ExecuteDataset(SqlCon, System.Data.CommandType.Text, "SELECT * from CustomerSearch  " + Session["userid20"]);
            string t1, Icbo_type, cbo_type = "0";
            for (int z = 0; z <= repDS.Tables[0].Rows.Count - 1; z++)

            {
                Data.Add(new ClientGroupinfoData
                {
                    aname = repDS.Tables[0].Rows[z].ItemArray[0].ToString(),
                    aaddress1 = repDS.Tables[0].Rows[z].ItemArray[10].ToString(),
                    birthdate = repDS.Tables[0].Rows[z].ItemArray[12].ToString(),
                    branch_id = repDS.Tables[0].Rows[z].ItemArray[16].ToString(),
                    code = repDS.Tables[0].Rows[z].ItemArray[3].ToString(),
                    eaddress1 = repDS.Tables[0].Rows[z].ItemArray[9].ToString(),
                    ename = repDS.Tables[0].Rows[z].ItemArray[1].ToString()
                });
            }
            return Data;
        }
    }
}

