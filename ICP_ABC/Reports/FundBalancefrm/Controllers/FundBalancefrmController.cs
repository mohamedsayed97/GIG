using Microsoft.ApplicationBlocks.Data;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ICP_ABC.Models;
using ICP_ABC.Reports.FundBalancefrm.Model;
using Microsoft.AspNet.Identity;
using System.Web.Configuration;
namespace ICP_ABC.Reports.FundBalancefrm.Controllers
{
    public class FundBalancefrmController : Controller
    {
        static string servername = WebConfigurationManager.AppSettings["servername"];


        DataSet dataSet = new DataSet();
        static string DataBaseName = WebConfigurationManager.AppSettings["DataBaseName"];
        private string SqlCon = "data source=" + servername + ";initial catalog=" + DataBaseName + ";persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework";

        //private string SqlCon = "data source=" + servername + ";initial catalog=LastUpdateDB;persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework";
        ApplicationDbContext _db = new ApplicationDbContext();

        //----------------

        public ActionResult Index()
        {
            var FundBallance = new FundBalancefrmVM
            {
                Funds = _db.Funds.ToList()
            };

            return View(FundBallance);
        }

        public ActionResult FundBalancefrm(FundBalancefrmVM VM)
        {
            var Data = TransactionByFundGetData(VM);

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/FundBalancefrm/FundBalancefrmReport.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = Data;
            localReport.DataSources.Add(reportDataSource);
            //DateTime FDate = (DateTime)Session["transfundfdate"];
            //DateTime TDate = (DateTime)Session["transfundtdate"];
            List<ReportParameter> parameters = new List<ReportParameter>();
            parameters.Add(new ReportParameter("FDate", Session["ref8fdate"].ToString()));
            parameters.Add(new ReportParameter("TDate", Session["ref8tdate"].ToString()));
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

        public List<FundBalancefrmData> TransactionByFundGetData(FundBalancefrmVM VM)
        {
            List<FundBalancefrmData> Data = new List<FundBalancefrmData>();
            string tot = "0";
            string nav = "0";
            string Stotal = "0";
            string Stotal1 = "0";
            string Rtotal = "0";
            string Rtotal1 = "0";
            string Sno = "0";
            string Sno1 = "0";
            string Rno = "0";
            string Rno1 = "0";
            string SRtot = "0";
            string SRtot1 = "0";
            string nav_all = "0";
            string nav_all1 = "0";

            var Fund_Name = VM.Fund;

            var this_User_id = User.Identity.GetUserId();
            var this_User_data = _db.Users.Where(x => x.Id == this_User_id).FirstOrDefault();

            Session["groupid"] = this_User_data.GroupId;
            Session["userno"] = this_User_data.Code;

            //Session["groupid"] = 5;
            //Session["userno"] = 259;
            Session["ref8fdate"] = VM.FromDate.Value.ToString("yyyy-MM-dd");
            Session["ref8tdate"] = VM.ToDate.Value.ToString("yyyy-MM-dd");
            string str1;
            string str2;
            var cond = "";
            var cond1 = "";
            var Flag = 0;
            var struser = "select branch_id,branch_right from users where code='" + Session["userno"] + "'";
            var druser = SqlHelper.ExecuteDataset(SqlCon, "Sp_druser", Convert.ToInt32(Session["userno"]));
            var branch_id = druser.Tables[0].Rows[0].ItemArray[0];
            var branch_right = druser.Tables[0].Rows[0].ItemArray[1];
            if (Convert.ToBoolean(branch_right) == true)
            {
                Session["branchno12"] = 0;
                Flag = 2;
            }
            else
            {
                Session["branchno12"] = Convert.ToInt32(branch_id);
                Flag = 3;
            }
            Session["Report"] = 0;
            Session["Bycust"] = 0;
            Session["repno"] = 7;
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
            DateTime? ref8tdate = Convert.ToDateTime(Session["ref8tdate"]);
            string sql = "select price from  icprice where icdate='" + ref8tdate.Value.ToString("dd-MM-yyyy") + "' and navauth=1";
            Session["price_todate"] = Returndecimal(sql);
            if (Convert.ToInt32(Session["Report"]) == 1)
            {
                if (Convert.ToInt32(Session["Bycust"]) == 0)
                {
                    str1 = "fund_id,fname,cust_id,branch_id,BName,quantity,nav_today,value_date,pur_sal,group_id,cbo_type";
                    str2 = " SELECT dbo.fund.code AS fund_id, dbo.fund.fname, trans.cust_id,trans.branch_id, { fn IFNULL(dbo.Branch.BName, 0) } AS BName, trans.quantity,  ISNULL(dbo.ICPrice.Price, 0) AS nav_today , trans.value_date ,trans.pur_sal ,dbo.fundright.group_id ,customer.cbo_type FROM (select fund_id ,branch_id ,quantity,pur_sal,value_date ,cust_id from trans where auth =1) trans INNER JOIN  dbo.fund ON trans.fund_id = dbo.fund.code LEFT OUTER JOIN  dbo.ICPrice ON dbo.fund.code = dbo.ICPrice.Fund_Id AND dbo.ICPrice.ICdate = { fn CURDATE() } INNER JOIN  dbo.Branch ON trans.branch_id = dbo.Branch.code INNER JOIN  dbo.customer ON trans.cust_id = dbo.customer.code INNER JOIN  dbo.fundright on trans.fund_id = dbo.fundright.fund_id ";
                    if (Convert.ToInt32(Session["ref8fund"]) == 0)
                    {
                        if (Flag == 1)
                        {
                            cond = " where value_date >= '" + Session["ref8fdate"] + "' and value_date <= '" + Session["ref8tdate"] + "'  ";
                        }
                        else if (Flag == 2)
                        {
                            cond = " where value_date >= '" + Session["ref8fdate"] + "' and value_date <= '" + Session["ref8tdate"] + "' ";
                        }
                        else if (Flag == 3)
                        {
                            cond = " where value_date >= '" + Session["ref8fdate"] + "' and value_date <= '" + Session["ref8tdate"] + "' " + " and trans.branch_id=" + Session["branchno7"];
                        }
                    }
                    else
                    {
                        if (Flag == 1)
                        {
                            cond = " where value_date >= '" + Session["ref8fdate"] + "' and value_date <= '" + Session["ref8tdate"] + "' and trans.fund_id=" + Session["ref8fund"] + "  ";
                        }
                        else if (Flag == 2)
                        {
                            cond = " where value_date >= '" + Session["ref8fdate"] + "' and value_date <= '" + Session["ref8tdate"] + "' " + " and trans.fund_id=" + Session["ref8fund"];
                        }
                        else if (Flag == 3)
                        {
                            cond = " where value_date >= '" + Session["ref8fdate"] + "' and value_date <= '" + Session["ref8tdate"] + "' " + " and trans.branch_id=" + Session["branchno7"] + " and trans.fund_id=" + Session["ref8fund"];
                        }
                    }
                }
                else
                {
                    str1 = "fund_id,fname,cust_id,BName,quantity,nav_today,value_date,pur_sal,group_id,cbo_type,branch_id";
                    str2 = " SELECT dbo.fund.code AS fund_id, dbo.fund.fname,  trans.cust_id, { fn IFNULL(dbo.Branch.BName, 0) } AS BName, trans.quantity, ISNULL(dbo.ICPrice.Price, 0) AS nav_today, trans.value_date, trans.pur_sal, dbo.fundright.group_id, dbo.Customer.cbo_type, dbo.Customer.branch_id FROM(SELECT fund_id, quantity, pur_sal, value_date,  cust_id FROM trans WHEREauth = 1) trans INNER JOIN dbo.fund ON trans.fund_id = dbo.fund.code LEFT OUTER JOIN dbo.ICPrice ON dbo.fund.code = dbo.ICPrice.Fund_Id AND dbo.ICPrice.ICdate = { fn CURDATE() } INNER JOIN dbo.Customer ON trans.cust_id = dbo.Customer.code INNER JOIN dbo.fundright ON trans.fund_id = dbo.fundright.fund_id INNER JOIN dbo.Branch ON dbo.Customer.branch_id = dbo.Branch.code ";
                    if (Convert.ToInt32(Session["ref8fund"]) == 0)
                    {
                        if (Flag == 1)
                        {
                            cond = " where value_date >= '" + Session["ref8fdate"] + "' and value_date <= '" + Session["ref8tdate"] + "' ";
                        }
                        else if (Flag == 2)
                        {
                            cond = " where value_date >= '" + Session["ref8fdate"] + "' and value_date <= '" + Session["ref8tdate"] + "' ";
                        }
                        else if (Flag == 3)
                        {
                            cond = " where value_date >= '" + Session["ref8fdate"] + "' and value_date <= '" + Session["ref8tdate"] + "' " + " and trans.branch_id=" + Session["branchno7"];
                        }
                    }
                    else
                    {
                        if (Flag == 1)
                        {
                            cond = " where value_date >= '" + Session["ref8fdate"] + "' and value_date <= '" + Session["ref8tdate"] + "' and trans.fund_id=" + Session["ref8fund"] + "  ";
                        }
                        else if (Flag == 2)
                        {
                            cond = " where value_date >= '" + Session["ref8fdate"] + "' and value_date <= '" + Session["ref8tdate"] + "' " + " and trans.fund_id=" + Session["ref8fund"];
                        }
                        else if (Flag == 3)
                        {
                            cond = " where value_date >= '" + Session["ref8fdate"] + "' and value_date <= '" + Session["ref8tdate"] + "' " + " and trans.branch_id=" + Session["branchno7"] + " and trans.fund_id=" + Session["ref8fund"];
                        }
                    }
                }
                try
                {
                    SqlHelper.ExecuteNonQuery(SqlCon, "Sp_InsertRptTableTemp", Session["userid7"], str1, str2, cond);
                }
                catch (Exception)
                {

                    ViewBag.Error = "The server is busy, Please try again.";
                }
                var chkDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select * from tempdb.dbo.a" + Session["userid7"]);
                if (chkDS.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Error = "The server is busy, Please try again.";
                }
                try
                {
                    string str3 = " nav_today ";
                    string str4 = " (select ISNULL(dbo.ICPrice.Price, 0) AS nav_today from icprice where icprice.fund_id=tempdb.dbo.A" + Session["userid7"] + ".fund_id AND dbo.ICPrice.ICdate = { fn CURDATE() }) , fname=(select fund.fname from fund where fund.code=tempdb.dbo.A" + Session["userid7"] + ".fund_id) , BName=(select Branch.BName from Branch where Branch.code=tempdb.dbo.A" + Session["userid7"] + ".branch_id)";
                    SqlHelper.ExecuteNonQuery(SqlCon, "Sp_UpdateRptTableTemp", Session["userid7"], str3, str4);
                }
                catch (Exception)
                {

                }
                var nav_todayDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "Select nav_today from tempdb.dbo.A" + Session["userid7"]);
                //                If IsDBNull(nav_todayDS.Tables(0).Rows(0)(0)) Then
                //                    SqlHelper.ExecuteNonQuery(SQLCon, "Sp_UpdateRptTableTemp", Session("userid7"), " nav_today ", "0")
                //                End If
            }
            else
            {
                if (Convert.ToInt32(Session["Bycust"]) == 0)
                {
                    if (Convert.ToInt32(Session["ref8fund"]) == 0)
                    {
                        if (Flag == 1)
                            cond = " where ReportView.value_date >= '" + Session["ref8fdate"] + "' and ReportView.value_date <= '" + Session["ref8tdate"] + "'  ";
                        else if (Flag == 2)
                            cond = " where ReportView.value_date >= '" + Session["ref8fdate"] + "' and ReportView.value_date <= '" + Session["ref8tdate"] + "'  ";
                        else if (Flag == 3)
                            cond = " where ReportView.value_date >= '" + Session["ref8fdate"] + "' and ReportView.value_date <= '" + Session["ref8tdate"] + "' " + " and ReportView.branch_id=" + Session["branchno7"];
                    }
                    else
                    {
                        if (Flag == 1)
                            cond = " where ReportView.value_date >= '" + Session["ref8fdate"] + "' and ReportView.value_date <= '" + Session["ref8tdate"] + "' and ReportView.fund_id=" + Session["ref8fund"] + "    ";
                        else if (Flag == 2)
                            cond = " where ReportView.value_date >= '" + Session["ref8fdate"] + "' and ReportView.value_date <= '" + Session["ref8tdate"] + "' " + " and ReportView.fund_id=" + Session["ref8fund"];
                        else if (Flag == 3)
                            cond = " where ReportView.value_date >= '" + Session["ref8fdate"] + "' and ReportView.value_date <= '" + Session["ref8tdate"] + "' " + " and ReportView.branch_id=" + Session["branchno7"] + " and ReportView.fund_id=" + Session["ref8fund"];
                    }
                }
                else
                {
                    str1 = "fund_id,fname,cust_id,BName,quantity,nav_today,value_date,pur_sal,group_id,cbo_type,branch_id";
                    str2 = "SELECT dbo.fund.code AS fund_id, dbo.fund.fname,  trans.cust_id, { fn IFNULL(dbo.Branch.BName, 0) } AS BName, trans.quantity, ISNULL(dbo.ICPrice.Price, 0) AS nav_today, trans.value_date, trans.pur_sal, dbo.fundright.group_id, dbo.Customer.cbo_type, dbo.Customer.branch_id FROM(SELECT fund_id, quantity, pur_sal, value_date,  cust_id FROM trans WHERE auth = 1) trans INNER JOIN dbo.fund ON trans.fund_id = dbo.fund.code LEFT OUTER JOIN dbo.ICPrice ON dbo.fund.code = dbo.ICPrice.Fund_Id AND dbo.ICPrice.ICdate = { fn CURDATE() } INNER JOIN dbo.Customer ON trans.cust_id = dbo.Customer.code INNER JOIN dbo.fundright ON trans.fund_id = dbo.fundright.fund_id INNER JOIN dbo.Branch ON dbo.Customer.branch_id = dbo.Branch.code ";
                    if (Convert.ToInt32(Session["ref8fund"]) == 0)
                    {
                        if (Flag == 1)
                            cond = " where value_date >= '" + Session["ref8fdate"] + "' and value_date <= '" + Session["ref8tdate"] + "' ";
                        else if (Flag == 2)
                            cond = " where value_date >= '" + Session["ref8fdate"] + "' and value_date <= '" + Session["ref8tdate"] + "' ";
                        else if (Flag == 3)
                            cond = " where value_date >= '" + Session["ref8fdate"] + "' and value_date <= '" + Session["ref8tdate"] + "' " + " and trans.branch_id=" + Session["branchno7"];
                    }
                    else
                    {
                        if (Flag == 1)
                            cond = " where value_date >= '" + Session["ref8fdate"] + "' and value_date <= '" + Session["ref8tdate"] + "' and trans.fund_id=" + Session["ref8fund"] + "  ";
                        else if (Flag == 2)
                            cond = " where value_date >= '" + Session["ref8fdate"] + "' and value_date <= '" + Session["ref8tdate"] + "' " + " and trans.fund_id=" + Session["ref8fund"];
                        else if (Flag == 3)
                            cond = " where value_date >= '" + Session["ref8fdate"] + "' and value_date <= '" + Session["ref8tdate"] + "'  " + " and trans.branch_id=" + Session["branchno7"] + " and trans.fund_id=" + Session["ref8fund"];

                    }
                }
            }
            var FundRight1 = SqlHelper.ExecuteDataset(SqlCon, "Sp_FundRight1", Session["groupid"]);
            for (int i = 0; i <= FundRight1.Tables[0].Rows.Count - 1; i++)
            {
                cond1 = "  ReportView.fund_id=" + Convert.ToInt32(FundRight1.Tables[0].Rows[i].ItemArray[2]) + " or " + cond1;
            }
            int xx1 = 0;
            xx1 = cond1.Length;
            cond1 = Microsoft.VisualBasic.Strings.Left(cond1, xx1 - 3);
            cond1 = " and " + cond1;
            Session["CondFund1"] = cond1;
            Session["userid7"] = cond;
            Flag = 0;
            var FundDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select distinct ReportView.fund_id, fname from ReportView " + Session["userid7"] + Session["CondFund1"]);
            for (int j = 0; j <= FundDS.Tables[0].Rows.Count - 1; j++)
            {
                var Price_DS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select Icprice.Price from Icprice where Icprice.icdate=(select max(icdate) from icprice where fund_id=" + FundDS.Tables[0].Rows[j].ItemArray[0] + ") and fund_id=" + FundDS.Tables[0].Rows[j].ItemArray[0]);
                string Price = Price_DS.Tables[0].Rows[0].ItemArray[0].ToString();
                string itotal_sub = "0";
                string itotal_red = "0";
                string inav = "0";
                string itot = "0";
                string BankName = "ABC Bank";
                var repDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select distinct (isnull(reportview.branch_id,'')) as branch_id,isnull(reportview.Bname,'') as Bname,isnull(a.total_sub,0) as total_sub,isnull(b.total_red,0)as total_red ,isnull(a.no_sub,0) as no_sub,isnull(b.no_red,0) as no_red,isnull(a.total_sub,0) - isnull(b.total_red,0) as tot,(isnull(a.total_sub,0) - isnull(b.total_red,0)) *" + Price + " as nav from (select sum(quantity)as total_sub,count(quantity) as no_sub,branch_id  from ReportView   " + Session["userid7"] + " and pur_sal=0 and Fund_id=" + FundDS.Tables[0].Rows[j].ItemArray[0] + " group by branch_id  )a full outer join (select sum(quantity)as total_red,count(quantity) as no_red,branch_id from ReportView  " + Session["userid7"] + " and pur_sal=1 and fund_id=" + FundDS.Tables[0].Rows[j].ItemArray[0] + " group by branch_id)b on a.branch_id=b.branch_id full outer join reportview on a.branch_id=reportview.branch_id or b.branch_id=reportview.branch_id " + Session["userid7"] + " and fund_id=" + FundDS.Tables[0].Rows[j].ItemArray[0]);
                var Fund_Date = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select system_date from fund where code=" + FundDS.Tables[0].Rows[j].ItemArray[0]);
                for (int i = 0; i <= repDS.Tables[0].Rows.Count - 1; i++)
                {
                    tot = (Convert.ToDouble(repDS.Tables[0].Rows[i].ItemArray[2]) - Convert.ToDouble(repDS.Tables[0].Rows[i].ItemArray[3])).ToString();
                    nav = (Convert.ToDouble(tot) * Convert.ToDouble(Price)).ToString();
                    Stotal = (Convert.ToDouble(Stotal) + Convert.ToDouble(repDS.Tables[0].Rows[i].ItemArray[2])).ToString();
                    Rtotal = (Convert.ToDouble(Rtotal) + Convert.ToDouble(repDS.Tables[0].Rows[i].ItemArray[3])).ToString();
                    Sno = (Convert.ToDouble(Sno) + Convert.ToDouble(repDS.Tables[0].Rows[i].ItemArray[2])).ToString();
                    Rno = (Convert.ToDouble(Rno) + Convert.ToDouble(repDS.Tables[0].Rows[i].ItemArray[3])).ToString();
                    SRtot = (Convert.ToDouble(Stotal) - Convert.ToDouble(Rtotal)).ToString();
                    nav_all = (Convert.ToDouble(SRtot) * Convert.ToDouble(Price)).ToString();
                    Data.Add(new FundBalancefrmData
                    {
                        Fname = FundDS.Tables[0].Rows[j].ItemArray[1].ToString(),
                        Price = Price,
                        branch_id = repDS.Tables[0].Rows[i].ItemArray[0].ToString(),
                        BName = repDS.Tables[0].Rows[i].ItemArray[1].ToString(),
                        total_sub = Convert.ToDouble(repDS.Tables[0].Rows[i].ItemArray[2]),
                        total_red = Convert.ToDouble(repDS.Tables[0].Rows[i].ItemArray[3]),
                        no_sub = Convert.ToDouble(repDS.Tables[0].Rows[i].ItemArray[4]),
                        no_red = Convert.ToDouble(repDS.Tables[0].Rows[i].ItemArray[5]),
                        tot = Convert.ToDouble(tot),
                        nav = Convert.ToDouble(nav),
                    });
                }
                var TotFund_DS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select sum(total_sub) as total_sub,sum(total_red) as total_red, sum(no_sub) as no_sub,sum(no_red) as no_red,sum(tot) as tot,sum(nav) as nav from(select distinct (isnull(reportview.branch_id,'')) as branch_id,isnull(reportview.Bname,'') as Bname,isnull(a.total_sub,0) as total_sub,isnull(b.total_red,0)as total_red ,isnull(a.no_sub,0) as no_sub,isnull(b.no_red,0) as no_red,isnull(a.total_sub,0) - isnull(b.total_red,0) as tot,(isnull(a.total_sub,0) - isnull(b.total_red,0)) *10.00000 as nav from (select sum(quantity)as total_sub,count(quantity) as no_sub,branch_id  from ReportView    where ReportView.value_date >= '" + ConvertDate(Convert.ToDateTime(Fund_Date.Tables[0].Rows[0].ItemArray[0])) + "' and ReportView.value_date <= { fn CURDATE() }  and pur_sal=0 and Fund_id=" + FundDS.Tables[0].Rows[j][0] + " group by branch_id  )a full outer join (select sum(quantity)as total_red,count(quantity) as no_red,branch_id from ReportView   where ReportView.value_date >= '" + ConvertDate(Convert.ToDateTime(Fund_Date.Tables[0].Rows[0].ItemArray[0])) + "' and ReportView.value_date <= { fn CURDATE() }   and pur_sal=1 and fund_id=" + FundDS.Tables[0].Rows[j].ItemArray[0] + " group by branch_id)b on a.branch_id=b.branch_id full outer join reportview on a.branch_id=reportview.branch_id or b.branch_id=reportview.branch_id  where ReportView.value_date >= '" + ConvertDate(Convert.ToDateTime(Fund_Date.Tables[0].Rows[0].ItemArray[0])) + "' and ReportView.value_date <= { fn CURDATE() }  and fund_id=" + FundDS.Tables[0].Rows[j].ItemArray[0] + ")a");
                Stotal1 = (Convert.ToDouble(Stotal1) + Convert.ToDouble(TotFund_DS.Tables[0].Rows[0].ItemArray[0])).ToString();
                Rtotal1 = (Convert.ToDouble(Rtotal1) + Convert.ToDouble(TotFund_DS.Tables[0].Rows[0].ItemArray[1])).ToString();
                Sno1 = (Convert.ToDouble(Sno1) + Convert.ToDouble(TotFund_DS.Tables[0].Rows[0].ItemArray[2])).ToString();
                Rno1 = (Convert.ToDouble(Rno1) + Convert.ToDouble(TotFund_DS.Tables[0].Rows[0].ItemArray[3])).ToString();
                SRtot1 = (Convert.ToDouble(Stotal1) - Convert.ToDouble(Rtotal1)).ToString();
                nav_all1 = (Convert.ToDouble(SRtot1) * Convert.ToDouble(Price)).ToString();

                tot = "0";
                nav = "0";
                Stotal = "0";
                Rtotal = "0";
                SRtot = "0";
                nav_all = "0";
                Sno = "0";
                Rno = "0";
                Stotal1 = "0";
                Rtotal1 = "0";
                Sno1 = "0";
                Rno1 = "0";
                SRtot1 = "0";
                nav_all1 = "0";
            }
            return Data;
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
                dataSet = SqlHelper.ExecuteDataset("data source=" + servername + ";initial catalog=IC_AWB2;persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework", "sp_returnfield", strselect, strfrom, strwhere);
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

        //-------------------		


    }
}