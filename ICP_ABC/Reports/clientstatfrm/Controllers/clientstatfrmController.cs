using Microsoft.ApplicationBlocks.Data;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ICP_ABC.Models;
using ICP_ABC.Reports.clientstatfrm.Model;
using Microsoft.AspNet.Identity;
using System.Web.Configuration;
namespace ICP_ABC.Reports.clientstatfrm.Controllers
{
    public class clientstatfrmController : Controller
    {
        static string servername = WebConfigurationManager.AppSettings["servername"];


        DataSet dataSet = new DataSet();
        // Context _db = new Context();
        ApplicationDbContext _db = new ApplicationDbContext();
        static string DataBaseName = WebConfigurationManager.AppSettings["DataBaseName"];
        private string SqlCon = "data source=" + servername + ";initial catalog=" + DataBaseName + ";persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework";

        //private string SqlCon = "data source=" + servername + ";initial catalog=LastUpdateDB;persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework";
        // GET: clientstatfrm
        public ActionResult Index()
        {
            var clientstatfrmVM = new clientstatfrmVM
            {
                Funds = _db.Funds.ToList()
            };

            return View(clientstatfrmVM);
        }
        public ActionResult clientstatfrm(clientstatfrmVM VM)
        {
            var Data = GetData(VM);
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/clientstatfrm/clientstatfrmReport.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = Data;
            localReport.DataSources.Add(reportDataSource);
            List<ReportParameter> parameters = new List<ReportParameter>();
            //DateTime FDate = (DateTime)Session["transfundfdate"];
            //DateTime TDate = (DateTime)Session["transfundtdate"];
            parameters.Add(new ReportParameter("FDate", Session["ref1fdate"].ToString()));
            parameters.Add(new ReportParameter("TDate", Session["ref1tdate"].ToString()));
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
        public List<clientstatfrmData> GetData(clientstatfrmVM VM)
        {
            var Data = new List<clientstatfrmData>();
            int Flag1;
            var cond = "";
            Session["repno"] = 9;
            Session["ClientRptCase"] = 1;
            Session["ref1fdate"] = VM.FromDate.Value.ToString("yyyy-MM-dd");
            Session["ref1tdate"] = VM.ToDate.Value.ToString("yyyy-MM-dd");
            var Fund_Name = VM.Fund;
            var Flag = 0;
            var this_User_id = User.Identity.GetUserId();
            var this_User_data = _db.Users.Where(x => x.Id == this_User_id).FirstOrDefault();
            Session["groupid"] = this_User_data.GroupId;
            Session["userno"] = this_User_data.Code;
            //Session["groupid"] =5;
            //Session["userno"] = 259;


            //Session["groupid"] = 9;
            //Session["userno"] = 11;
            Session["userid9"] = "CSR9" + Session["userno"];
            Session["tempCStat"] = "ClientStat" + Session["userno"];
            SqlHelper.ExecuteNonQuery(SqlCon, "Sp_DropRptTableTemp", Session["userid9"]);
            try
            {
                var repDS = SqlHelper.ExecuteDataset(SqlCon, System.Data.CommandType.Text, "SELECT * from tempdb.dbo.A" + Session["userid9"]);
                if (repDS.Tables.Count != 0)
                    SqlHelper.ExecuteNonQuery(SqlCon, "Sp_DropRptTableTemp", Session["userid9"]);
            }
            catch (Exception)
            {


            }
            SqlHelper.ExecuteNonQuery(SqlCon, "Sp_ReportTableTemp", Session["userid9"]);
            var druser = SqlHelper.ExecuteDataset(SqlCon, "Sp_druser", Convert.ToInt32(Session["userno"]));
            var branch_id = druser.Tables[0].Rows[0].ItemArray[0];
            var branch_right = druser.Tables[0].Rows[0].ItemArray[1];
            if (Convert.ToBoolean(branch_right) == true)
            {
                Flag = 1;
            }
            else
            {
                Session["branchno5"] = Convert.ToInt32(branch_id);
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
            var sql = "select price from  icprice where icdate='" + formatdate1(VM.ToDate.Value.ToString("dd-MM-yyyy")) + "' and navauth=1";
            Session["price_date"] = Returndecimal(sql);
            Session["ref1fund"] = fundid;
            if (VM.Code == "" || VM.Code == null)
            {
                Session["ref1cust"] = "";
                Flag1 = 1;
            }
            else
            {
                Session["ref1cust"] = VM.Code;
                Flag1 = 4;
            }
            string str1 = " fund_id,branch_id,code,curr_id,pur_sal,value_date,quantity,unit_price,today_Date,date,inputer,cust_id,fund,transid";
            string str2 = " select fund_id,branch_id,code,curr_id,pur_sal,value_date,quantity,unit_price,{ fn CURDATE() } AS today_Date,value_date,inputer,cust_id,fund_id as fund,transid from trans WHERE (dbo.trans.auth = 1)  ";
            if (Convert.ToInt32(Session["ref1fund"]) == 0)
            {
                if (Flag1 == 1)
                {
                    if (Flag == 1)
                        cond = " and value_date >= '" + Session["ref1fdate"] + "' and value_date <= '" + Session["ref1tdate"] + "'  ";
                    else if (Flag == 2)
                        cond = " and value_date >= '" + Session["ref1fdate"] + "' and value_date <= '" + Session["ref1tdate"] + "' ";
                    else if (Flag == 3)
                        cond = " and value_date >= '" + Session["ref1fdate"] + "' and value_date <= '" + Session["ref1tdate"] + "' " + " and trans.branch_id=" + Session["branchno5"];
                }
                else if (Flag1 == 2)
                {
                    if (Flag == 1)
                        cond = " and value_date >= '" + Session["ref1fdate"] + "' and value_date <= '" + Session["ref1tdate"] + "' ";
                    else if (Flag == 2)
                        cond = " and value_date >= '" + Session["ref1fdate"] + "' and value_date <= '" + Session["ref1tdate"] + "' ";
                    else if (Flag == 3)
                        cond = " and value_date >= '" + Session["ref1fdate"] + "' and value_date <= '" + Session["ref1tdate"] + "' " + " and trans.branch_id=" + Session["branchno5"];
                }
                else if (Flag1 == 3)
                {
                    if (Flag == 1)
                        cond = " and value_date >= '" + Session["ref1fdate"] + "' and value_date <= '" + Session["ref1tdate"] + "' ";
                    else if (Flag == 2)
                        cond = " and value_date >= '" + Session["ref1fdate"] + "' and value_date <= '" + Session["ref1tdate"] + "' ";
                    else if (Flag == 3)
                        cond = " and value_date >= '" + Session["ref1fdate"] + "' and value_date <= '" + Session["ref1tdate"] + "' " + " and trans.branch_id=" + Session["branchno5"];
                }
                else if (Flag1 == 4)
                {
                    if (Flag == 1)
                        cond = " and value_date >= '" + Session["ref1fdate"] + "' and value_date <= '" + Session["ref1tdate"] + "' and trans.cust_id='" + Session["ref1cust"] + "'  ";
                    else if (Flag == 2)
                        cond = " and value_date >= '" + Session["ref1fdate"] + "' and value_date <= '" + Session["ref1tdate"] + "' " + " and trans.cust_id='" + Session["ref1cust"] + "'";
                    else if (Flag == 3)
                        cond = " and value_date >= '" + Session["ref1fdate"] + "' and value_date <= '" + Session["ref1tdate"] + "' " + " and trans.branch_id=" + Session["branchno5"] + " and trans.cust_id='" + Session["ref1cust"] + "'";
                }

            }
            else
            {
                if (Flag1 == 1)
                {
                    if (Flag == 1)
                        cond = " and value_date >= '" + Session["ref1fdate"] + "' and value_date <= '" + Session["ref1tdate"] + "' and trans.fund_id=" + Session["ref1fund"] + " ";
                    else if (Flag == 2)
                        cond = " and value_date >= '" + Session["ref1fdate"] + "' and value_date <= '" + Session["ref1tdate"] + "' " + " and trans.fund_id=" + Session["ref1fund"];
                    else if (Flag == 3)
                        cond = " and value_date >= '" + Session["ref1fdate"] + "' and value_date <= '" + Session["ref1tdate"] + "' " + " and trans.branch_id=" + Session["branchno5"] + " and trans.fund_id=" + Session["ref1fund"];
                }
                else if (Flag1 == 2)
                {
                    if (Flag == 1)
                        cond = " and value_date >= '" + Session["ref1fdate"] + "' and value_date <= '" + Session["ref1tdate"] + "' " + " and trans.fund_id=" + Session["ref1fund"];
                    else if (Flag == 2)
                        cond = " and value_date >= '" + Session["ref1fdate"] + "' and value_date <= '" + Session["ref1tdate"] + "' " + " and trans.fund_id=" + Session["ref1fund"];
                    else if (Flag == 3)
                        cond = " and value_date >= '" + Session["ref1fdate"] + "' and value_date <= '" + Session["ref1tdate"] + "' " + " and trans.branch_id=" + Session["branchno5"] + " and trans.fund_id=" + Session["ref1fund"];
                }
                else if (Flag1 == 3)
                {
                    if (Flag == 1)
                        cond = " and value_date >= '" + Session["ref1fdate"] + "' and value_date <= '" + Session["ref1tdate"] + "' " + " and trans.fund_id=" + Session["ref1fund"];
                    else if (Flag == 2)
                        cond = " and value_date >= '" + Session["ref1fdate"] + "' and value_date <= '" + Session["ref1tdate"] + "' " + " and trans.fund_id=" + Session["ref1fund"];
                    else if (Flag == 3)
                        cond = " and value_date >= '" + Session["ref1fdate"] + "' and value_date <= '" + Session["ref1tdate"] + "' " + " and trans.branch_id=" + Session["branchno5"] + " and trans.fund_id=" + Session["ref1fund"];
                }
                else if (Flag1 == 4)
                {
                    if (Flag == 1)
                        cond = " and value_date >= '" + Session["ref1fdate"] + "' and value_date <= '" + Session["ref1tdate"] + "' and trans.cust_id='" + Session["ref1cust"] + "' and trans.fund_id=" + Session["ref1fund"] + " ";
                    else if (Flag == 2)
                        cond = " and value_date >= '" + Session["ref1fdate"] + "' and value_date <= '" + Session["ref1tdate"] + "' " + " and trans.cust_id='" + Session["ref1cust"] + "' and trans.fund_id=" + Session["ref1fund"];
                    else if (Flag == 3)
                        cond = " and value_date >= '" + Session["ref1fdate"] + "' and value_date <= '" + Session["ref1tdate"] + "' " + " and trans.branch_id=" + Session["branchno5"] + " and trans.cust_id='" + Session["ref1cust"] + "' and trans.fund_id=" + Session["ref1fund"];
                }
            }
            Flag = 0;
            Flag1 = 0;
            try
            {
                var repDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "SELECT * from tempdb.dbo.A" + Session["tempCStat"]);
                if (repDS.Tables.Count != 0)
                {
                    SqlHelper.ExecuteNonQuery(SqlCon, "Sp_DropRptTableTemp", Session["tempCStat"]);
                }
            }
            catch (Exception)
            {
            }
            SqlHelper.ExecuteNonQuery(SqlCon, "Sp_ReportTableTemp", Session["tempCStat"]);
            try
            {
                //var str3 = " enaddress1 ";
                //var str4 = " (select customer.enaddress1 from customer where customer.code=tempdb.dbo.A" + Session["tempCStat"] + ".cust_id), BranchId=(select customer.BranchId from customer where customer.code=tempdb.dbo.A" + Session["tempCStat"] + ".cust_id), EnName=(select customer.enname from customer where customer.code=tempdb.dbo.A" + Session["tempCStat"] + ".cust_id), curr_id=(select distinct trans.curr_id from trans where trans.cust_id=tempdb.dbo.A" + Session["tempCStat"] + ".cust_id)";
                var str3 = " eaddress1 ";
                var str4 = " (select customer.enaddress1 from customer where customer.code=tempdb.dbo.A" + Session["tempCStat"] + ".cust_id), branch_id =(select customer.BranchId from customer where customer.code=tempdb.dbo.A" + Session["tempCStat"] + ".cust_id), ename =(select customer.enname from customer where customer.code=tempdb.dbo.A" + Session["tempCStat"] + ".cust_id), curr_id =(select distinct trans.curr_id from trans where trans.cust_id=tempdb.dbo.A" + Session["tempCStat"] + ".cust_id)";
                SqlHelper.ExecuteNonQuery(SqlCon, "Sp_UpdateRptTableTemp", Session["tempCStat"], str3, str4);
                //var str7 = " BName ";
                //var str8 = " (select Branch.BName from Branch where Branch.branchcode=tempdb.dbo.A" + Session["tempCStat"] + ".branch_id), short_name=(select Currency.shortname from Currency where Currency.code=tempdb.dbo.A" + Session["tempCStat"] + ".curr_id)";
                var str7 = " BName ";
                var str8 = " (select Branch.BName from Branch where Branch.branchcode=tempdb.dbo.A" + Session["tempCStat"] + ".branch_id), short_name=(select Currency.shortname from Currency where Currency.code=tempdb.dbo.A" + Session["tempCStat"] + ".curr_id)";

                SqlHelper.ExecuteNonQuery(SqlCon, "Sp_UpdateRptTableTemp", Session["tempCStat"], str7, str8);
            }
            catch (Exception)
            {

            }
            try
            {
                SqlHelper.ExecuteNonQuery(SqlCon, "Sp_InsertRptTableTemp", Session["userid9"], str1, str2, cond);
            }
            catch (Exception)
            {

            }
            try
            {
                var str3 = " nav_today ";

                //var str4 = " (select ISNULL(dbo.ICPrice.Price, 0) AS nav_today from icprice where icprice.FundId=tempdb.dbo.A" + Session["userid9"] + ".fund_id AND dbo.ICPrice.ICdate = { fn CURDATE() }),fname  =(select fund.fname from fund where fund.code=tempdb.dbo.A" + Session["userid9"] + ".fund_id),BName=(select Branch.BName from Branch where Branch.branchcode=tempdb.dbo.A" + Session["userid9"] + ".branch_id),AccountNo =(select customer.AccountNo from Customer where Customer.code=tempdb.dbo.A" + Session["userid9"] + ".Cust_id),  eaddress1 =(select customer.enaddress1 from customer where customer.code=tempdb.dbo.A" + Session["userid9"] + ".cust_id),emaddress1 =(select customer.Email1 from customer where customer.code=tempdb.dbo.A" + Session["userid9"] + ".cust_id),tel1=(select customer.tel1 from customer where customer.code=tempdb.dbo.A" + Session["userid9"] + ".cust_id), aname=(select customer.arname from customer where customer.code=tempdb.dbo.A" + Session["userid9"] + ".cust_id),ename=(select customer.enname from customer where customer.code=tempdb.dbo.A" + Session["userid9"] + ".cust_id),aaddress1 =(select customer.araddress1 from customer where customer.code=tempdb.dbo.A" + Session["userid9"] + ".cust_id),short_name =(select Currency.shortname from Currency where Currency.code=tempdb.dbo.A" + Session["userid9"] + ".curr_id),cbo_type=(select fund.cbo_type from fund where fund.code=tempdb.dbo.A" + Session["userid9"] + ".fund_id),group_id=(select users.group_id from users where users.code=tempdb.dbo.A" + Session["userid9"] + ".inputer)";
                var str4 = "(select ISNULL(dbo.ICPrice.Price, 0) AS nav_today from icprice where icprice.FundId=tempdb.dbo.A" + Session["userid9"] + ".fund_id AND dbo.ICPrice.ICdate = { fn CURDATE() }),fname  =(select fund.fname from fund where fund.code=tempdb.dbo.A" + Session["userid9"] + ".fund_id),BName=(select Branch.BName from Branch where Branch.branchcode=tempdb.dbo.A" + Session["userid9"] + ".branch_id),AccountNo =(select customer.AccountNo from Customer where Customer.code=tempdb.dbo.A" + Session["userid9"] + ".Cust_id),eaddress1 =(select customer.enaddress1 from customer where customer.code=tempdb.dbo.A" + Session["userid9"] + ".cust_id),emaddress1 =(select customer.Email1 from customer where customer.code=tempdb.dbo.A" + Session["userid9"] + ".cust_id),tel1=(select customer.tel1 from customer where customer.code=tempdb.dbo.A" + Session["userid9"] + ".cust_id),aname=(select customer.arname from customer where customer.code=tempdb.dbo.A" + Session["userid9"] + ".cust_id),ename=(select customer.enname from customer where customer.code=tempdb.dbo.A" + Session["userid9"] + ".cust_id),aaddress1 =(select customer.araddress1 from customer where customer.code=tempdb.dbo.A" + Session["userid9"] + ".cust_id),short_name =(select Currency.shortname from Currency where Currency.code=tempdb.dbo.A" + Session["userid9"] + ".curr_id),cbo_type=(select fund.cbo_type from fund where fund.code=tempdb.dbo.A" + Session["userid9"] + ".fund_id),group_id=(select users.group_id from users where users.code=tempdb.dbo.A" + Session["userid9"] + ".inputer)";
                //var str4 = " (select ISNULL(dbo.ICPrice.Price, 0) AS nav_today from icprice where icprice.FundId=tempdb.dbo.A" + Session["userid9"] + ".fund_id AND dbo.ICPrice.ICdate = { fn CURDATE() }) , fname=(select fund.fname from fund where fund.code=tempdb.dbo.A" + Session["userid9"] + ".fund_id) , BName=(select Branch.BName from Branch where Branch.branchcode=tempdb.dbo.A" + Session["userid9"] + ".branch_id),AccountNo=(select customer.AccountNo from Customer where Customer.code=tempdb.dbo.A" + Session["userid9"] + ".Cust_id), enaddress1=(select customer.enaddress1 from customer where customer.code=tempdb.dbo.A" + Session["userid9"] + ".cust_id), Email1=(select customer.Email1 from customer where customer.code=tempdb.dbo.A" + Session["userid9"] + ".cust_id), tel1=(select customer.tel1 from customer where customer.code=tempdb.dbo.A" + Session["userid9"] + ".cust_id), arname=(select customer.arname from customer where customer.code=tempdb.dbo.A" + Session["userid9"] + ".cust_id), enname=(select customer.enname from customer where customer.code=tempdb.dbo.A" + Session["userid9"] + ".cust_id), araddress1=(select customer.araddress1 from customer where customer.code=tempdb.dbo.A" + Session["userid9"] + ".cust_id), shortname=(select Currency.shortname from Currency where Currency.code=tempdb.dbo.A" + Session["userid9"] + ".curr_id),cbo_type=(select fund.cbo_type from fund where fund.code=tempdb.dbo.A" + Session["userid9"] + ".fund_id),group_id=(select users.group_id from users where users.code=tempdb.dbo.A" + Session["userid9"] + ".inputer)";
                SqlHelper.ExecuteNonQuery(SqlCon, "Sp_UpdateRptTableTemp", Session["userid9"], str3, str4);
            }
            catch (Exception)
            {

            }
            //end henaa---------------------------
            var nav_todayDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "Select nav_today from tempdb.dbo.A" + Session["userid9"]);
            if (nav_todayDS.Tables[0].Rows[0].ItemArray[0] == null)
            {
                SqlHelper.ExecuteNonQuery(SqlCon, "Sp_UpdateRptTableTemp", Session["userid9"], " nav_today ", "0");
            }
            //edit
            var CSDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "SELECT code,ename,eaddress1,emaddress1,tel1,fund_id,fname,cbo_type,value_date,aname,unit_price,pur_sal,inputer,nav_today,aaddress1,short_name,branch_id,cust_id,bname,curr_id,date,today_date,fund,OB_Flag,opening_balance,quantity from tempdb.dbo.A" + Session["userid9"]);
            if (CSDS.Tables[0].Rows.Count == 0)
            {
                //Exit Sub
            }
            var ename = CSDS.Tables[0].Rows[0].ItemArray[1];
            var eaddress1 = CSDS.Tables[0].Rows[0].ItemArray[2];
            var Bname = CSDS.Tables[0].Rows[0].ItemArray[18];
            var cust_id = CSDS.Tables[0].Rows[0].ItemArray[17];
            var short_name = CSDS.Tables[0].Rows[0].ItemArray[15];
            var tdate = VM.ToDate.Value.ToString("dd-MM-yyyy");
            var FundDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "SELECT distinct tempdb.dbo.A" + Session["userid9"] + ".fund_id, fund.fname as fname from tempdb.dbo.A" + Session["userid9"] + " inner join fund on fund.code = tempdb.dbo.A" + Session["userid9"] + " .fund_id");
            string Opening_Balance, no_unit;
            for (int j = 0; j <= FundDS.Tables[0].Rows.Count - 1; j++)
            {
                var str5 = "cust_id,Opening_Balance";
                var str6 = " SELECT a.cust_id , a.total_sub - isnull(b.total_red, 0) AS Opening_Balance FROM(SELECT  trans.cust_id, SUM(trans.quantity) AS total_sub FROM trans  where (trans.pur_sal = 0)and   (trans.value_date<'" + Session["ref1fdate"] + "'  and fund_id=" + FundDS.Tables[0].Rows[j].ItemArray[0] + ")GROUP BY pur_sal, cust_id )a LEFT OUTER JOIN(SELECT  trans.cust_id, SUM(trans.quantity) AS total_red FROM trans  where (trans.pur_sal = 1) and  ( trans.value_date<'" + Session["ref1fdate"] + "' and fund_id=" + FundDS.Tables[0].Rows[j].ItemArray[0] + " )GROUP BY pur_sal, cust_id )b ON a.cust_id = b.cust_id ";
                var cond1 = " order by a.cust_id ";
                try
                {
                    SqlHelper.ExecuteNonQuery(SqlCon, "Sp_InsertRptTableTemp", Session["tempCStat"], str5, str6, cond1);
                }
                catch (Exception)
                {

                }
                try
                {
                    //var str3 = " enaddress1 ";
                    //var str4 = " (select customer.enaddress1 from customer where customer.code=tempdb.dbo.A" + Session["tempCStat"] + ".cust_id), BranchId=(select customer.BranchId from customer where customer.code=tempdb.dbo.A" + Session["tempCStat"] + ".cust_id), ename=(select customer.enname from customer where customer.code=tempdb.dbo.A" + Session["tempCStat"] + ".cust_id), curr_id=(select distinct trans.curr_id from trans where trans.cust_id=tempdb.dbo.A" + Session["tempCStat"] + ".cust_id)";

                    var str3 = " eaddress1 ";
                    var str4 = " (select customer.enaddress1 from customer where customer.code=tempdb.dbo.A" + Session["tempCStat"] + ".cust_id), branch_id=(select customer.BranchId from customer where customer.code=tempdb.dbo.A" + Session["tempCStat"] + ".cust_id), ename=(select customer.enname from customer where customer.code=tempdb.dbo.A" + Session["tempCStat"] + ".cust_id), curr_id=(select distinct trans.curr_id from trans where trans.cust_id=tempdb.dbo.A" + Session["tempCStat"] + ".cust_id)";


                    SqlHelper.ExecuteNonQuery(SqlCon, "Sp_UpdateRptTableTemp", Session["tempCStat"], str3, str4);


                    //var str7 = " BName ";
                    //var str8 = " (select Branch.BName from Branch where Branch.branchcode=tempdb.dbo.A" + Session["tempCStat"] + ".branch_id), short_name=(select Currency.shortname from Currency where Currency.code=tempdb.dbo.A" + Session["tempCStat"] + ".curr_id)";

                    var str7 = " BName ";
                    var str8 = " (select Branch.BName from Branch where Branch.branchcode=tempdb.dbo.A" + Session["tempCStat"] + ".branch_id), short_name=(select Currency.shortname from Currency where Currency.code=tempdb.dbo.A" + Session["tempCStat"] + ".curr_id)";


                    SqlHelper.ExecuteNonQuery(SqlCon, "Sp_UpdateRptTableTemp", Session["tempCStat"], str7, str8);
                }
                catch (Exception)
                {

                }
                try
                {
                    //var str3 = " nav_today ";
                    //var str4 = " (select ISNULL(dbo.ICPrice.Price, 0) AS nav_today from icprice where icprice.FundId=tempdb.dbo.A" + Session["userid9"] + ".fund_id AND dbo.ICPrice.ICdate = { fn CURDATE() }) , fname=(select fund.fname from fund where fund.code=tempdb.dbo.A" + Session["userid9"] + ".fund_id) , BName=(select Branch.BName from Branch where Branch.branchcode=tempdb.dbo.A" + Session["userid9"] + ".branch_id),AccountNo=(select customer.AccountNo from Customer where Customer.code=tempdb.dbo.A" + Session["userid9"] + ".Cust_id), eaddress1=(select customer.enaddress1 from customer where customer.code=tempdb.dbo.A" + Session["userid9"] + ".cust_id), emaddress1=(select customer.Email1 from customer where customer.code=tempdb.dbo.A" + Session["userid9"] + ".cust_id), tel1=(select customer.tel1 from customer where customer.code=tempdb.dbo.A" + Session["userid9"] + ".cust_id), aname=(select customer.arname from customer where customer.code=tempdb.dbo.A" + Session["userid9"] + ".cust_id), ename=(select customer.enname from customer where customer.code=tempdb.dbo.A" + Session["userid9"] + ".cust_id), aaddress1=(select customer.araddress1 from customer where customer.code=tempdb.dbo.A" + Session["userid9"] + ".cust_id), short_name=(select Currency.shortname from Currency where Currency.code=tempdb.dbo.A" + Session["userid9"] + ".curr_id),cbo_type=(select fund.cbo_type from fund where fund.code=tempdb.dbo.A" + Session["userid9"] + ".fund_id),group_id=(select users.group_id from users where users.code=tempdb.dbo.A" + Session["userid9"] + ".inputer)";

                    var str3 = " nav_today ";
                    var str4 = " (select ISNULL(dbo.ICPrice.Price, 0) AS nav_today from icprice where icprice.FundId=tempdb.dbo.A" + Session["userid9"] + ".fund_id AND dbo.ICPrice.ICdate = { fn CURDATE() }) , fname=(select fund.fname from fund where fund.code=tempdb.dbo.A" + Session["userid9"] + ".fund_id) , BName=(select Branch.BName from Branch where Branch.branchcode=tempdb.dbo.A" + Session["userid9"] + ".branch_id),AccountNo=(select customer.AccountNo from Customer where Customer.code=tempdb.dbo.A" + Session["userid9"] + ".Cust_id), eaddress1=(select customer.enaddress1 from customer where customer.code=tempdb.dbo.A" + Session["userid9"] + ".cust_id), emaddress1=(select customer.Email1 from customer where customer.code=tempdb.dbo.A" + Session["userid9"] + ".cust_id), tel1=(select customer.tel1 from customer where customer.code=tempdb.dbo.A" + Session["userid9"] + ".cust_id), aname=(select customer.arname from customer where customer.code=tempdb.dbo.A" + Session["userid9"] + ".cust_id), ename=(select customer.enname from customer where customer.code=tempdb.dbo.A" + Session["userid9"] + ".cust_id), aaddress1=(select customer.araddress1 from customer where customer.code=tempdb.dbo.A" + Session["userid9"] + ".cust_id), short_name=(select Currency.shortname from Currency where Currency.code=tempdb.dbo.A" + Session["userid9"] + ".curr_id),cbo_type=(select fund.cbo_type from fund where fund.code=tempdb.dbo.A" + Session["userid9"] + ".fund_id),group_id=(select users.group_id from users where users.code=tempdb.dbo.A" + Session["userid9"] + ".inputer)";


                    SqlHelper.ExecuteNonQuery(SqlCon, "Sp_UpdateRptTableTemp", Session["userid9"], str3, str4);
                }
                catch (Exception)
                {

                }
                nav_todayDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "Select nav_today from tempdb.dbo.A" + Session["userid9"]);
                if (nav_todayDS.Tables[0].Rows[0].ItemArray[0] == null)
                {
                    SqlHelper.ExecuteNonQuery(SqlCon, "Sp_UpdateRptTableTemp", Session["userid9"], " nav_today ", "0");
                }
                var str13 = " Opening_Balance ";
                var str14 = " (select  Opening_Balance from tempdb.dbo.A" + Session["tempCStat"] + " where tempdb.dbo.A" + Session["tempCStat"] + ".cust_id=tempdb.dbo.A" + Session["userid9"] + ".cust_id ),OB_Flag=1";

