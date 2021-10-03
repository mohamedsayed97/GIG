using Microsoft.ApplicationBlocks.Data;
using Microsoft.Reporting.WebForms;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ICP_ABC.Models;
using ICP_ABC.Reports.branchAverage.Model;
using Microsoft.AspNet.Identity;
using System.Web.Configuration;
namespace ICP_ABC.Reports.branchAverage.Controllers
{
    public class branchAverageController : Controller
    {
        static string servername = WebConfigurationManager.AppSettings["servername"];

        static string DataBaseName = WebConfigurationManager.AppSettings["DataBaseName"];
        private string SqlCon = "data source=" + servername + ";initial catalog=" + DataBaseName + ";persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework";

        //private string SqlCon = "data source=" + servername + ";initial catalog=LastUpdateDB;persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework";
        ApplicationDbContext _db = new ApplicationDbContext();

        DataSet dataSet = new DataSet();
        // private string SqlCon = "data source=SERVER2;initial catalog=IC_AWB2;persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework";

        //Context _db = new Context();
        // GET: branchAverage
        public ActionResult Index()
        {
            var branchAverageVM = new branchAverageVM
            {
                Funds = _db.Funds.ToList(),
                ClientTypes = new List<ClientType>
                {
                    new ClientType
                    {
                        Id=0,
                        Name="No Types"
                    },
                    new ClientType
                    {
                        Id=1,
                        Name="SAFWA"
                    },
                    new ClientType
                    {
                        Id=4,
                        Name="LOCAL BANKS"
                    },
                    new ClientType
                    {
                        Id=5,
                        Name="SG HEAD OFF. & BRANCHES"
                    },
                    new ClientType
                    {
                        Id=7,
                        Name="COMPANIES"
                    },
                    new ClientType
                    {
                        Id=8,
                        Name="ONE MAN INSTITUTION"
                    },
                    new ClientType
                    {
                        Id=9,
                        Name="INDIVIDUAL"
                    }
                }
            };

            return View(branchAverageVM);
        }

