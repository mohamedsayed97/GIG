
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ICP_ABC.Areas.Account.Models;
using ICP_ABC.Reports._UsersRpt.Model;
using Microsoft.ApplicationBlocks.Data;
using ICP_ABC.Models;
using Microsoft.AspNet.Identity;
using System.Web.Configuration;
namespace ICP_ABC.Reports._UsersRpt.Controllers
{
    public class _UsersRptController : Controller
    {
        static string servername = WebConfigurationManager.AppSettings["servername"];
        static string DataBaseName = WebConfigurationManager.AppSettings["DataBaseName"];
        private string SqlCon = "data source=" + servername + ";initial catalog="+ DataBaseName + ";persist security info=True;user id=ICpro;password=ICpro!09;MultipleActiveResultSets=True;App=EntityFramework";
        ApplicationDbContext _db = new ApplicationDbContext();

        DataSet dataSet = new DataSet();
        //Context _db = new Context();
        // GET: _UsersRpt
        public ActionResult Index()
        {
            var _UsersRptVM = new _UsersRptVM
            {
                Statuses = new List<UserStatus>
                {
                    new UserStatus{Id=0, Name="All"},
                    new UserStatus{Id=1, Name="Creating"},
                    new UserStatus{Id=2, Name="Modifying"},
                    new UserStatus{Id=3, Name="Deleting"},
                    new UserStatus{Id=4, Name="Profile"},
                    new UserStatus{Id=5, Name="Locked"},
                    new UserStatus{Id=6, Name="Expired Password"},
                    new UserStatus{Id=7, Name="Unlogged Users"},
                    new UserStatus{Id=8, Name="Logged Users"}
                }
            };
            return View(_UsersRptVM);
        }