                SqlHelper.ExecuteNonQuery(SqlCon, "Sp_UpdateRptTableTemp", Session["userid9"], str13, str14);
               
                
                var str15 = " Opening_Balance ";
                var str16 = " 0 where tempdb.dbo.A" + Session["userid9"] + ".Opening_Balance is null";
               
                SqlHelper.ExecuteNonQuery(SqlCon, "Sp_UpdateRptTableTemp", Session["userid9"], str15, str16);
              
                var str9 = " OB_Flag ";
                var str10 = " (select distinct OB_Flag from tempdb.dbo.A" + Session["userid9"] + "  where tempdb.dbo.A" + Session["userid9"] + ".cust_id=tempdb.dbo.A" + Session["tempCStat"] + ".cust_id )";
                SqlHelper.ExecuteNonQuery(SqlCon, "Sp_UpdateRptTableTemp", Session["tempCStat"], str9, str10);
                var fname = FundDS.Tables[0].Rows[j].ItemArray[1];
                
                var Price_DS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select Icprice.Price from Icprice where Icprice.icdate=(select max(icdate) from icprice where FundId=" + FundDS.Tables[0].Rows[j].ItemArray[0] + ") and FundId=" + FundDS.Tables[0].Rows[j].ItemArray[0]);
                var Price = Price_DS.Tables[0].Rows[0].ItemArray[0];
                CSDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "SELECT code,ename,eaddress1,emaddress1,tel1,fund_id,fname,cbo_type,value_date,aname,unit_price,pur_sal,inputer,nav_today,aaddress1,short_name,branch_id,cust_id,bname,curr_id,date,today_date,fund,OB_Flag,opening_balance,quantity from tempdb.dbo.A" + Session["userid9"] + " where fund_id=" + FundDS.Tables[0].Rows[j].ItemArray[0]);
                Opening_Balance = CSDS.Tables[0].Rows[0].ItemArray[24].ToString();
                no_unit = "0";
                var no_unit1 = "0";
                //Get No Units
                for (int i = 0; i <= CSDS.Tables[0].Rows.Count - 1; i++)
                {
                    var value_date = CSDS.Tables[0].Rows[i].ItemArray[7].ToString();
                    var unit_price = CSDS.Tables[0].Rows[i].ItemArray[10].ToString();
                    var trans_type = "";
                    var qty = "0";
                    if (Convert.ToInt32(CSDS.Tables[0].Rows[i].ItemArray[11]) == 0)
                    {
                        trans_type = "Subscription";
                        qty = CSDS.Tables[0].Rows[i].ItemArray[25].ToString();
                    }
                    else
                    {
                        trans_type = "Redemeption";
                        qty = "-" + CSDS.Tables[0].Rows[i].ItemArray[25].ToString();
                    }
                    var tot = (Convert.ToDouble(qty) * Convert.ToDouble(unit_price)).ToString();
                    no_unit1 = (Convert.ToDouble(qty) + Convert.ToDouble(no_unit1)).ToString();
                }

                var total = (Convert.ToDouble(Opening_Balance) + Convert.ToDouble(no_unit1)).ToString();
                var Price_date = (Convert.ToDouble(no_unit1) * Convert.ToDouble(Price_DS.Tables[0].Rows[0].ItemArray[0])).ToString();

                for (int i = 0; i <= CSDS.Tables[0].Rows.Count - 1; i++)
                {
                    var value_date = CSDS.Tables[0].Rows[i].ItemArray[8].ToString();
                    var unit_price = CSDS.Tables[0].Rows[i].ItemArray[10].ToString();
                    var trans_type = "";
                    var qty = "";
                    if (Convert.ToInt32(CSDS.Tables[0].Rows[i].ItemArray[11]) == 0)
                    {
                        trans_type = "Subscription";
                        qty = CSDS.Tables[0].Rows[i].ItemArray[25].ToString();
                    }
                    else
                    {
                        trans_type = "Redemeption";
                        qty = "-" + CSDS.Tables[0].Rows[i].ItemArray[25].ToString();
                    }
                    var tot = (Convert.ToDouble(qty) * Convert.ToDouble(unit_price)).ToString();
                    no_unit = (Convert.ToDouble(qty) + Convert.ToDouble(no_unit)).ToString();
                    Data.Add(new clientstatfrmData
                    {
                        ename = ename.ToString(),
                        eaddress1 = eaddress1.ToString(),
                        Bname = Bname.ToString(),
                        cust_id = cust_id.ToString(),
                        short_name = short_name.ToString(),
                        tdate = tdate.ToString(),
                        fname = fname.ToString(),
                        value_date = Convert.ToDateTime(value_date),
                        trans_type = trans_type.ToString(),
                        qty = qty,
                        unit_price = unit_price,
                        tot = tot,
                        no_unit = no_unit,
                        total = total,
                        Price = Price.ToString(),
                        Price_date = Price_date
                    });
                }


            }
            return Data;
        }
        private DateTime formatdate1(string d)
        {
            int dd, mm, yy;
            string str;
            str = d.Substring(0, 2);
            dd = Convert.ToInt32(str);
            str = d.Substring(3, 2);
            mm = Convert.ToInt32(str);
            str = d.Substring(6, 4);
            yy = Convert.ToInt32(str);
            DateTime ndate = new DateTime(yy, mm, dd);
            return ndate;
        }
        private decimal Returndecimal(String SQL)
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
        private void SelectToDataSet(string selectStr, params object[] ParamArray)
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
                dataSet = SqlHelper.ExecuteDataset("data source=" + servername + ";initial catalog=LastUpdateDB;persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework", "sp_returnfield", strselect, strfrom, strwhere);
            }
            catch (Exception)
            {

            }
        }

    }
}
