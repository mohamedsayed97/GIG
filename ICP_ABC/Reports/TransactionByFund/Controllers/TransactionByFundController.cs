using Microsoft.ApplicationBlocks.Data;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ICP_ABC.Models;
using ICP_ABC.Reports.TransactionByFund.Model;
using Microsoft.AspNet.Identity;
using System.Web.Configuration;

namespace ICP_ABC.Reports.TransactionByFund.Controllers
{
    public class TransactionByFundController : Controller
    {
        //Context _db = new Context();
        ApplicationDbContext _db = new ApplicationDbContext();
        string servername = WebConfigurationManager.AppSettings["servername"];
        static string DataBaseName = WebConfigurationManager.AppSettings["DataBaseName"];
        DataSet dataSet = new DataSet();
        // GET: TransactionByFund
        public ActionResult Index()
        {
            var FundBallance = new TransactionByFundVM
            {
                Funds = _db.Funds.ToList()

            };

            return View(FundBallance);
        }
        public ActionResult TransactionByFund(TransactionByFundVM VM)
        {
            var Data = TransactionByFundGetData(VM);
            var Cnt = Data.ToList();
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/TransactionByFund/TransactionByFundReport2.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = Data;
            localReport.DataSources.Add(reportDataSource);
            List<ReportParameter> parameters = new List<ReportParameter>();
            //DateTime FDate = (DateTime)Session["transfundfdate"];
            //DateTime TDate = (DateTime)Session["transfundtdate"];
            parameters.Add(new ReportParameter("FDate", Session["transfundfdate"].ToString()));
            parameters.Add(new ReportParameter("TDate", Session["transfundtdate"].ToString()));
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
        public List<TransactionByFundData> TransactionByFundGetData(TransactionByFundVM VM)
        {
            var transactionByFundData = new List<TransactionByFundData>();
            var this_User_id = User.Identity.GetUserId();
            var this_User_data = _db.Users.Where(x=>x.Id == this_User_id).FirstOrDefault();

            Session["groupid"] = this_User_data.GroupId;
            Session["userno"] = this_User_data.Code;
            //Session["groupid"] =5 ;
            //Session["userno"] = 259;
            Session["transfundfdate"] = VM.FromDate.Value.ToString("yyyy-MM-dd");
            Session["transfundtdate"] = VM.ToDate.Value.ToString("yyyy-MM-dd");
            var Fund_Name = VM.Fund;
            var cond = "";
            var type = "";
            var quantity = "";
            var navToday = "";
            var Flag = 0;

            var struser = "select branch_id,branch_right from users where code='" + Session["userno"] + "'";
            var druser = GetData(struser);
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
            decimal nav;
            string str = "select Price from ICPrice where ICdate=  { fn CURDATE() } and navauth=1";
            nav = Returndecimal(str);
            if (nav == -1)
                nav = 0;
            Session["repno"] = 15;
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
            Session["transfundfundid"] = fundid;
            Session["transfundnav"] = nav;

            if (VM.Auth == "1")
                Session["transtype"] = 1;
            else if (VM.Auth == "2")
                Session["transtype"] = 0;
            else
                Session["transtype"] = 2;

            if (Convert.ToInt32(Session["transfundfundid"]) == 0)
            {
                if (Convert.ToInt32(Session["transtype"]) == 0 || Convert.ToInt32(Session["transtype"]) == 1)
                {
                    if (Flag == 1)
                        cond = " where ReportView.value_date >= '" + Session["transfundfdate"] + "' and ReportView.value_date <= '" + Session["transfundtdate"] + "' and ReportView.auth= " + Session["transtype"] + " ";
                    else if (Flag == 2)
                        cond = " where ReportView.value_date >= '" + Session["transfundfdate"] + "' and ReportView.value_date <= '" + Session["transfundtdate"] + "' " + " and ReportView.auth= " + Session["transtype"];
                    else if (Flag == 3)
                        cond = " where ReportView.value_date >= '" + Session["transfundfdate"] + "' and ReportView.value_date <= '" + Session["transfundtdate"] + "' " + " and ReportView.branch_id=" + Session["branchno12"] + " and ReportView.auth= " + Session["transtype"];
                }
                else
                {
                    if (Flag == 1)
                        cond = " where ReportView.value_date >= '" + Session["transfundfdate"] + "' and ReportView.value_date <= '" + Session["transfundtdate"] + "' ";
                    else if (Flag == 2)
                        cond = " where ReportView.value_date >= '" + Session["transfundfdate"] + "' and ReportView.value_date <= '" + Session["transfundtdate"] + "' ";
                    else if (Flag == 3)
                        cond = " where ReportView.value_date >= '" + Session["transfundfdate"] + "' and ReportView.value_date <= '" + Session["transfundtdate"] + "' " + " and ReportView.branch_id=" + Session["branchno12"];

                }
            }
            else
            {
                if (Convert.ToInt32(Session["transtype"]) == 0 || Convert.ToInt32(Session["transtype"]) == 1)
                {
                    if (Flag == 1)
                        cond = " where ReportView.value_date >= '" + Session["transfundfdate"] + "' and ReportView.value_date <= '" + Session["transfundtdate"] + "' and ReportView.fund_id=" + Session["transfundfundid"] + " and ReportView.auth= " + Session["transtype"] + " ";
                    else if (Flag == 2)
                        cond = " where ReportView.value_date >= '" + Session["transfundfdate"] + "' and ReportView.value_date <= '" + Session["transfundtdate"] + "' " + " and ReportView.fund_id=" + Session["transfundfundid"] + " and ReportView.auth= " + Session["transtype"];
                    else if (Flag == 3)
                        cond = " where ReportView.value_date >= '" + Session["transfundfdate"] + "' and ReportView.value_date <= '" + Session["transfundtdate"] + "' " + " and ReportView.branch_id=" + Session["branchno12"] + " and ReportView.fund_id=" + Session["transfundfundid"] + " and ReportView.auth= " + Session["transtype"];
                }
                else
                {
                    if (Flag == 1)
                        cond = " where ReportView.value_date >= '" + Session["transfundfdate"] + "' and ReportView.value_date <= '" + Session["transfundtdate"] + "' and ReportView.fund_id=" + Session["transfundfundid"] + " ";
                    else if (Flag == 2)
                        cond = " where ReportView.value_date >= '" + Session["transfundfdate"] + "' and ReportView.value_date <= '" + Session["transfundtdate"] + "' " + " and ReportView.fund_id=" + Session["transfundfundid"];
                    else if (Flag == 3)
                        cond = " where ReportView.value_date >= '" + Session["transfundfdate"] + "' and ReportView.value_date <= '" + Session["transfundtdate"] + "' " + " and ReportView.branch_id=" + Session["branchno12"] + " and ReportView.fund_id=" + Session["transfundfundid"];
                }
            }
            Flag = 0;
            var cond1 = "";
            var FundRight = GetData("select * from FundRight where GroupID=" + Session["groupid"] + " and auth=1");
            for (int i = 0; i <= FundRight.Tables[0].Rows.Count - 1; i++)
            {
                cond1 = "  ReportView.fund_id=" +Convert.ToInt32( FundRight.Tables[0].Rows[i].ItemArray[2]) + " or " + cond1;
            }
            int xx = 0;
            xx = cond1.Length;
            cond1 = Microsoft.VisualBasic.Strings.Left(cond1, xx - 3);
            cond1 = " and (" + cond1 + ")";
            Session["CondFund3"] = cond1;
            Session["userid15"] = cond;
            var repDS = GetData("select name,fname,branch_id,value_date,nav_today as price,cust_id,bname,quantity,pur_sal,user_id as auther,user_id as inputer,transid,total_value from ReportView " + Session["userid15"]);
            Session["repDS15"] = repDS;
            var FundDS = GetData("select distinct ReportView.fund_id, fname from ReportView " + Session["userid15"] + Session["CondFund3"]);

            if (Convert.ToInt16(Session["transtype"]) == 0)
                type = "Unauthorized";
            if (Convert.ToInt16(Session["transtype"]) == 2)
                type = "Unauthorized /Authorized";
            else
                type = "Authorized";
            for (int j = 0; j <= FundDS.Tables[0].Rows.Count - 1; j++)
            {
                string t11 = "0";
                string t22 = "0";
                string t33 = "0";
                string t44 = "0";
                string Fname = FundDS.Tables[0].Rows[j].ItemArray[1].ToString();
                var Price_DS = GetData("select Icprice.Price from Icprice where Icprice.icdate=(select max(icdate) from icprice where FundId=" + FundDS.Tables[0].Rows[j].ItemArray[0] + ") and FundId=" + FundDS.Tables[0].Rows[j].ItemArray[0]);
                string Price = Price_DS.Tables[0].Rows[0].ItemArray[0].ToString();
                string BankName = "ABC Bank";
                repDS = GetData("select isnull(ename,'') as ename ,isnull(branch_id,'') as branch_id,isnull(convert(char(11),value_date,105),'') as value_date,isnull(cust_id,'') as cust_id,isnull(bname,'') as bname, isnull(pur_sal,'')as pur_sal ,isnull(user_id,'') as auther,isnull(user_id,'') as inputer,isnull(transid,'') as transid,isnull(quantity,0) as quantity,isnull(total_value,0) as total_value from ReportView  " + Session["userid15"] + " and fund_id=" + FundDS.Tables[0].Rows[j].ItemArray[0] + " order by branch_id");
                string t3 = "0";
                string t4 = "0";
                string branch = "0";
                for (int z = 0; z <= repDS.Tables[0].Rows.Count - 1; z++)
                {
                    if (repDS.Tables[0].Rows[z].ItemArray[1].ToString() == branch)
                    {
                        if (repDS.Tables[0].Rows[z].ItemArray[5].ToString() == "0")
                        {
                            quantity = repDS.Tables[0].Rows[z].ItemArray[9].ToString();
                        }
                        else
                        {
                            quantity = "-" + repDS.Tables[0].Rows[z].ItemArray[9].ToString();
                        }
                        navToday = (Convert.ToDouble(Price) * Convert.ToDouble(quantity)).ToString();
                        t3 = (Convert.ToDouble(t3) + Convert.ToDouble(quantity)).ToString();
                        t4 = (Convert.ToDouble(t4) + Convert.ToDouble(navToday)).ToString();
                        Flag = 1;
                    }
                    else
                    {
                        if (Flag != 0)
                        {
                            t3 = "0";
                            t4 = "0";
                            Flag = 0;
                        }
                        Flag = 1;
                        branch = repDS.Tables[0].Rows[z].ItemArray[1].ToString();
                        if (repDS.Tables[0].Rows[z].ItemArray[5].ToString() == "0")
                        {
                            quantity = repDS.Tables[0].Rows[z].ItemArray[9].ToString();
                        }
                        else
                        {
                            quantity = "-" + repDS.Tables[0].Rows[z].ItemArray[9].ToString();
                        }
                        navToday = (Convert.ToDouble(Price) * Convert.ToDouble(quantity)).ToString();
                        t3 = (Convert.ToDouble(t3) + Convert.ToDouble(quantity)).ToString();
                        t4 = (Convert.ToDouble(t4) + Convert.ToDouble(navToday)).ToString();

                    }
                    t33 = (Convert.ToDouble(t33) + Convert.ToDouble(quantity)).ToString();
                    t44 = (Convert.ToDouble(t44) + Convert.ToDouble(navToday)).ToString();

                    transactionByFundData.Add(new TransactionByFundData
                    {
                        Fname = Fname,
                        Price = Price,
                        type = type,
                        BankName = BankName,
                        transid = repDS.Tables[0].Rows[z].ItemArray[8].ToString(),
                        ename = repDS.Tables[0].Rows[z].ItemArray[0].ToString(),
                        cust_id = repDS.Tables[0].Rows[z].ItemArray[3].ToString(),
                        quantity = Convert.ToInt32(quantity),
                        total_value = repDS.Tables[0].Rows[z].ItemArray[10].ToString(),
                        navToday = Convert.ToDouble(navToday),
                        inputer = repDS.Tables[0].Rows[z].ItemArray[7].ToString(),
                        auther = repDS.Tables[0].Rows[z].ItemArray[6].ToString(),
                        value_date = repDS.Tables[0].Rows[z].ItemArray[2].ToString(),
                        bname = repDS.Tables[0].Rows[z].ItemArray[4].ToString()

                    });
                }
                if (Flag != 0)
                {
                    t3 = "0";
                    t4 = "0";
                    Flag = 0;
                }
            }
            return transactionByFundData;
        }
        private DataSet GetData(String Query)
        {
            var Ds = SqlHelper.ExecuteDataset("data source="+ servername + ";initial catalog="+ DataBaseName + ";persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework", CommandType.Text, Query);
            return Ds;
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


    }
}