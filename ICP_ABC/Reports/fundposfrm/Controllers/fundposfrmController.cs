using Microsoft.ApplicationBlocks.Data;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ICP_ABC.Models;
using ICP_ABC.Reports.fundposfrm.Model;
using Microsoft.AspNet.Identity;
using System.Web.Configuration;

namespace ICP_ABC.Reports.fundposfrm.Controllers
{
    public class fundposfrmController : Controller
    {
        static string servername = WebConfigurationManager.AppSettings["servername"];
        static string DataBaseName = WebConfigurationManager.AppSettings["DataBaseName"];
      
        private string SqlCon = "data source=" + servername + ";initial catalog=" + DataBaseName + ";persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework";
        ApplicationDbContext _db = new ApplicationDbContext();
        DataSet dataSet = new DataSet();
        //Context _db = new Context();
        // GET: fundposfrm
        public ActionResult Index()
        {
            var fundposfrmVM = new fundposfrmVM
            {
                Funds = _db.Funds.ToList(),
                Branches = _db.Branches.ToList()
            };

            return View(fundposfrmVM);
        }

        public ActionResult fundposfrm(fundposfrmVM VM)
        {
            var Data = GetData(VM);

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/fundposfrm/fundposfrmreport.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = Data;
            localReport.DataSources.Add(reportDataSource);
            List<ReportParameter> parameters = new List<ReportParameter>();
            //DateTime FDate = (DateTime)Session["transfundfdate"];
            //DateTime TDate = (DateTime)Session["transfundtdate"];
            parameters.Add(new ReportParameter("FDate", Session["fundposfdate"].ToString()));
            parameters.Add(new ReportParameter("TDate", Session["fundpostdate"].ToString()));
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

        public List<fundposfrmData> GetData(fundposfrmVM VM)
        {
            List<fundposfrmData> Data = new List<fundposfrmData>();
            var cond = "";
            var cond1 = "";
            int Flag = 0;
            string t1 = "0";
            string t2 = "0";
            string t3 = "0";
            string t4 = "0";
            string noShares = "";
            string navToday = "";

            var this_User_id = User.Identity.GetUserId();
            var this_User_data = _db.Users.Where(x => x.Id == this_User_id).FirstOrDefault();
            Session["groupid"] = this_User_data.GroupId;
            Session["userno"] = this_User_data.Code;

            //Session["groupid"] = 5;
            //Session["userno"] = 259;
            Session["repno"] = 22;
            Session["fundposfdate"] = VM.FromDate.Value.ToString("yyyy-MM-dd");
            Session["fundpostdate"] = VM.ToDate.Value.ToString("yyyy-MM-dd");
            var Fund_Name = VM.Fund;
            var Branch_Name = VM.Branche;
            int fundid;
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
            decimal nav;
            if (Convert.ToBoolean(Branch_Name) == false)
            {
                Session["branchno8"] = 0;
                Flag = 1;
            }
            else
            {
                Session["branchno8"] = Branch_Name;

                Flag = 3;
            }
            String str = "select Price from ICPrice where ICdate=  { fn CURDATE() }";
            nav = Returndecimal(str);
            Session["fundposfund"] = fundid;
            Session["fundposnav"] = nav;
            if (Convert.ToInt32(Session["fundposfund"]) == 0)
            {
                if (Flag == 1)
                    cond = " where ReportView.value_date >= '" + Session["fundposfdate"] + "' and ReportView.value_date <= '" + Session["fundpostdate"] + "' ";
                else if (Flag == 2)
                    cond = " where ReportView.value_date >= '" + Session["fundposfdate"] + "' and ReportView.value_date <= '" + Session["fundpostdate"] + "'";
                else if (Flag == 3)
                    cond = " where ReportView.value_date >= '" + Session["fundposfdate"] + "' and ReportView.value_date <= '" + Session["fundpostdate"] + "' " + " and ReportView.branch_id=" + Session["branchno8"];
            }
            else
            {
                if (Flag == 1)
                    cond = " where ReportView.value_date >= '" + Session["fundposfdate"] + "' and ReportView.value_date <= '" + Session["fundpostdate"] + "' and ReportView.fund_id=" + Session["fundposfund"] + " ";
                else if (Flag == 2)
                    cond = " where ReportView.value_date >= '" + Session["fundposfdate"] + "' and ReportView.value_date <= '" + Session["fundpostdate"] + "' and ReportView.fund_id=" + Session["fundposfund"] + " ";
                else if (Flag == 3)
                    cond = " where ReportView.value_date >= '" + Session["fundposfdate"] + "' and ReportView.value_date <= '" + Session["fundpostdate"] + "' and ReportView.fund_id=" + Session["fundposfund"] + " " + " and ReportView.branch_id=" + Session["branchno8"];
            }
            Flag = 0;
            var FundRight = SqlHelper.ExecuteDataset(SqlCon, "Sp_FundRight1", Session["groupid"]);
            for (int i = 0; i <= FundRight.Tables[0].Rows.Count - 1; i++)
            {
                cond1 = "  ReportView.fund_id=" +Convert.ToInt32(FundRight.Tables[0].Rows[i].ItemArray[2]) + " or " + cond1;
            }
            var xx = 0;
            xx = cond1.Length;
            cond1 = Microsoft.VisualBasic.Strings.Left(cond1, xx - 3);
            cond1 = " and (" + cond1 + ")";
            Session["CondFund4"] = cond1;
            Session["userid22"] = cond;
            var FundDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select distinct ReportView.fund_id, fname from ReportView " + Session["userid22"] + Session["CondFund4"]);
            var fname = "";
            for (int j = 0; j <= FundDS.Tables[0].Rows.Count - 1; j++)
            {
                string t11 = "0";
                string t22 = "0";
                string t33 = "0";
                string t44 = "0";
                fname = FundDS.Tables[0].Rows[j].ItemArray[1].ToString();
                var Price_DS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select Icprice.Price from Icprice where Icprice.icdate=(select max(icdate) from icprice where FundId=" + FundDS.Tables[0].Rows[j].ItemArray[0] + ") and FundId=" + FundDS.Tables[0].Rows[j].ItemArray[0]);
                var Price = Price_DS.Tables[0].Rows[0].ItemArray[0];
                var brchDs = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select distinct branch_id,BName from reportview" + Session["userid22"] + " and fund_id=" + FundDS.Tables[0].Rows[j].ItemArray[0] + " order by branch_id");
                for (int n = 0; n <= brchDs.Tables[0].Rows.Count - 1; n++)
                {
                    var repDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select distinct (isnull(reportview.cust_id,''))as cust_id,isnull(branch_id,'')as branch_id,isnull(bname,'') as bname,isnull(reportview.ename,'')as ename,isnull(a.total_sub,0) as total_sub,isnull(b.total_red,0)as total_red,isnull(a.total_sub,0) - isnull(b.total_red,0) as tot,(isnull(a.total_sub,0) - isnull(b.total_red,0)) * " + Price + " as nav from (select sum(quantity)as total_sub,cust_id  from ReportView " + Session["userid22"] + " and pur_sal=0 and fund_id=" + FundDS.Tables[0].Rows[j].ItemArray[0] + " and branch_id=" + brchDs.Tables[0].Rows[n].ItemArray[0] + " group by cust_id  )a full outer join (select sum(quantity)as total_red,cust_id from ReportView " + Session["userid22"] + " and pur_sal=1 and fund_id=" + FundDS.Tables[0].Rows[j].ItemArray[0] + "   and branch_id=" + brchDs.Tables[0].Rows[n].ItemArray[0] + " group by cust_id)b on a.cust_id=b.cust_id full outer join reportview on a.cust_id=reportview.cust_id or b.cust_id=reportview.cust_id " + Session["userid22"] + " and fund_id=" + FundDS.Tables[0].Rows[j].ItemArray[0] + "  and branch_id=" + brchDs.Tables[0].Rows[n].ItemArray[0]);
                    t1 = "0";
                    t2 = "0";
                    t3 = "0";
                    t4 = "0";
                    for (int z = 0; z <= repDS.Tables[0].Rows.Count - 1; z++)
                    {
                        noShares = (Convert.ToDouble(repDS.Tables[0].Rows[z].ItemArray[4]) - Convert.ToDouble(repDS.Tables[0].Rows[z].ItemArray[5])).ToString();
                        navToday = (Convert.ToDouble(Price) * Convert.ToDouble(noShares)).ToString();
                        t1 = (Convert.ToDouble(t1) + Convert.ToDouble(repDS.Tables[0].Rows[z].ItemArray[4])).ToString();
                        t2 = (Convert.ToDouble(t2) + Convert.ToDouble(repDS.Tables[0].Rows[z].ItemArray[5])).ToString();
                        t3 = (Convert.ToDouble(t3) + Convert.ToDouble(noShares)).ToString();
                        t4 = (Convert.ToDouble(t4) + Convert.ToDouble(navToday)).ToString();

                        t11 = (Convert.ToDouble(t11) + Convert.ToDouble(repDS.Tables[0].Rows[z].ItemArray[4])).ToString();
                        t22 = (Convert.ToDouble(t22) + Convert.ToDouble(repDS.Tables[0].Rows[z].ItemArray[5])).ToString();
                        t33 = (Convert.ToDouble(t33) + Convert.ToDouble(noShares)).ToString();
                        t44 = (Convert.ToDouble(t44) + Convert.ToDouble(navToday)).ToString();
                        Data.Add(new fundposfrmData
                        {
                            fname = fname,
                            price = Price.ToString(),
                            bname = brchDs.Tables[0].Rows[n].ItemArray[1].ToString(),
                            branch_id = brchDs.Tables[0].Rows[n].ItemArray[0].ToString(),
                            cust_id = repDS.Tables[0].Rows[z].ItemArray[0].ToString(),
                            ename = repDS.Tables[0].Rows[z].ItemArray[3].ToString(),
                            total_sub = Convert.ToDouble(repDS.Tables[0].Rows[z].ItemArray[4]),
                            total_red = Convert.ToDouble(repDS.Tables[0].Rows[z].ItemArray[5]),
                            noshares = Convert.ToDouble(noShares),
                            natoday = Convert.ToDouble(navToday)
                        });
                    }
                    t1 = "0";
                    t2 = "0";
                    t3 = "0";
                    t4 = "0";
                }
            }
            return Data.ToList();
        }
        public decimal Returndecimal(String SQL)
        {
            try
            {
                if (SQL != "" && SQL != null)
                {
                    SelectToDataSet(SQL);
                    return Convert.ToDecimal(dataSet.Tables[0].Rows[0].ItemArray[0]);
                }
                else
                    return -1;
            }
            catch (Exception)
            {

                return -1;
            }
        }

        public void SelectToDataSet(string selectStr, params object[] ParamArray)
        {
            dataSet.Tables.Clear();
            selectStr = selectStr.ToLower();
            string strselect, strfrom, strwhere;
            int intselect, intfrom, intwhere, readlen;
            intselect = selectStr.IndexOf("select");
            intfrom = selectStr.IndexOf("from");
            intwhere = selectStr.IndexOf("where");
            readlen = intfrom - intselect - 6;
            strselect = selectStr.Substring(intselect + 6, readlen);
            if (intwhere == -1)
                readlen = selectStr.Length - intfrom - 4;
            else
                readlen = intwhere - intfrom - 4;
            strfrom = selectStr.Substring(intfrom + 4, readlen);
            if (intwhere == -1)
                strwhere = "1=1";
            else
                readlen = selectStr.Length - intwhere - 5;
            strwhere = selectStr.Substring(intwhere + 5, readlen);
            try
            {
                strwhere = strwhere.ToUpper();
                dataSet = SqlHelper.ExecuteDataset("data source=" + servername + ";initial catalog=" + DataBaseName + ";persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework", "sp_returnfield", strselect, strfrom, strwhere);
            }
            catch (Exception)
            {

            }
        }

        public string ConvertDate(DateTime d)
        {
            int dd, mm, yy;
            dd = d.Day;
            mm = d.Month;
            yy = d.Year;
            var ndate = new DateTime(yy, mm, dd);
            return ndate.ToString("MM/dd/yyyy");
        }
    }
}