        public ActionResult branchAverage(branchAverageVM VM)
        {
            var Data = GetData(VM);
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/branchAverage/branchAverageReport.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = Data;
            localReport.DataSources.Add(reportDataSource);
            List<ReportParameter> parameters = new List<ReportParameter>();
            //DateTime FDate = (DateTime)Session["transfundfdate"];
            //DateTime TDate = (DateTime)Session["transfundtdate"];
            parameters.Add(new ReportParameter("FDate", Session["ref8fdate"].ToString()));
            parameters.Add(new ReportParameter("TDate", Session["ref8tdate"].ToString()));
            // parameters.Add(new ReportParameter("TransactionOver", Session["transover"].ToString()));
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

        public List<branchAverageData> GetData(branchAverageVM VM)
        {
            var Data = new List<branchAverageData>();
            var cond = "";
            var cond1 = "";
            var Flag = 0;
            var Fund_Name = VM.Fund;
            var ClientType = VM.ClientType;

            var this_User_id = User.Identity.GetUserId();
            var this_User_data = _db.Users.Where(x => x.Id == this_User_id).FirstOrDefault();
            Session["groupid"] = this_User_data.GroupId;
            Session["userno"] = this_User_data.Code;
            //Session["groupid"] = 5;
            //Session["userno"] = 259;

            //Session["userno"] = 11;
            //Session["groupid"] = 9;
            Session["ref8fdate"] = VM.FromDate.Value.ToString("yyyy-MM-dd");
            Session["ref8tdate"] = VM.ToDate.Value.ToString("yyyy-MM-dd");
            var druser = SqlHelper.ExecuteDataset(SqlCon, "Sp_druser", Convert.ToInt32(Session["userno"]));
            var branch_id = druser.Tables[0].Rows[0].ItemArray[0];
            var branch_right = druser.Tables[0].Rows[0].ItemArray[1];
            if (Convert.ToBoolean(branch_right) == true)
            {
                Session["branchno6"] = 0;
                Flag = 2;
            }
            else
            {
                Session["branchno6"] = Convert.ToInt32(branch_id);
                Flag = 3;
            }
            Session["Bycust"] = 0;
            if (VM.byCustomer == true)
                Session["Bycust"] = 1;
            if (VM.ByBranch == false)
                Session["repno"] = 77;
            if (Convert.ToInt32(Session["Bycust"]) == 0)
            {
                if (Convert.ToInt32(Session["ref8fundtype"]) == 0)
                {
                    if (Flag == 1)
                        cond = " where ReportView.value_date >= '" + Session["ref8fdate"] + "' and ReportView.value_date <= '" + Session["ref8tdate"] + "'   ";
                    else if (Flag == 2)
                        cond = " where ReportView.value_date >= '" + Session["ref8fdate"] + "' and ReportView.value_date <= '" + Session["ref8tdate"] + "'  ";
                    else if (Flag == 3)
                        cond = " where ReportView. value_date >= '" + Session["ref8fdate"] + "' and ReportView.value_date <= '" + Session["ref8tdate"] + "' " + " and ReportView.branch_id=" + Session["branchno6"];
                }
                else
                {
                    if (Flag == 1)
                        cond = " where ReportView.value_date >= '" + Session["ref8fdate"] + "' and ReportView.value_date <= '" + Session["ref8tdate"] + "' and ReportView.CustTypeId =" + Session["ref8fundtype"] + "   ";
                    else if (Flag == 2)
                        cond = " where ReportView.value_date >= '" + Session["ref8fdate"] + "' and ReportView.value_date <= '" + Session["ref8tdate"] + "' " + " and ReportView.CustTypeId =" + Session["ref8fundtype"];
                    else if (Flag == 3)
                        cond = " where ReportView.value_date >= '" + Session["ref8fdate"] + "' and ReportView.value_date <= '" + Session["ref8tdate"] + "' " + " and ReportView.branch_id=" + Session["branchno6"] + " and ReportView.CustTypeId =" + Session["ref8fundtype"];
                }
            }
            else
            {
                if (Convert.ToInt32(Session["ref8fundtype"]) == 0)
                {
                    if (Flag == 1)
                        cond = " where value_date >= '" + Session["ref8fdate"] + "' and value_date <= '" + Session["ref8tdate"] + "'  ";
                    else if (Flag == 2)
                        cond = " where value_date >= '" + Session["ref8fdate"] + "' and value_date <= '" + Session["ref8tdate"] + "' ";
                    else if (Flag == 3)
                        cond = " where value_date >= '" + Session["ref8fdate"] + "' and value_date <= '" + Session["ref8tdate"] + "' " + " and branch_id=" + Session["branchno6"];
                }
                else
                {
                    if (Flag == 1)
                        cond = " where value_date >= '" + Session["ref8fdate"] + "' and value_date <= '" + Session["ref8tdate"] + "' and CustTypeId =" + Session["ref8fundtype"] + "  ";
                    else if (Flag == 2)
                        cond = " where value_date >= '" + Session["ref8fdate"] + "' and value_date <= '" + Session["ref8tdate"] + "' " + " and CustTypeId =" + Session["ref8fundtype"];
                    else if (Flag == 3)
                        cond = " where value_date >= '" + Session["ref8fdate"] + "' and value_date <= '" + Session["ref8tdate"] + "' " + " and branch_id=" + Session["branchno6"] + " and CustTypeId =" + Session["ref8fundtype"];
                }
            }
            Flag = 0;
            Session["userid77"] = cond;
            var FundRight2 = SqlHelper.ExecuteDataset(SqlCon, "Sp_FundRight1", Session["groupid"]);
            for (int i = 0; i <= FundRight2.Tables[0].Rows.Count - 1; i++)
            {
                cond1 = "  TransOver.fund_id=" + FundRight2.Tables[0].Rows[i].ItemArray[2] + " or " + cond1;
            }
            var xx2 = 0;
            xx2 = cond1.Length;
            cond1 = Microsoft.VisualBasic.Strings.Left(cond1, xx2 - 3);
            cond1 = " and (" + cond1 + ")";
            Session["CondFund9"] = cond1;

            if (Convert.ToInt32(Session["Bycust"]) == 0)
            {

                var days = ((TimeSpan)(Convert.ToDateTime(Session["ref8tdate"]) - Convert.ToDateTime(Session["ref8fdate"]))).Days;
                string nav, nav_all, tot, SRtot, AvrTot, s;
                string Stotal = "0";
                string Rtotal = "0";
                var CTDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select distinct CustTypeId from reportview " + Session["userid77"]);
                for (int i = 0; i <= CTDS.Tables[0].Rows.Count - 1; i++)
                {
                    int Icbo_type = 0;
                    string cbo_type;
                    if (Convert.ToInt32(CTDS.Tables[0].Rows[i].ItemArray[0]) == 1)
                    {
                        Icbo_type = 1;
                        cbo_type = "Safwa";
                    }
                    else if (Convert.ToInt32(CTDS.Tables[0].Rows[i].ItemArray[0]) == 2)
                    {
                        Icbo_type = 2;
                        cbo_type = "CENTRAL BANK OF EGYPT";
                    }
                    else if (Convert.ToInt32(CTDS.Tables[0].Rows[i].ItemArray[0]) == 3)
                    {
                        Icbo_type = 3;
                        cbo_type = "NATIONAL BANK OF EGYPT";
                    }
                    else if (Convert.ToInt32(CTDS.Tables[0].Rows[i].ItemArray[0]) == 4)
                    {
                        Icbo_type = 4;
                        cbo_type = "LOCAL BANKS";
                    }
                    else if (Convert.ToInt32(CTDS.Tables[0].Rows[i].ItemArray[0]) == 5)
                    {
                        Icbo_type = 5;
                        cbo_type = "SG HEAD OFF. & BRANCHES";
                    }
                    else if (Convert.ToInt32(CTDS.Tables[0].Rows[i].ItemArray[0]) == 6)
                    {
                        Icbo_type = 6;
                        cbo_type = "FOREIGN BANKS";
                    }
                    else if (Convert.ToInt32(CTDS.Tables[0].Rows[i].ItemArray[0]) == 7)
                    {
                        Icbo_type = 7;
                        cbo_type = "COMPANIES";
                    }
                    else if (Convert.ToInt32(CTDS.Tables[0].Rows[i].ItemArray[0]) == 8)
                    {
                        Icbo_type = 8;
                        cbo_type = "ONE MAN INSTITUTION";
                    }
                    else if (Convert.ToInt32(CTDS.Tables[0].Rows[i].ItemArray[0]) == 9)
                    {
                        Icbo_type = 9;
                        cbo_type = "INDIVIDUAL";
                    }
                    else if (Convert.ToInt32(CTDS.Tables[0].Rows[i].ItemArray[0]) == 10)
                    {
                        Icbo_type = 10;
                        cbo_type = "MISCEALLENOUS";
                    }
                    else
                    {
                        Icbo_type = 0;
                        cbo_type = "No Types";
                    }
                    var FundDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select  distinct fund_id, fname  from ReportView " + Session["userid77"] + "  and CustTypeId = " + CTDS.Tables[0].Rows[i].ItemArray[0]);
                    for (int j = 0; j <= FundDS.Tables[0].Rows.Count - 1; j++)
                    {
                        var Price_DS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select Icprice.Price from Icprice where Icprice.icdate=(select max(icdate) from icprice where FundId=" + FundDS.Tables[0].Rows[j].ItemArray[0] + ") and FundId=" + FundDS.Tables[0].Rows[j].ItemArray[0]);
                        var Price = Price_DS.Tables[0].Rows[0].ItemArray[0];
                        string itotal_sub, itotal_red, inav, AVG, itot = "0";
                        var repDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select distinct (isnull(a.branch_id,'')) as branch_id,isnull(reportview.Bname,'') as Bname,isnull(a.total_sub,0) as total_sub,isnull(b.total_red,0)as total_red,isnull(a.total_sub,0) - isnull(b.total_red,0) as tot,(isnull(a.total_sub,0) - isnull(b.total_red,0)) *" + Price + " as nav from (select sum(quantity)as total_sub,branch_id  from ReportView   " + Session["userid77"] + " and pur_sal=0 and Fund_id=" + FundDS.Tables[0].Rows[j].ItemArray[0] + " and CustTypeId =" + CTDS.Tables[0].Rows[i].ItemArray[0] + " group by branch_id  )a full outer join (select sum(quantity)as total_red,branch_id from ReportView  " + Session["userid77"] + " and pur_sal=1 and fund_id=" + FundDS.Tables[0].Rows[j].ItemArray[0] + " and CustTypeId = " + CTDS.Tables[0].Rows[i].ItemArray[0] + " group by branch_id)b on a.branch_id=b.branch_id full outer join reportview on a.branch_id=reportview.branch_id or b.branch_id=reportview.branch_id " + Session["userid77"] + " and fund_id=" + FundDS.Tables[0].Rows[j].ItemArray[0] + " and CustTypeId = " + CTDS.Tables[0].Rows[i].ItemArray[0]);
                        for (int k = 0; k <= repDS.Tables[0].Rows.Count - 1; k++)
                        {
                            tot = (Convert.ToDouble(repDS.Tables[0].Rows[k].ItemArray[2]) - Convert.ToDouble(repDS.Tables[0].Rows[k].ItemArray[3])).ToString();
                            nav = (Convert.ToDouble(tot) * Convert.ToDouble(Price)).ToString();
                            AVG = (Convert.ToDouble(tot) / Convert.ToDouble(days)).ToString();
                            Stotal = (Convert.ToDouble(Stotal) + Convert.ToDouble(repDS.Tables[0].Rows[k].ItemArray[2])).ToString();
                            Rtotal = (Convert.ToDouble(Rtotal) + Convert.ToDouble(repDS.Tables[0].Rows[k].ItemArray[3])).ToString();
                            SRtot = (Convert.ToDouble(Stotal) - Convert.ToDouble(Rtotal)).ToString();
                            nav_all = (Convert.ToDouble(SRtot) * Convert.ToDouble(Price)).ToString();
                            AvrTot = (Convert.ToDouble(SRtot) / Convert.ToDouble(days)).ToString();
                            Data.Add(new branchAverageData
                            {
                                cbo_type = cbo_type,
                                Fname = FundDS.Tables[0].Rows[j].ItemArray[1].ToString(),
                                Price = Price.ToString(),
                                branch_id = repDS.Tables[0].Rows[k].ItemArray[0].ToString(),
                                BName = repDS.Tables[0].Rows[k].ItemArray[1].ToString(),
                                total_sub = Convert.ToDouble(repDS.Tables[0].Rows[k].ItemArray[2]),
                                total_red = Convert.ToDouble(repDS.Tables[0].Rows[k].ItemArray[3]),
                                tot = Convert.ToDouble(tot),
                                nav = Convert.ToDouble(nav),
                                AVG = Convert.ToDouble(AVG),
                            });
                        }
                        tot = "0";
                        nav = "0";
                        Stotal = "0";
                        Rtotal = "0";
                        SRtot = "0";
                        nav_all = "0";
                        AvrTot = "0";
                    }
                }

            }
            else
            {
                string nav, nav_all, tot, SRtot, AvrTot, s;
                string Stotal = "0";
                string Rtotal = "0";
                var days = ((TimeSpan)(Convert.ToDateTime(Session["ref8tdate"]) - Convert.ToDateTime(Session["ref8fdate"]))).Days;
                var CTDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select distinct CustTypeId  from reportview1 " + Session["userid77"]);
                for (int i = 0; i <= CTDS.Tables[0].Rows.Count - 1; i++)
                {
                    int Icbo_type = 0;
                    string cbo_type;
                    if (Convert.ToInt32(CTDS.Tables[0].Rows[i].ItemArray[0]) == 1)
                    {
                        Icbo_type = 1;
                        cbo_type = "Safwa";
                    }
                    else if (Convert.ToInt32(CTDS.Tables[0].Rows[i].ItemArray[0]) == 2)
                    {
                        Icbo_type = 2;
                        cbo_type = "CENTRAL BANK OF EGYPT";
                    }
                    else if (Convert.ToInt32(CTDS.Tables[0].Rows[i].ItemArray[0]) == 3)
                    {
                        Icbo_type = 3;
                        cbo_type = "NATIONAL BANK OF EGYPT";
                    }
                    else if (Convert.ToInt32(CTDS.Tables[0].Rows[i].ItemArray[0]) == 4)
                    {
                        Icbo_type = 4;
                        cbo_type = "LOCAL BANKS";
                    }
                    else if (Convert.ToInt32(CTDS.Tables[0].Rows[i].ItemArray[0]) == 5)
                    {
                        Icbo_type = 5;
                        cbo_type = "SG HEAD OFF. & BRANCHES";
                    }
                    else if (Convert.ToInt32(CTDS.Tables[0].Rows[i].ItemArray[0]) == 6)
                    {
                        Icbo_type = 6;
                        cbo_type = "FOREIGN BANKS";
                    }
                    else if (Convert.ToInt32(CTDS.Tables[0].Rows[i].ItemArray[0]) == 7)
                    {
                        Icbo_type = 7;
                        cbo_type = "COMPANIES";
                    }
                    else if (Convert.ToInt32(CTDS.Tables[0].Rows[i].ItemArray[0]) == 8)
                    {
                        Icbo_type = 8;
                        cbo_type = "ONE MAN INSTITUTION";
                    }
                    else if (Convert.ToInt32(CTDS.Tables[0].Rows[i].ItemArray[0]) == 9)
                    {
                        Icbo_type = 9;
                        cbo_type = "INDIVIDUAL";
                    }
                    else if (Convert.ToInt32(CTDS.Tables[0].Rows[i].ItemArray[0]) == 10)
                    {
                        Icbo_type = 10;
                        cbo_type = "MISCEALLENOUS";
                    }
                    else
                    {
                        Icbo_type = 0;
                        cbo_type = "No Types";
                    }
                    var FundDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select distinct ReportView1.fund_id, fname from ReportView1 " + Session["userid77"] + "  and CustTypeId = " + CTDS.Tables[0].Rows[i].ItemArray[0]);
                    for (int j = 0; j <= FundDS.Tables[0].Rows.Count - 1; j++)
                    {
                        var Price_DS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select Icprice.Price from Icprice where Icprice.icdate=(select max(icdate) from icprice where FundId=" + FundDS.Tables[0].Rows[j].ItemArray[0] + ") and FundId=" + FundDS.Tables[0].Rows[j].ItemArray[0]);
                        var Price = Price_DS.Tables[0].Rows[0].ItemArray[0];
                        var itotal_sub = 0;
                        var itotal_red = 0;
                        var inav = 0;
                        var AVG = "0";
                        var itot = 0;
                        var repDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select distinct(isnull(a.branch_id,'')) as branch_id,isnull(reportview1.Bname,'') as Bname,isnull(a.total_sub,0) as total_sub,isnull(b.total_red,0)as total_red,isnull(a.total_sub,0) - isnull(b.total_red,0) as tot,(isnull(a.total_sub,0) - isnull(b.total_red,0)) *" + Price + " as nav from (select sum(quantity)as total_sub,branch_id  from ReportView1  " + Session["userid77"] + " and pur_sal=0 and Fund_id=" + FundDS.Tables[0].Rows[j].ItemArray[0] + " and CustTypeId =" + CTDS.Tables[0].Rows[i].ItemArray[0] + " group by branch_id  )a full outer join (select sum(quantity)as total_red,branch_id from ReportView1   " + Session["userid77"] + " and pur_sal=1 and Fund_id=" + FundDS.Tables[0].Rows[j].ItemArray[0] + " and CustTypeId =" + CTDS.Tables[0].Rows[i].ItemArray[0] + " group by branch_id)b on a.branch_id=b.branch_id full outer join reportview1 on a.branch_id=reportview1.branch_id or b.branch_id=reportview1.branch_id " + Session["userid77"] + " and fund_id=" + FundDS.Tables[0].Rows[j].ItemArray[0] + " and CustTypeId = " + CTDS.Tables[0].Rows[i].ItemArray[0]);
                        for (int k = 0; k <= repDS.Tables[0].Rows.Count - 1; k++)
                        {
                            tot = (Convert.ToDouble(repDS.Tables[0].Rows[k].ItemArray[2]) - Convert.ToDouble(repDS.Tables[0].Rows[k].ItemArray[3])).ToString();
                            nav = (Convert.ToDouble(tot) * Convert.ToDouble(Price)).ToString();
                            AVG = (Convert.ToDouble(tot) / Convert.ToDouble(days)).ToString();
                            Stotal = (Convert.ToDouble(Stotal) + Convert.ToDouble(repDS.Tables[0].Rows[k].ItemArray[2])).ToString();
                            Rtotal = (Convert.ToDouble(Rtotal) + Convert.ToDouble(repDS.Tables[0].Rows[k].ItemArray[3])).ToString();
                            SRtot = (Convert.ToDouble(Stotal) - Convert.ToDouble(Rtotal)).ToString();
                            nav_all = (Convert.ToDouble(SRtot) * Convert.ToDouble(Price)).ToString();
                            AvrTot = (Convert.ToDouble(SRtot) / Convert.ToDouble(days)).ToString();
                            Data.Add(new branchAverageData
                            {
                                cbo_type = cbo_type,
                                Fname = FundDS.Tables[0].Rows[j].ItemArray[1].ToString(),
                                Price = Price.ToString(),
                                branch_id = repDS.Tables[0].Rows[k].ItemArray[0].ToString(),
                                BName = repDS.Tables[0].Rows[k].ItemArray[1].ToString(),
                                total_sub = Convert.ToDouble(repDS.Tables[0].Rows[k].ItemArray[2]),
                                total_red = Convert.ToDouble(repDS.Tables[0].Rows[k].ItemArray[3]),
                                tot = Convert.ToDouble(tot),
                                nav = Convert.ToDouble(nav),
                                AVG = Convert.ToDouble(AVG),
                            });
                        }
                        tot = "0";
                        nav = "0";
                        Stotal = "0";
                        Rtotal = "0";
                        SRtot = "0";
                        nav_all = "0";
                        AvrTot = "0";
                    }
                }
            }
            return Data;
        }
    }
}