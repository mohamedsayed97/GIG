using Microsoft.ApplicationBlocks.Data;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ICP_ABC.Models;
using ICP_ABC.Reports.TransByUser.Model;
//using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity;
using System.Web.Configuration;

namespace ICP_ABC.Reports.TransByUser.Controllers
{
    public class TransByUserController : Controller
    {
        static string servername = WebConfigurationManager.AppSettings["servername"];
        static string DataBaseName = WebConfigurationManager.AppSettings["DataBaseName"];
        private string SqlCon = "data source=" + servername + ";initial catalog=" + DataBaseName + ";persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework";
        ApplicationDbContext _db = new ApplicationDbContext();

        //Context _db = new Context();
        //private string SqlCon = "data source=SERVER2;initial catalog=IC_AWB2;persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework";
        DataSet dataSet = new DataSet();
       
        // GET: fundposfrm
        public ActionResult Index()
        {
            var TransByUserVM = new TransByUserVM
            {
                users = _db.User.ToList()
            };

            return View(TransByUserVM);
        }
        public ActionResult TransByUser(TransByUserVM VM)
        {
            var Data = GetData(VM);

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/TransByUser/TransByUserReport.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = Data;
            localReport.DataSources.Add(reportDataSource);
            List<ReportParameter> parameters = new List<ReportParameter>();
            //DateTime FDate = (DateTime)Session["transfundfdate"];
            //DateTime TDate = (DateTime)Session["transfundtdate"];
            parameters.Add(new ReportParameter("FDate", Session["transuserfdate"].ToString()));
            parameters.Add(new ReportParameter("TDate", Session["transusertdate"].ToString()));
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
        private List<TransByUserData> GetData(TransByUserVM VM)
        {
            var Data = new List<TransByUserData>();
            var cond = "";
            var cond1 = "";
            int Flag = 0;

            var this_User_id = User.Identity.GetUserId();
            var this_User_data = _db.Users.Where(x => x.Id == this_User_id).FirstOrDefault();
            Session["groupid"] = this_User_data.GroupId;
            Session["userno"] = this_User_data.Code;
            //Session["groupid"] = 9;
            //Session["userno"] = 11;
            Session["transuserfdate"] = VM.FromDate.Value.ToString("yyyy-MM-dd");
            Session["transusertdate"] = VM.ToDate.Value.ToString("yyyy-MM-dd");
            var druser = SqlHelper.ExecuteDataset(SqlCon, "Sp_druser", Convert.ToInt32(Session["userno"]));
            var branch_id = druser.Tables[0].Rows[0].ItemArray[0];
            var branch_right = druser.Tables[0].Rows[0].ItemArray[1];
            if (Convert.ToBoolean(branch_right) == true)
            {
                Session["branchno13"] = 0;
                Flag = 2;
            }
            else
            {
                Session["branchno13"] = Convert.ToInt32(branch_id);
                Flag = 3;
            }
            int transtype;
            if (VM.Auth == "1")
                transtype = 1;
            else if (VM.Auth == "2")
                transtype = 0;
            else
                transtype = 2;
            Session["repno"] = 16;
            var UserVM = VM.user;
            if (Convert.ToBoolean(UserVM) == false)
            {
                Session["transuserfundid"] = 0;
            }
            else
            {
                Session["transuserfundid"] = UserVM;
            }
            Session["transusertype"] = transtype;
            if (VM.TransactionOver == "" || VM.TransactionOver == null)
                Session["transuserover"] = 0;
            else
                Session["transuserover"] = Convert.ToDouble(VM.TransactionOver);
            if (Convert.ToInt32(Session["transuserover"]) == 0)
            {
                if (Convert.ToInt32(Session["transuserfundid"]) == 0)
                {
                    if (Convert.ToInt32(Session["transusertype"]) == 0 || Convert.ToInt32(Session["transusertype"]) == 1)
                    {
                        if (Flag == 1)
                            cond = " where TransByUser.value_date >= '" + Session["transuserfdate"] + "' and TransByUser.value_date <= '" + Session["transusertdate"] + "' and TransByUser.auth= " + Session["transusertype"] + "";
                        else if (Flag == 2)
                            cond = " where TransByUser.value_date >= '" + Session["transuserfdate"] + "' and TransByUser.value_date <= '" + Session["transusertdate"] + "' " + " and TransByUser.auth= " + Session["transusertype"];
                        else if (Flag == 3)
                            cond = " where TransByUser.value_date >= '" + Session["transuserfdate"] + "' and TransByUser.value_date <= '" + Session["transusertdate"] + "' " + " and TransByUser.branch_id=" + Session["branchno13"] + " and TransByUser.auth= " + Session["transusertype"];
                    }
                    else
                    {
                        if (Flag == 1)
                            cond = " where TransByUser.value_date >= '" + Session["transuserfdate"] + "' and TransByUser.value_date <= '" + Session["transusertdate"] + "' ";
                        else if (Flag == 2)
                            cond = " where TransByUser.value_date >= '" + Session["transuserfdate"] + "' and TransByUser.value_date <= '" + Session["transusertdate"] + "' ";
                        else if (Flag == 3)
                            cond = " where TransByUser.value_date >= '" + Session["transuserfdate"] + "' and TransByUser.value_date <= '" + Session["transusertdate"] + "' " + " and TransByUser.branch_id=" + Session["branchno13"];
                    }
                }
                else
                {
                    if (Convert.ToInt32(Session["transusertype"]) == 0 || Convert.ToInt32(Session["transusertype"]) == 1)
                    {
                        if (Flag == 1)
                            cond = " where TransByUser.value_date >= '" + Session["transuserfdate"] + "' and TransByUser.value_date <= '" + Session["transusertdate"] + "' and TransByUser.auth= " + Session["transusertype"] + " and TransByUser.user_id=" + Session["transuserfundid"] + " ";
                        else if (Flag == 2)
                            cond = " where TransByUser.value_date >= '" + Session["transuserfdate"] + "' and TransByUser.value_date <= '" + Session["transusertdate"] + "' " + " and TransByUser.auth= " + Session["transusertype"] + " and TransByUser.user_id=" + Session["transuserfundid"];
                        else if (Flag == 3)
                            cond = " where TransByUser.value_date >= '" + Session["transuserfdate"] + "' and TransByUser.value_date <= '" + Session["transusertdate"] + "' " + " and TransByUser.branch_id=" + Session["branchno13"] + " and TransByUser.auth= " + Session["transusertype"] + " and TransByUser.user_id=" + Session["transuserfundid"];
                    }
                    else
                    {
                        if (Flag == 1)
                            cond = " where TransByUser.value_date >= '" + Session["transuserfdate"] + "' and TransByUser.value_date <= '" + Session["transusertdate"] + "' and TransByUser.user_id=" + Session["transuserfundid"] + "  ";
                        else if (Flag == 2)
                            cond = " where TransByUser.value_date >= '" + Session["transuserfdate"] + "' and TransByUser.value_date <= '" + Session["transusertdate"] + "' " + " and TransByUser.user_id=" + Session["transuserfundid"];
                        else if (Flag == 3)
                            cond = " where TransByUser.value_date >= '" + Session["transuserfdate"] + "' and TransByUser.value_date <= '" + Session["transusertdate"] + "' " + " and TransByUser.branch_id=" + Session["branchno13"] + " and TransByUser.user_id=" + Session["transuserfundid"];
                    }
                }
            }
            else
            {
                if (Convert.ToInt32(Session["transuserfundid"]) == 0)
                {
                    if (Convert.ToInt32(Session["transusertype"]) == 0 || Convert.ToInt32(Session["transusertype"]) == 1)
                    {
                        if (Flag == 1)
                            cond = " where TransByUser.value_date >= '" + Session["transuserfdate"] + "' and TransByUser.value_date <= '" + Session["transusertdate"] + "' and TransByUser.total_value >=" + Session["transuserover"] + " and TransByUser.auth= " + Session["transusertype"] + "  ";
                        else if (Flag == 2)
                            cond = " where TransByUser.value_date >= '" + Session["transuserfdate"] + "' and TransByUser.value_date <= '" + Session["transusertdate"] + "' " + " and TransByUser.total_value >=" + Session["transuserover"] + " and TransByUser.auth= " + Session["transusertype"];
                        else if (Flag == 3)
                            cond = " where TransByUser.value_date >= '" + Session["transuserfdate"] + "' and TransByUser.value_date <= '" + Session["transusertdate"] + "' " + " and TransByUser.branch_id=" + Session["branchno13"] + " and TransByUser.total_value >=" + Session["transuserover"] + " and TransByUser.auth= " + Session["transusertype"];
                    }
                    else
                    {
                        if (Flag == 1)
                            cond = " where TransByUser.value_date >= '" + Session["transuserfdate"] + "' and TransByUser.value_date <= '" + Session["transusertdate"] + "' and TransByUser.total_value >=" + Session["transuserover"] + "  ";
                        else if (Flag == 2)
                            cond = " where TransByUser.value_date >= '" + Session["transuserfdate"] + "' and TransByUser.value_date <= '" + Session["transusertdate"] + "' " + " and TransByUser.total_value >=" + Session["transuserover"];
                        else if (Flag == 3)
                            cond = " where TransByUser.value_date >= '" + Session["transuserfdate"] + "' and TransByUser.value_date <= '" + Session["transusertdate"] + "' " + " and TransByUser.branch_id=" + Session["branchno13"] + " and TransByUser.total_value >=" + Session["transuserover"];
                    }
                }
                else
                {
                    if (Convert.ToInt32(Session["transusertype"]) == 0 || Convert.ToInt32(Session["transusertype"]) == 1)
                    {
                        if (Flag == 1)
                            cond = " where TransByUser.value_date >= '" + Session["transuserfdate"] + "' and TransByUser.value_date <= '" + Session["transusertdate"] + "' and TransByUser.total_value >=" + Session["transuserover"] + " and TransByUser.auth= " + Session["transusertype"] + " and TransByUser.user_id=" + Session["transuserfundid"] + "  ";
                        else if (Flag == 2)
                            cond = " where TransByUser.value_date >= '" + Session["transuserfdate"] + "' and TransByUser.value_date <= '" + Session["transusertdate"] + "' " + " and TransByUser.total_value >=" + Session["transuserover"] + " and TransByUser.auth= " + Session["transusertype"] + " and TransByUser.user_id=" + Session["transuserfundid"];
                        else if (Flag == 3)
                            cond = " where TransByUser.value_date >= '" + Session["transuserfdate"] + "' and TransByUser.value_date <= '" + Session["transusertdate"] + "' " + " and TransByUser.branch_id=" + Session["branchno13"] + " and TransByUser.total_value >=" + Session["transuserover"] + " and TransByUser.auth= " + Session["transusertype"] + " and TransByUser.user_id=" + Session["transuserfundid"];
                    }
                    else
                    {
                        if (Flag == 1)
                            cond = " where TransByUser.value_date >= '" + Session["transuserfdate"] + "' and TransByUser.value_date <= '" + Session["transusertdate"] + "' and TransByUser.total_value >=" + Session["transuserover"] + " and TransByUser.user_id=" + Session["transuserfundid"] + " ";
                        else if (Flag == 2)
                            cond = " where TransByUser.value_date >= '" + Session["transuserfdate"] + "' and TransByUser.value_date <= '" + Session["transusertdate"] + "' " + " and TransByUser.total_value >=" + Session["transuserover"] + " and TransByUser.user_id=" + Session["transuserfundid"];
                        else if (Flag == 3)
                            cond = " where TransByUser.value_date >= '" + Session["transuserfdate"] + "' and TransByUser.value_date <= '" + Session["transusertdate"] + "' " + " and TransByUser.branch_id=" + Session["branchno13"] + " and TransByUser.total_value >=" + Session["transuserover"] + " and TransByUser.user_id=" + Session["transuserfundid"];
                    }
                }
            }
            Flag = 0;
            Session["userid16"] = cond;
            var repDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "SELECT * from TransByUser " + Session["userid16"]);
            Session["repDS16"] = repDS;
            string sub_red = "0";
            string totalnav = "0";
            repDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "SELECT distinct isnull(user_id,'') as user_id,isnull(name,'') as name,isnull(groupname,'') as groupName,isnull(bname,'') as bname from TransByUser " + Session["userid16"]);
            for (int i = 0; i <= repDS.Tables[0].Rows.Count - 1; i++)
            {
                var FundDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select  distinct fund_id, fname  from TransByUser " + Session["userid16"]);
                for (int z = 0; z <= FundDS.Tables[0].Rows.Count - 1; z++)
                {
                    var DS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "SELECT isnull(transid,'') as transid,isnull(ename,'') as ename,isnull(cust_id,'') as cust_id,isnull(quantity,0) as quantity,isnull(pur_sal,'') as pur_sal,isnull(total,0) as total,isnull(inputer,'') as inputer,isnull(auther,'') as auther from TransByUser  " + Session["userid16"] + " and user_id=" + repDS.Tables[0].Rows[i].ItemArray[0] + " and fund_id= " + FundDS.Tables[0].Rows[z].ItemArray[0]);
                    if (DS.Tables[0].Rows.Count != 0)
                    {
                        var Price_DS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select Icprice.Price from Icprice where Icprice.icdate=(select max(icdate) from icprice where fund_id=" + FundDS.Tables[0].Rows[z].ItemArray[0] + ") and fund_id=" + FundDS.Tables[0].Rows[z].ItemArray[0]);
                        var Price = Price_DS.Tables[0].Rows[0].ItemArray[0];
                        for (int j = 0; j <= DS.Tables[0].Rows.Count - 1; j++)
                        {
                            if (Convert.ToInt32(DS.Tables[0].Rows[j].ItemArray[0]) == 0)
                                sub_red = DS.Tables[0].Rows[j].ItemArray[0].ToString();
                            else
                                sub_red = "-" + DS.Tables[0].Rows[j].ItemArray[0].ToString();
                            totalnav = (Convert.ToDouble(sub_red) * Convert.ToDouble(Price)).ToString();
                            Data.Add(new TransByUserData
                            {
                                name = repDS.Tables[0].Rows[i].ItemArray[1].ToString(),
                                user_id = repDS.Tables[0].Rows[i].ItemArray[0].ToString(),
                                groupname = repDS.Tables[0].Rows[i].ItemArray[2].ToString(),
                                bname = repDS.Tables[0].Rows[i].ItemArray[3].ToString(),
                                Fname = FundDS.Tables[0].Rows[z].ItemArray[1].ToString(),
                                Price = Price.ToString(),
                                transid = DS.Tables[0].Rows[j].ItemArray[0].ToString(),
                                ename = DS.Tables[0].Rows[j].ItemArray[1].ToString(),
                                cust_id = DS.Tables[0].Rows[j].ItemArray[2].ToString(),
                                sub_red = sub_red,
                                total = DS.Tables[0].Rows[j].ItemArray[5].ToString(),
                                totalnav = totalnav,
                                inputer = DS.Tables[0].Rows[j].ItemArray[6].ToString(),
                                auther = DS.Tables[0].Rows[j].ItemArray[7].ToString()
                            });
                        }
                    }
                }
            }
            return Data;
        }
    }
}