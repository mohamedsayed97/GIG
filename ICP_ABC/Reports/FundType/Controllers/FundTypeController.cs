using Microsoft.ApplicationBlocks.Data;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ICP_ABC.Models;
using ICP_ABC.Reports.FundType.Model;
using Microsoft.AspNet.Identity;
using System.Web.Configuration;



namespace ICP_ABC.Reports.FundType.Controllers
{
    public class FundTypeController : Controller
    {
        // GET: FundType

        static string servername = WebConfigurationManager.AppSettings["servername"];
        static string DataBaseName = WebConfigurationManager.AppSettings["DataBaseName"];
        //private string SqlCon = "data source=" + servername + ";initial catalog=" + DataBaseName + ";persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework";
        ApplicationDbContext _db = new ApplicationDbContext();
        public ActionResult Index()
        {

            //ViewData["Funds"] = new SelectList(_db.Funds.ToList(), "code", "name");
            var FundTypeVM = new FundTypeVM
            {
                
                Funds = _db.Funds.ToList()

            };

            return View(FundTypeVM);
        }
        //----------------------------

        private List<FundTypeData> GetData(FundTypeVM FundTypeVM)
        {

           
            var this_User_id = User.Identity.GetUserId();
            var this_User_data = _db.Users.Where(x => x.Id == this_User_id).FirstOrDefault();
            Session["groupid"] = this_User_data.GroupId;
            Session["userno"] = this_User_data.Code;
            //Session["groupid"] = 5;
            //Session["userno"] = 259;

            //Session["groupid"] = 9;
            var FundTypeData = new List<FundTypeData>();
            string totalPos;
            //string navDate;
            string price;
            string fund_id;
            string units;
            string fname;
            var cond = "";
            var Flag = 0;
            //var fund_type = FundTypeVM.FundType;
            var Fund_Name = FundTypeVM.Funds;
            //Session["userno"] = 11;//Loged In User
            //var fromDate= FundBallanceVM.FromDate.ToString();
            //var toDate = FundBallanceVM.ToDate.ToString();
            Session["balancefdate"] = FundTypeVM.FromDate.Value.ToString("yyyy-MM-dd");
         
            Session["balancetdate"] = FundTypeVM.ToDate.Value.ToString("yyyy-MM-dd");
            
            var druser = SqlHelper.ExecuteDataset("data source=" + servername + ";initial catalog=" + DataBaseName + ";persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework", CommandType.Text, "select branch_id,branch_right from users where code='" + Session["userno"] + "'");
            var branch_id = druser.Tables[0].Rows[0].ItemArray[0];
            var branch_right = druser.Tables[0].Rows[0].ItemArray[1];
            if (Convert.ToBoolean(branch_right) == true)
            {
                Session["branchno"] = 0;
                Flag = 2;
            }
            //else
            //{
            //    Session["branchno"] = Convert.ToInt32(branch_id);
            //    Flag = 3;
            //}
            //if (fund_type == 3)
            //{
                Session["balancefundtype"] = 0;
            //}
            //else
            //{
            //    Session["balancefundtype"] = fund_type;
            //}
            int fundid;
            try
            {
                if (Convert.ToInt32(Fund_Name) == 0 || Convert.ToInt32(Fund_Name) == 0)
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
            Session["balancefund"] = fundid;
            if (Convert.ToInt32(Session["balancefund"]) == 0)
            {
                if (Convert.ToInt32(Session["balancefundtype"]) == 0)
                {
                    if (Flag == 1)
                        cond = " and value_date >= '" + Session["balancefdate"] + "' and value_date <= '" + Session["balancetdate"] + "'   ";
                    else if (Flag == 2)
                        cond = " and value_date >= '" + Session["balancefdate"] + "' and value_date <= '" + Session["balancetdate"] + "'  ";
                    else if (Flag == 3)
                        cond = " and value_date >= '" + Session["balancefdate"] + "' and value_date <= '" + Session["balancetdate"] + "' " + " and branch_id=" + Session["branchno"];
                }
                else
                {
                    if (Flag == 1)
                        cond = " and value_date >= '" + Session["balancefdate"] + "' and value_date <= '" + Session["balancetdate"] + "' and CustTypeId=" + Session["balancefundtype"] + "   ";
                    else if (Flag == 2)
                        cond = " and value_date >= '" + Session["balancefdate"] + "' and value_date <= '" + Session["balancetdate"] + "' " + " and CustTypeId=" + Session["balancefundtype"];
                    else if (Flag == 3)
                        cond = " and value_date >= '" + Session["balancefdate"] + "' and value_date <= '" + Session["balancetdate"] + "' " + " and branch_id=" + Session["branchno"] + " and CustTypeId=" + Session["balancefundtype"];
                }
            }
            else
            {
                if (Convert.ToInt32(Session["balancefundtype"]) == 0)
                {
                    if (Flag == 1)
                        cond = " and ReportView.value_date >= '" + Session["balancefdate"] + "' and ReportView.value_date <= '" + Session["balancetdate"] + "' and ReportView.fund_id=" + Session["balancefund"] + "  ";
                    else if (Flag == 2)
                        cond = " and ReportView.value_date >= '" + Session["balancefdate"] + "' and ReportView.value_date <= '" + Session["balancetdate"] + "' " + " and ReportView.fund_id=" + Session["balancefund"];
                    else if (Flag == 3)
                        cond = " and ReportView.value_date >= '" + Session["balancefdate"] + "' and ReportView.value_date <= '" + Session["balancetdate"] + "' " + " and ReportView.branch_id=" + Session["branchno"] + " and ReportView.fund_id=" + Session["balancefund"];
                }
                else
                {
                    if (Flag == 1)
                        cond = " and ReportView.value_date >= '" + Session["balancefdate"] + "' and ReportView.value_date <= '" + Session["balancetdate"] + "' and ReportView.fund_id=" + Session["balancefund"] + " and ReportView.CustTypeId=" + Session["balancefundtype"] + "  ";
                    else if (Flag == 2)
                        cond = " and ReportView.value_date >= '" + Session["balancefdate"] + "' and ReportView.value_date <= '" + Session["balancetdate"] + "' " + " and ReportView.fund_id=" + Session["balancefund"] + " and ReportView.CustTypeId=" + Session["balancefundtype"];
                    else if (Flag == 3)
                        cond = " and ReportView.value_date >= '" + Session["balancefdate"] + "' and ReportView.value_date <= '" + Session["balancetdate"] + "' " + " and ReportView.branch_id=" + Session["branchno"] + " and ReportView.fund_id=" + Session["balancefund"] + " and ReportView.CustTypeId=" + Session["balancefundtype"];
                }

            }
            Flag = 0;
            Session["userid5"] = cond;
            var cond1 = "";
            var FundRight = SqlHelper.ExecuteDataset("data source=" + servername + ";initial catalog=" + DataBaseName + ";persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework", CommandType.Text, "select * from fundright where GroupID=" + Session["groupid"] + " and Auth=1");
            for (int i = 0; i <= FundRight.Tables[0].Rows.Count - 1; i++)
            {
                cond1 = "  ReportView.fund_id=" + Convert.ToInt32(FundRight.Tables[0].Rows[i].ItemArray[2]) + " or " + cond1;
            }
            int xx;
            xx = cond1.Length;
            cond1 = Microsoft.VisualBasic.Strings.Left(cond1, xx - 3);
            cond1 = " and (" + cond1 + ")";
            Session["CondFund2"] = cond1;
            var repDS = SqlHelper.ExecuteDataset("data source=" + servername + ";initial catalog=" + DataBaseName + ";persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework", CommandType.Text, "select Sum (total_sub) - Sum (total_red) as units,a.fund_id from (select sum(quantity) as total_sub,fund_id from reportview where pur_sal=0 and auth=1 " + Session["userid5"] + Session["CondFund2"] + " group by fund_id) a full outer join (select sum(quantity) as total_red ,fund_id from reportview where pur_sal=1 and auth=1 " + Session["userid5"] + Session["CondFund2"] + " group by fund_id)b on a.fund_id=b.fund_id group by a.fund_id");
            for (int i = 0; i <= repDS.Tables[0].Rows.Count - 1; i++)
            {
                if (repDS.Tables[0].Rows[i].ItemArray[0] != null)
                {
                    units = repDS.Tables[0].Rows[i].ItemArray[0].ToString();
                    fund_id = repDS.Tables[0].Rows[i].ItemArray[1].ToString();
                }
                else
                {
                    var subDS = SqlHelper.ExecuteDataset("data source=" + servername + ";initial catalog=" + DataBaseName + ";persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework", CommandType.Text, "select sum(quantity) as units,fund_id from reportview where pur_sal=0 and auth=1 " + Session["userid5"] + " and fund_id=" + repDS.Tables[0].Rows[i].ItemArray[1] + " group by fund_id");
                    if (subDS.Tables[0].Rows.Count != 0)
                    {
                        units = subDS.Tables[0].Rows[i].ItemArray[0].ToString();
                        fund_id = subDS.Tables[0].Rows[i].ItemArray[1].ToString();
                    }
                    else
                    {
                        fund_id = "0";
                        units = "0";
                    }
                }

                if (fund_id != "0")
                {
                    var fDS = SqlHelper.ExecuteDataset("data source=" + servername + ";initial catalog=" + DataBaseName + ";persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework", CommandType.Text, "select fname from fund where code=" + repDS.Tables[0].Rows[i].ItemArray[1]);
                    fname = fDS.Tables[0].Rows[0].ItemArray[0].ToString();
                }
                else
                {
                    fname = "";
                }

                var pDS = SqlHelper.ExecuteDataset("data source=" + servername + ";initial catalog=" + DataBaseName + ";persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework", CommandType.Text, " select  Price from icprice where icprice.icdate=(select max(icdate) from icprice where Fund_Id=" + fund_id + ") and Fund_Id=" + fund_id);
                try
                {
                    //navDate = pDS.Tables[0].Rows[0].ItemArray[0].ToString();
                    price = pDS.Tables[0].Rows[0].ItemArray[0].ToString();
                }
                catch (Exception)
                {

                    price = "0";
                }
                totalPos = (Convert.ToDouble(units) * Convert.ToDouble(price)).ToString();
                units = (Convert.ToDouble(units)).ToString();

                //Response.Write("<table border=" + s + "1" + s + " cellpadding=" + s + "0" + s + " cellspacing=" + s + "0" + s + "width=" + s + "1000" + s + " bordercolor=" + s + "black" + s + " style=" + s + "font-weight: bold; font-size: 10pt" + s + "> <tr> <td align=" + s + "center" + s + "style=" + s + "width: 200px; height: 18px;" + s + " rowspan=" + s + "1" + s + ">" + fund_id + "</td> <td align=" + s + "center" + s + "style=" + s + "width: 200px; height: 18px;" + s + " rowspan=" + s + "1" + s + ">" + fname + "</td> <td align=" + s + "center" + s + "style=" + s + "width: 200px; height: 18px;" + s + ">" + units + "</td> <td align=" + s + "center" + s + "style=" + s + "width: 200px; height: 18px;" + s + ">" + navDate + "</td> <td align=" + s + "center" + s + "style=" + s + "width: 200px; height: 18px;" + s + "> " + totalPos + "</td></tr> </table>")
                FundTypeData.Add(new FundTypeData
                {
                    fname = fname,
                    fund_id = fund_id,
                    price = price,
                    totalPos = totalPos,
                    units = units
                });
            }
            return FundTypeData;
        }
        public ActionResult FundType(FundTypeVM FundTypeVM)
        {
            var Data = GetData(FundTypeVM);

            LocalReport localReport = new LocalReport();
            //localReport.ReportPath = Server.MapPath("~/Reports/MovementRpt/MovementRptReport.rdlc");
            localReport.ReportPath = Server.MapPath("~/Reports/FundType/Fund_Type.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = Data;
            localReport.DataSources.Add(reportDataSource);
            List<ReportParameter> parameters = new List<ReportParameter>();
            DateTime FDate =Convert.ToDateTime (Session["balancefdate"]);
            DateTime TDate = Convert.ToDateTime(Session["balancetdate"]);
            parameters.Add(new ReportParameter("Start_Date", FDate.ToString()));
            parameters.Add(new ReportParameter("End_Date", TDate.ToString()));
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

            //return View();
        }

        //-----------------------------


    }
}