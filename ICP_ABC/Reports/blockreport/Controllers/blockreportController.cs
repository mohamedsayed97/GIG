using Microsoft.ApplicationBlocks.Data;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ICP_ABC.Models;
using ICP_ABC.Reports.blockreport.Model;
using Microsoft.AspNet.Identity;
using System.Web.Configuration;
namespace ICP_ABC.Reports.blockreport.Controllers
{
    public class blockreportController : Controller
    {
        static string servername = WebConfigurationManager.AppSettings["servername"];


        static string DataBaseName = WebConfigurationManager.AppSettings["DataBaseName"];
        private string SqlCon = "data source=" + servername + ";initial catalog=" + DataBaseName + ";persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework";

        //private string SqlCon = "data source=" + servername + ";initial catalog=LastUpdateDB;persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework";
        ApplicationDbContext _db = new ApplicationDbContext();

            DataSet dataSet = new DataSet();
            //Context _db = new Context();
           // private string SqlCon = "data source=SERVER2;initial catalog=IC_AWB2;persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework";
            // GET: blockreport
            public ActionResult Index()
            {
                var blockreportVM = new blockreportVM
                {
                    Funds = _db.Funds.ToList()
                };

                return View(blockreportVM);
            }
            public ActionResult blockreport(blockreportVM VM)
            {
                var Data = GetData(VM);

                LocalReport localReport = new LocalReport();
                localReport.ReportPath = Server.MapPath("~/Reports/blockreport/blockreportReport.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource();
                reportDataSource.Name = "DataSet1";
                reportDataSource.Value = Data;
                localReport.DataSources.Add(reportDataSource);
                List<ReportParameter> parameters = new List<ReportParameter>();
                //DateTime FDate = (DateTime)Session["transfundfdate"];
                //DateTime TDate = (DateTime)Session["transfundtdate"];
                //parameters.Add(new ReportParameter("FDate", Session["clientinfofdate"].ToString()));
                //parameters.Add(new ReportParameter("TDate", Session["clientinfotdate"].ToString()));
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
            private List<blockreportData> GetData(blockreportVM VM)
            {
                var Data = new List<blockreportData>();
                var Flag = 0;
                var cond = "";

            var this_User_id = User.Identity.GetUserId();
            var this_User_data = _db.Users.Where(x => x.Id == this_User_id).FirstOrDefault();
            Session["groupid"] = this_User_data.GroupId;
            Session["userno"] = this_User_data.Code;

            //Session["groupid"] = 5;
            //Session["userno"] = 259;
            var Fund_Name = VM.Fund;
                try
                {
                    Session["repno"] = 11;
                    var druser = SqlHelper.ExecuteDataset(SqlCon, "Sp_druser", Convert.ToInt32(Session["userno"]));
                    var branch_id = druser.Tables[0].Rows[0].ItemArray[0];
                    var branch_right = druser.Tables[0].Rows[0].ItemArray[1];

                    if (Convert.ToBoolean(branch_right) == true)
                    {
                        Session["branchno1"] = 0;
                        Session["Bankno1"] = 1;
                        Flag = 2;
                    }
                    else
                    {
                        Session["branchno1"] = branch_id;
                        Session["Bankno1"] = 1;
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
                    Session["blockfund"] = fundid;
                    if (VM.Code == null)
                    {
                        VM.Code = "";
                    }
                    if (VM.Code == "" || VM.Code == null)
                    {
                        Session["blockcust"] = "";
                    }
                    else
                    {
                        Session["blockcust"] = VM.Code;
                    }
                    if (Convert.ToInt32(Session["blockfund"]) == 0)
                    {
                        if (Flag == 1)
                        {
                            if (Session["blockcust"].ToString() == "")
                                cond = " ";
                            else
                                cond = " and cust_id='" + Session["blockcust"] + "' ";
                        }
                        else if (Flag == 2)
                        {
                            if (Session["blockcust"].ToString() == "")
                                cond = " ";
                            else
                                cond = " and cust_id='" + Session["blockcust"] + "'";
                        }
                        else if (Flag == 3)
                        {
                            if (Session["blockcust"].ToString() == "")
                                cond = " ";
                            else
                                cond = " and cust_id='" + Session["blockcust"] + "' ";
                        }

                    }
                    else
                    {
                        if (Flag == 1)
                        {
                            if (Session["blockcust"].ToString() == "")
                                cond = " ";
                            else
                                cond = " and cust_id='" + Session["blockcust"] + "' and fund_id=" + Session["blockfund"];
                        }
                        else if (Flag == 2)
                        {
                            if (Session["blockcust"].ToString() == "")
                                cond = " ";
                            else
                                cond = " and cust_id='" + Session["blockcust"] + "' and fund_id=" + Session["blockfund"];
                        }
                        else if (Flag == 3)
                        {
                            if (Session["blockcust"].ToString() == "")
                                cond = " ";
                            else
                                cond = " and cust_id='" + Session["blockcust"] + "' and fund_id=" + Session["blockfund"];
                        }
                    }
                    Flag = 0;
                    Session["userid11"] = cond;

                }
                catch (Exception)
                {

                    throw;
                }
                var FundDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "SELECT distinct fund_id,fname from reportview where pur_sal=0 and ReportView.auth=1 " + Session["userid11"]);
                for (int j = 0; j <= FundDS.Tables[0].Rows.Count - 1; j++)
                {
                    var Fname = FundDS.Tables[0].Rows[j].ItemArray[1];
                    var repDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select Sum (total_sub) - isnull(Sum (total_red),0) as Total_SR,a.cust_id,a.fund_id from (select sum(quantity) as total_sub,fund_id,cust_id from reportview where pur_sal=0 and ReportView.auth=1 " + Session["userid11"] + " and fund_id=" + FundDS.Tables[0].Rows[j].ItemArray[0] + " group by fund_id,cust_id) a full outer join (select sum(quantity) as total_red ,fund_id,cust_id from reportview where pur_sal=1 and ReportView.auth=1 " + Session["userid11"] + " and fund_id=" + FundDS.Tables[0].Rows[j].ItemArray[0] + " group by fund_id,cust_id)b on a.cust_id=b.cust_id group by a.cust_id,a.Fund_id ");
                    if (repDS.Tables[0].Rows.Count == 0)
                    {
                        repDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select sum(quantity) as Total_SR,fund_id,cust_id from reportview where pur_sal=0 and ReportView.auth=1 " + Session["userid11"] + " and fund_id=" + FundDS.Tables[0].Rows[j].ItemArray[0] + " group by fund_id,cust_id");
                    }
                    string ename = "";
                    string cust_id = "";
                    string totalSR = "";
                    string totalBuB = "";
                    string totalPos = "";
                    for (int i = 0; i <= repDS.Tables[0].Rows.Count - 1; i++)
                    {
                        if (repDS.Tables[0].Rows.Count != 0)
                        {
                            cust_id = repDS.Tables[0].Rows[i].ItemArray[1].ToString();
                            totalSR = repDS.Tables[0].Rows[i].ItemArray[0].ToString();
                            var repDS1 = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select Sum (total_block) - isnull(Sum (total_unblock),0) as Total_Block,a.cust_id from (select sum(qty_Block) as total_block,cust_id from Block where BlockCmb=0 and Block.Blockauth=1  " + Session["userid11"] + " and fund_id=" + FundDS.Tables[0].Rows[j].ItemArray[0] + "  group by cust_id) a full outer join (select sum(qty_Block) as total_unblock ,cust_id from Block where BlockCmb=1 and Block.Blockauth=1 " + Session["userid11"] + " and fund_id=" + FundDS.Tables[0].Rows[j].ItemArray[0] + "  group by cust_id)b on a.cust_id=b.cust_id group by a.cust_id");
                            if (repDS1.Tables[0].Rows.Count != 0)
                            {
                                totalBuB = repDS1.Tables[0].Rows[i].ItemArray[0].ToString(); ;
                                totalPos = (Convert.ToDouble(repDS.Tables[0].Rows[0].ItemArray[0]) - Convert.ToDouble(repDS1.Tables[0].Rows[0].ItemArray[0])).ToString();
                            }
                            else
                            {
                                totalBuB = "0";
                                totalPos = repDS.Tables[0].Rows[0].ItemArray[0].ToString();
                            }
                        }
                        else
                        {
                            cust_id = "";
                            ename = "";
                            totalSR = "";
                            totalBuB = "";
                            totalPos = "";
                        }
                        if (cust_id != "")
                        {
                            var nDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select EnName from customer where code='" + cust_id + "'");
                            ename = nDS.Tables[0].Rows[0].ItemArray[0].ToString();
                        }
                        else
                        {
                            ename = "";
                        }
                        Data.Add(new blockreportData
                        {
                            cust_id = cust_id,
                            ename = ename,
                            fname = Fname.ToString(),
                            totalBuB = totalBuB,
                            totalPos = totalPos,
                            totalSR = totalSR
                        });
                    }

                }
                Session["repDS11"] = null;
                return Data;
            }
        }
    
}