        public ActionResult _UsersRpt(_UsersRptVM VM)
        {
            var Data = GetData(VM);
            LocalReport localReport = new LocalReport();
            if (Convert.ToInt32(Session["UserStat2"]) == 1)
                localReport.ReportPath = Server.MapPath("~/Reports/_UsersRpt/_UsersRpReport1.rdlc");
            else if (Convert.ToInt32(Session["UserStat2"]) == 2)
                localReport.ReportPath = Server.MapPath("~/Reports/_UsersRpt/_UsersRpReport2.rdlc");
            else if (Convert.ToInt32(Session["UserStat2"]) == 3 || Convert.ToInt32(Session["UserStat2"]) == 4)
                localReport.ReportPath = Server.MapPath("~/Reports/_UsersRpt/_UsersRpReport3.rdlc");
            else if (Convert.ToInt32(Session["UserStat2"]) == 6 || Convert.ToInt32(Session["UserStat2"]) == 5)
                localReport.ReportPath = Server.MapPath("~/Reports/_UsersRpt/_UsersRpReport5.rdlc");
            else
                localReport.ReportPath = Server.MapPath("~/Reports/_UsersRpt/Default.rdlc");

            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = Data;
            localReport.DataSources.Add(reportDataSource);
            List<ReportParameter> parameters = new List<ReportParameter>();
            //DateTime FDate = (DateTime)Session["transfundfdate"];
            //DateTime TDate = (DateTime)Session["transfundtdate"];
            parameters.Add(new ReportParameter("FDate", Session["Userfdate"].ToString()));
            parameters.Add(new ReportParameter("TDate", Session["Usertdate"].ToString()));
            parameters.Add(new ReportParameter("Status", Session["UserStat"].ToString()));
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

        private List<_UsersRptData> GetData(_UsersRptVM VM)
        {
            var Data = new List<_UsersRptData>();
            var cond = "";
            var cond1 = "";
            var Flag = 0;

            var this_User_id = User.Identity.GetUserId();
            var this_User_data = _db.Users.Where(x => x.Id == this_User_id).FirstOrDefault();
            Session["groupid"] = this_User_data.GroupId;
            Session["userno"] = this_User_data.Code;
            //Session["userno"] = 11;
            //Session["groupid"] = 9;

            var Status = VM.status;
            if (Status != 4)
            {
                if (Status != 7)
                {
                    if (Status != 8)
                    {
                        Session["Userfdate"] = VM.FromDate.Value.ToString("yyyy-MM-dd");
                        Session["Usertdate"] = VM.ToDate.Value.ToString("yyyy-MM-dd");
                    }
                }
            }
            string Str = "";
            string Stat = "";
            string Stat1 = "";
            Session["UserStat2"] = 0;
            if (Status == 0)
            {
                Str = "select users_log.code,users_log.name,pass, convert(char(11),exp_date,103) as exp_date,branch_id,group_id,users_log.flag_tr,admin,position,branch_right,lockuser,flag_d,userid,convert(char(11),sys_date,103) as sys_date,convert(char(11),system_date,103) as system_date,type,usergroup.name as GroupName,Bname as BranchName from users_log inner join usergroup on users_log.group_id=usergroup.code inner join branch on branch.code=users_log.branch_id  where system_date >='" + Session["Userfdate"] + "' and system_date <='" + Session["Usertdate"] + "'";
                Stat = "All Data";
                Stat1 = " modified ";
                Session["UserStat2"] = 3;
            }
            else if (Status == 1)
            {
                Str = "select users_log.code,users_log.name,pass, convert(char(11),exp_date,103) as exp_date,branch_id,group_id,users_log.flag_tr,admin,position,branch_right,lockuser,flag_d,userid,convert(char(11),sys_date,103) as sys_date,convert(char(11),system_date,103) as system_date,type,usergroup.name as GroupName,Bname as BranchName from users_log inner join usergroup on users_log.group_id=usergroup.code  inner join branch on branch.code=users_log.branch_id  where type='Insert' and system_date >='" + Session["Userfdate"] + "' and system_date <='" + Session["Usertdate"] + "'";
                Stat = "Added Users";
                Stat1 = " added ";
            }
            else if (Status == 2)
            {
                Str = "select users_log.code,users_log.name,pass, convert(char(11),exp_date,103) as exp_date,branch_id,group_id,users_log.flag_tr,admin,position,branch_right,lockuser,flag_d,userid,convert(char(11),sys_date,103) as sys_date,convert(char(11),system_date,103) as system_date,type,usergroup.name as GroupName,Bname as BranchName from users_log inner join usergroup on users_log.group_id=usergroup.code inner join branch on branch.code=users_log.branch_id  where type='Update' and system_date >='" + Session["Userfdate"] + "' and system_date <='" + Session["Usertdate"] + "'";
                Stat = "Modified Users";
                Session["UserStat2"] = 4;
                Stat1 = " modified ";
            }
            else if (Status == 3)
            {
                Str = "select users_log.code,users_log.name,pass, convert(char(11),exp_date,103) as exp_date,branch_id,group_id,users_log.flag_tr,admin,position,branch_right,lockuser,flag_d,userid,convert(char(11),sys_date,103) as sys_date,convert(char(11),system_date,103) as system_date,type,usergroup.name as GroupName,Bname as BranchName from users_log inner join usergroup on users_log.group_id=usergroup.code  inner join branch on branch.code=users_log.branch_id where type='Delete' and system_date >='" + Session["Userfdate"] + "' and system_date <='" + Session["Usertdate"] + "'";
                Stat = "Deleted Users";
                Stat1 = " deleted ";
            }
            else if (Status == 4)
            {
                Str = "select users.code,users.name,pass, convert(char(11),exp_date,103) as exp_date,branch_id,group_id,users.flag_tr,admin,position,branch_right,lockuser,flag_d,userid,convert(char(11),sys_date,103) as sys_date,usergroup.Name as GroupName,Bname as BranchName from users inner join usergroup on users.group_id=usergroup.code inner join branch on  branch.code=users.branch_id";
                Stat = "Users' Profile";
                Session["UserStat2"] = 1;
            }
            else if (Status == 5)
            {
                Str = "select users_log.code,users_log.name,pass, convert(char(11),exp_date,103) as exp_date,branch_id,group_id,users_log.flag_tr,admin,position,branch_right,lockuser,flag_d,userid,convert(char(11),sys_date,103) as sys_date,convert(char(11),system_date,103) as system_date,type,usergroup.name as GroupName,Bname as BranchName from users_log inner join usergroup on users_log.group_id=usergroup.code  inner join branch on branch.code=users_log.branch_id where LockUser=1 and system_date >='" + Session["Userfdate"] + "' and system_date <='" + Session["Usertdate"] + "'";
                Stat = "Locked Users";
                Stat1 = " locked ";
            }
            else if (Status == 6)
            {
                Str = "select users.code,users.name,pass, convert(char(11),exp_date,103) as exp_date,branch_id,group_id,users.flag_tr,admin,position,branch_right,lockuser,flag_d,userid,convert(char(11),sys_date,103) as sys_date,usergroup.name as GroupName,Bname as BranchName from users inner join usergroup on users.group_id=usergroup.code  inner join branch on branch.code=users.branch_id where exp_date >='" + Session["Userfdate"] + "' and exp_date <='" + Session["Usertdate"] + "'";
                Stat = "Expired Password";
                Session["UserStat2"] = 2;
            }
            else if (Status == 7)
            {
                Str = "select *,users.name as UserName,group_id,usergroup.name as GroupName, bname as BranchName from loginusers inner join users on users.code=loginusers.usercode inner join usergroup on users.group_id=usergroup.code inner join branch on branch.code=users.branch_id where sysdate <='" + ReFormateDate2(DateTime.Now.AddDays(-30).ToShortDateString().ToString()) + "' or sysdate is null";
                Stat = "UnLogged Users";
                Session["UserStat2"] = 5;
            }
            else if (Status == 8)
            {
                Str = "select *,users.name as UserName,group_id,usergroup.name as GroupName, bname as BranchName from loginusers inner join users on users.code=loginusers.usercode inner join usergroup on users.group_id=usergroup.code inner join branch on branch.code=users.branch_id where convert(char(11),sysdate,101)='" + ReFormateDate2(VM.ToDate.ToString()) + "'";
                Stat = "Logged Users";
                Session["UserStat2"] = 6;
            }
            Session["UserStr"] = Str;
            Session["UserStat"] = Stat;
            Session["UserStat1"] = Stat1;
            if (Convert.ToInt32(Session["UserStat2"]) == 1)
            {
                var repDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, Session["UserStr"].ToString());
                string Position = "";
                if (repDS.Tables[0].Rows.Count != 0)
                {
                    for (int i = 0; i <= repDS.Tables[0].Rows.Count - 1; i++)
                    {
                        if (repDS.Tables[0].Rows[i].ItemArray[8].ToString() == "1")
                            Position = "A/C officer";
                        else if (repDS.Tables[0].Rows[i].ItemArray[8].ToString() == "2")
                            Position = "A/C officer IB Pool ";
                        else if (repDS.Tables[0].Rows[i].ItemArray[8].ToString() == "3")
                            Position = "Acting as Branch Manager";
                        else if (repDS.Tables[0].Rows[i].ItemArray[8].ToString() == "4")
                            Position = "Acting as IB Head";
                        else if (repDS.Tables[0].Rows[i].ItemArray[8].ToString() == "5")
                            Position = "Branch Manager";
                        else if (repDS.Tables[0].Rows[i].ItemArray[8].ToString() == "6")
                            Position = "Counter Officer";
                        else if (repDS.Tables[0].Rows[i].ItemArray[8].ToString() == "7")
                            Position = "Counter Officer IB Pool ";
                        else if (repDS.Tables[0].Rows[i].ItemArray[8].ToString() == "8")
                            Position = "CSR";
                        else if (repDS.Tables[0].Rows[i].ItemArray[8].ToString() == "9")
                            Position = "CSR IB Pool ";
                        else if (repDS.Tables[0].Rows[i].ItemArray[8].ToString() == "10")
                            Position = "IB Head";
                        else if (repDS.Tables[0].Rows[i].ItemArray[8].ToString() == "11")
                            Position = "Marketing";
                        else if (repDS.Tables[0].Rows[i].ItemArray[8].ToString() == "12")
                            Position = "Operations";
                        else if (repDS.Tables[0].Rows[i].ItemArray[8].ToString() == "13")
                            Position = "Operation Head";
                        else if (repDS.Tables[0].Rows[i].ItemArray[8].ToString() == "14")
                            Position = "Operation Officer";
                        else if (repDS.Tables[0].Rows[i].ItemArray[8].ToString() == "15")
                            Position = "Operation officer IB Pool ";
                        else if (repDS.Tables[0].Rows[i].ItemArray[8].ToString() == "16")
                            Position = "Sales follow-up";
                        else if (repDS.Tables[0].Rows[i].ItemArray[8].ToString() == "17")
                            Position = "Senior A/C officer";
                        else if (repDS.Tables[0].Rows[i].ItemArray[8].ToString() == "18")
                            Position = "Senior A/C officer IB Pool ";
                        else if (repDS.Tables[0].Rows[i].ItemArray[8].ToString() == "19")
                            Position = "HO.Safwa Unit Senior Officer";
                        else if (repDS.Tables[0].Rows[i].ItemArray[8].ToString() == "20")
                            Position = "HO.Safwa Unit Officer";
                        else if (repDS.Tables[0].Rows[i].ItemArray[8].ToString() == "21")
                            Position = "OM";
                        else if (repDS.Tables[0].Rows[i].ItemArray[8].ToString() == "22")
                            Position = "Marketing";
                        else if (repDS.Tables[0].Rows[i].ItemArray[8].ToString() == "23")
                            Position = "Reporting";
                        else if (repDS.Tables[0].Rows[i].ItemArray[8].ToString() == "24")
                            Position = "CALL CENTER";
                        else if (repDS.Tables[0].Rows[i].ItemArray[8].ToString() == "25")
                            Position = "System Adminstrator";
                        else if (repDS.Tables[0].Rows[i].ItemArray[8].ToString() == "26")
                            Position = "Security Officer";
                        Data.Add(new _UsersRptData
                        {
                            code = repDS.Tables[0].Rows[i].ItemArray[0].ToString(),
                            name = repDS.Tables[0].Rows[i].ItemArray[1].ToString(),
                            GroupName = repDS.Tables[0].Rows[i].ItemArray[16].ToString(),
                            BranchName = repDS.Tables[0].Rows[i].ItemArray[17].ToString(),
                            Position = Position,
                            exp_date = Convert.ToDateTime(repDS.Tables[0].Rows[i].ItemArray[3])
                        });
                    }
                }
            }
            else if (Convert.ToInt32(Session["UserStat2"]) == 2)
            {
                var repDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, Session["UserStr"].ToString());
                if (repDS.Tables[0].Rows.Count != 0)
                {
                    for (int i = 0; i <= repDS.Tables[0].Rows.Count - 1; i++)
                    {
                        Data.Add(new _UsersRptData
                        {
                            name = repDS.Tables[0].Rows[i].ItemArray[1].ToString(),
                            exp_date = Convert.ToDateTime(repDS.Tables[0].Rows[i].ItemArray[3])
                        });
                    }
                }
            }
            else if (Convert.ToInt32(Session["UserStat2"]) == 3)
            {
                var repDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, Session["UserStr"].ToString());
                var forDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select distinct users_log.code from users_log where type='Update' or type='Insert' or type='Delete' and system_date >='" + Session["Userfdate"] + "' and system_date <='" + Session["Usertdate"] + "'");
                if (repDS.Tables[0].Rows.Count != 0)
                {
                    for (int i = 0; i <= forDS.Tables[0].Rows.Count - 1; i++)
                    {
                        var crtDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select  * from ( select users_log.code,users_log.name,pass, convert(char(11),exp_date,103) as exp_date,branch_id,group_id,users_log.flag_tr,admin,position,branch_right,lockuser,flag_d,userid,convert(char(11),sys_date,103) as sys_date,convert(char(11),system_date,103) as system_date,type ,usergroup.name as groupname,branch.bname as branchName from users_log inner join usergroup on usergroup.code=users_log.group_id inner join branch on branch.code=users_log.branch_id where users_log.code=" + forDS.Tables[0].Rows[i].ItemArray[0] + "  and type<>'before delete'  and system_date >'" + Session["Userfdate"] + "' ) a where a.type='Insert'");
                        if (crtDS.Tables[0].Rows.Count != 0)
                        {
                            Data.Add(new _UsersRptData
                            {
                                code = crtDS.Tables[0].Rows[crtDS.Tables[0].Rows.Count - 1].ItemArray[0].ToString(),
                                System_date = Convert.ToDateTime(crtDS.Tables[0].Rows[crtDS.Tables[0].Rows.Count - 1].ItemArray[14]),
                                Type = crtDS.Tables[0].Rows[crtDS.Tables[0].Rows.Count - 1].ItemArray[15].ToString(),
                                name = crtDS.Tables[0].Rows[crtDS.Tables[0].Rows.Count - 1].ItemArray[1].ToString(),
                                GroupName = crtDS.Tables[0].Rows[crtDS.Tables[0].Rows.Count - 1].ItemArray[16].ToString(),
                                BranchName = crtDS.Tables[0].Rows[crtDS.Tables[0].Rows.Count - 1].ItemArray[17].ToString(),
                            });
                        }
                        var modDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select  * from ( select users_log.code,users_log.name,pass, convert(char(11),exp_date,103) as exp_date,branch_id,group_id,users_log.flag_tr,admin,position,branch_right,lockuser,flag_d,userid,convert(char(11),sys_date,103) as sys_date,convert(char(11),system_date,103) as system_date,type ,usergroup.name as groupname,branch.bname as branchName from users_log inner join usergroup on usergroup.code=users_log.group_id inner join branch on branch.code=users_log.branch_id where users_log.code=" + forDS.Tables[0].Rows[i].ItemArray[0] + "  and type<>'before delete'  and system_date >'" + Session["Userfdate"] + "' ) a where a.type='Update'");
                        if (modDS.Tables[0].Rows.Count != 0)
                        {
                            if (modDS.Tables[0].Rows.Count > 1)
                            {
                                Data.Add(new _UsersRptData
                                {
                                    code = modDS.Tables[0].Rows[modDS.Tables[0].Rows.Count - 1].ItemArray[0].ToString(),
                                    System_date = Convert.ToDateTime(modDS.Tables[0].Rows[modDS.Tables[0].Rows.Count - 1].ItemArray[14]),
                                    Type = modDS.Tables[0].Rows[modDS.Tables[0].Rows.Count - 1].ItemArray[15].ToString(),
                                    name = modDS.Tables[0].Rows[modDS.Tables[0].Rows.Count - 2].ItemArray[1].ToString(),
                                    GroupName = modDS.Tables[0].Rows[modDS.Tables[0].Rows.Count - 2].ItemArray[16].ToString(),
                                    BranchName = modDS.Tables[0].Rows[modDS.Tables[0].Rows.Count - 2].ItemArray[17].ToString(),
                                    name2 = modDS.Tables[0].Rows[modDS.Tables[0].Rows.Count - 1].ItemArray[1].ToString(),
                                    GroupName2 = modDS.Tables[0].Rows[modDS.Tables[0].Rows.Count - 1].ItemArray[16].ToString(),
                                    BranchName2 = modDS.Tables[0].Rows[modDS.Tables[0].Rows.Count - 1].ItemArray[17].ToString(),
                                });
                            }
                            else if (modDS.Tables[0].Rows.Count == 1)
                            {
                                Data.Add(new _UsersRptData
                                {
                                    code = modDS.Tables[0].Rows[modDS.Tables[0].Rows.Count - 1].ItemArray[0].ToString(),
                                    System_date = Convert.ToDateTime(modDS.Tables[0].Rows[modDS.Tables[0].Rows.Count - 1].ItemArray[14]),
                                    Type = modDS.Tables[0].Rows[modDS.Tables[0].Rows.Count - 1].ItemArray[15].ToString(),
                                    name = crtDS.Tables[0].Rows[crtDS.Tables[0].Rows.Count - 1].ItemArray[1].ToString(),
                                    GroupName = crtDS.Tables[0].Rows[crtDS.Tables[0].Rows.Count - 1].ItemArray[16].ToString(),
                                    BranchName = crtDS.Tables[0].Rows[crtDS.Tables[0].Rows.Count - 1].ItemArray[17].ToString(),
                                    name2 = modDS.Tables[0].Rows[modDS.Tables[0].Rows.Count - 1].ItemArray[1].ToString(),
                                    GroupName2 = modDS.Tables[0].Rows[modDS.Tables[0].Rows.Count - 1].ItemArray[16].ToString(),
                                    BranchName2 = modDS.Tables[0].Rows[modDS.Tables[0].Rows.Count - 1].ItemArray[17].ToString()
                                });
                            }

                        }
                        var delDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select  * from ( select users_log.code,users_log.name,pass, convert(char(11),exp_date,103) as exp_date,branch_id,group_id,users_log.flag_tr,admin,position,branch_right,lockuser,flag_d,userid,convert(char(11),sys_date,103) as sys_date,convert(char(11),system_date,103) as system_date,type ,usergroup.name as groupname,branch.bname as branchName from users_log inner join usergroup on usergroup.code=users_log.group_id inner join branch on branch.code=users_log.branch_id where users_log.code=" + forDS.Tables[0].Rows[i].ItemArray[0] + "  and type<>'before delete'  and system_date >'" + Session["Userfdate"] + "' ) a where a.type='Delete'");
                        if (delDS.Tables[0].Rows.Count != 0)
                        {
                            Data.Add(new _UsersRptData
                            {
                                code = delDS.Tables[0].Rows[delDS.Tables[0].Rows.Count - 1].ItemArray[0].ToString(),
                                System_date = Convert.ToDateTime(delDS.Tables[0].Rows[delDS.Tables[0].Rows.Count - 1].ItemArray[14]),
                                Type = delDS.Tables[0].Rows[delDS.Tables[0].Rows.Count - 1].ItemArray[15].ToString(),
                                name = delDS.Tables[0].Rows[delDS.Tables[0].Rows.Count - 1].ItemArray[1].ToString(),
                                GroupName = delDS.Tables[0].Rows[delDS.Tables[0].Rows.Count - 1].ItemArray[16].ToString(),
                                BranchName = delDS.Tables[0].Rows[delDS.Tables[0].Rows.Count - 1].ItemArray[17].ToString()
                            });
                        }
                    }
                }
            }
            else if (Convert.ToInt32(Session["UserStat2"]) == 5)
            {
                var repDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, Session["UserStr"].ToString());
                if (repDS.Tables[0].Rows.Count != 0)
                {
                    for (int i = 0; i <= repDS.Tables[0].Rows.Count - 1; i++)
                    {
                        string stat;
                        if (repDS.Tables[0].Rows[i].ItemArray[13] == null)
                            stat = "Never Loged";
                        else
                            stat = repDS.Tables[0].Rows[i].ItemArray[13].ToString();

                        Data.Add(new _UsersRptData
                        {
                            code = repDS.Tables[0].Rows[i].ItemArray[0].ToString(),
                            name = repDS.Tables[0].Rows[i].ItemArray[1].ToString(),
                            GroupName = repDS.Tables[0].Rows[i].ItemArray[16].ToString(),
                            BranchName = repDS.Tables[0].Rows[i].ItemArray[17].ToString(),
                            exp_date = Convert.ToDateTime(ReFormateDate2(repDS.Tables[0].Rows[i].ItemArray[3].ToString()))
                        });
                    }
                }
            }
            else if (Convert.ToInt32(Session["UserStat2"]) == 6)
            {
                var repDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, Session["UserStr"].ToString());
                if (repDS.Tables[0].Rows.Count != 0)
                {
                    for (int i = 0; i <= repDS.Tables[0].Rows.Count - 1; i++)
                    {
                        string stat;
                        stat = repDS.Tables[0].Rows[i].ItemArray[13].ToString();
                        Data.Add(new _UsersRptData
                        {
                            code = repDS.Tables[0].Rows[i].ItemArray[0].ToString(),
                            name = repDS.Tables[0].Rows[i].ItemArray[1].ToString(),
                            GroupName = repDS.Tables[0].Rows[i].ItemArray[16].ToString(),
                            BranchName = repDS.Tables[0].Rows[i].ItemArray[17].ToString(),
                            exp_date = Convert.ToDateTime(ReFormateDate2(repDS.Tables[0].Rows[i].ItemArray[3].ToString()))
                        });
                    }
                }
            }
            else if (Convert.ToInt32(Session["UserStat2"]) == 4)
            {
                var repDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, Session["UserStr"].ToString());
                var forDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select distinct users_log.code from users_log where type='Update' and system_date >='" + Session["Userfdate"] + "' and system_date <='" + Session["Usertdate"] + "'");
                if (repDS.Tables[0].Rows.Count != 0)
                {
                    for (int i = 0; i <= forDS.Tables[0].Rows.Count - 1; i++)
                    {
                        var modDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select users_log.code,users_log.name,pass, convert(char(11),exp_date,103) as exp_date,branch_id,group_id,users_log.flag_tr,admin,position,branch_right,lockuser,flag_d,userid,convert(char(11),sys_date,103) as sys_date,convert(char(11),system_date,103) as system_date,type ,usergroup.name as groupname,branch.bname as branchName from users_log inner join usergroup on usergroup.code=users_log.group_id inner join branch on branch.code=users_log.branch_id where users_log.code=" + forDS.Tables[0].Rows[i].ItemArray[0] + " and type<>'delete' and type<>'before delete'  and system_date >'" + Session["Userfdate"] + "'");
                        if (modDS.Tables[0].Rows.Count != 0)
                        {
                            Data.Add(new _UsersRptData
                            {
                                code = modDS.Tables[0].Rows[modDS.Tables[0].Rows.Count - 1].ItemArray[0].ToString(),
                                System_date = Convert.ToDateTime(modDS.Tables[0].Rows[modDS.Tables[0].Rows.Count - 1].ItemArray[14]),
                                Type = modDS.Tables[0].Rows[modDS.Tables[0].Rows.Count - 1].ItemArray[15].ToString(),
                                name = modDS.Tables[0].Rows[modDS.Tables[0].Rows.Count - 2].ItemArray[1].ToString(),
                                GroupName = modDS.Tables[0].Rows[modDS.Tables[0].Rows.Count - 2].ItemArray[16].ToString(),
                                BranchName = modDS.Tables[0].Rows[modDS.Tables[0].Rows.Count - 2].ItemArray[17].ToString(),
                                name2 = modDS.Tables[0].Rows[modDS.Tables[0].Rows.Count - 1].ItemArray[1].ToString(),
                                GroupName2 = modDS.Tables[0].Rows[modDS.Tables[0].Rows.Count - 1].ItemArray[16].ToString(),
                                BranchName2 = modDS.Tables[0].Rows[modDS.Tables[0].Rows.Count - 1].ItemArray[17].ToString()
                            });
                        }
                    }
                }
            }
            else
            {
                var repDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, Session["UserStr"].ToString());
                if (repDS.Tables[0].Rows.Count != 0)
                {
                    for (int i = 0; i <= repDS.Tables[0].Rows.Count - 1; i++)
                    {
                        var userDS = SqlHelper.ExecuteDataset(SqlCon, CommandType.Text, "select distinct name from users_log where code = " + repDS.Tables[0].Rows[i].ItemArray[12]);
                        Data.Add(new _UsersRptData
                        {
                            code = repDS.Tables[0].Rows[i].ItemArray[0].ToString(),
                            name = repDS.Tables[0].Rows[i].ItemArray[1].ToString(),
                            GroupName = repDS.Tables[0].Rows[i].ItemArray[16].ToString(),
                            BranchName = repDS.Tables[0].Rows[i].ItemArray[17].ToString(),
                            System_date = Convert.ToDateTime(ReFormateDate2(repDS.Tables[0].Rows[i].ItemArray[14].ToString()))
                        });
                    }
                }
            }
            return Data;
        }

        public string ReFormateDate2(String d)
        {
            int dd, mm, yy;
            dd = Convert.ToInt32(d.ToString().Substring(0, 2));
            mm = Convert.ToInt32(d.ToString().Substring(3, 2));
            yy = Convert.ToInt32(d.ToString().Substring(6, 4));
            DateTime ndate = new DateTime(yy, mm, dd);
            return ndate.ToString("MM/dd/yyyy");
        }
    }
}