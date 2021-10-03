using Microsoft.ApplicationBlocks.Data;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ICP_ABC.Reports.Rpt_CLientMapV8.Model;
using System.Web.Configuration;
namespace ICP_ABC.Reports.Rpt_CLientMapV8.Controllers
{
    public class Rpt_CLientMapV8Controller : Controller
    {
       static string servername = WebConfigurationManager.AppSettings["servername"];
        static string DataBaseName = WebConfigurationManager.AppSettings["DataBaseName"];
        private string SqlCon = "data source=" + servername + ";initial catalog="+ DataBaseName + ";persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework";

        //private string SqlCon = "data source=SERVER2;initial catalog=IC_AWB2;persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework";
        // GET: Rpt_CLientMapV8
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Rpt_CLientMapV8(Rpt_CLientMapV8VM VM)
        {
            var Data = GetData(VM);
            //var Rpt_CLientMapV8VMData = Session["Rpt_CLientMapV8VMData"];
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/Rpt_CLientMapV8/Rpt_CLientMapV8Report.rdlc");
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

        public List<Rpt_CLientMapV8Data> GetData(Rpt_CLientMapV8VM VM)
        {
            var Data = new List<Rpt_CLientMapV8Data>();
            try
            {
                Session["repno"] = 8;
                Session["navfdate"] = VM.FromDate.Value.ToString("yyyy-MM-dd");
                Session["navtdate"] = VM.ToDate.Value.ToString("yyyy-MM-dd");
                var cond = "";
                cond = " where trans.value_date >= '" + Session["navfdate"] + "' and trans.value_date <= '" + Session["navtdate"] + "' and cust_id not in ( select cust_id from trans where trans.value_date < '" + Session["navfdate"] + "') group by ICproCID,CoreCID,enname  order by StartDate ";
                int Flag = 0;
                Session["userid8"] = cond;
            }
            catch (Exception)
            {

            }
            var repDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select distinct ICproCID, CoreCID,enname, min(value_date)as StartDate from client_CodeMap inner join customer on client_CodeMap.ICproCID= Customer.code  inner join  trans on trans.cust_id=customer.code " + Session["userid8"] + Session["CondFund5"]);
            for (int i = 0; i <= repDS.Tables[0].Rows.Count - 1; i++)
            {
                Data.Add(new Rpt_CLientMapV8Data
                {
                    ICproCID = repDS.Tables[0].Rows[i].ItemArray[0].ToString(),
                    CoreCID = repDS.Tables[0].Rows[i].ItemArray[1].ToString(),
                    ename = repDS.Tables[0].Rows[i].ItemArray[2].ToString(),
                    StartDate = Convert.ToDateTime(repDS.Tables[0].Rows[i].ItemArray[3])
                });
            }
            return Data;
        }
    }
}