using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Net.Sockets;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using ClientGetInfo;
using GetInfo.GetInfo;
using Microsoft.AspNet.Identity;
using Oracle.ManagedDataAccess.Client;
using MRSocket.MRSocket;

namespace ICP_ABC.Areas.Customers.Controllers
{
    public class GustomerInfoController : Controller
    {
        public string GetCustInfo(string CustCode , string  IPAdress , string PortNo , string userno )
        {
            var client = new TcpClient();

            SocketModule.sendtoserver(CustCode,IPAdress,int.Parse(PortNo), "027",userno, client);
            var cipher = new Cipher();
            string oradb = cipher.Decryption(ConfigurationManager.AppSettings["ORCL_ConStr"]);
            //"Data Source=ORCL;User Id=hr;Password=hr;";
            OracleConnection conn = new OracleConnection(oradb);
            DateTime time = DateTime.Now.AddSeconds(30);
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                return "Error 01; Client Info. Name Connection Error";
            }

            string[] StartClientArray = SocketModule.StartClient("027", client);
            if (StartClientArray == null) { 
                return "Error 02; Client Info. Name Return failed ";
            }
           else if (StartClientArray[0]=="5")
            {
                return "Error 03; Client Info. Name Invaild Account Data";
            }
            try
            {
                string ename, aname;
                ename = StartClientArray[0].Trim();
                aname = StartClientArray[1].Trim();

                return ename + "|" + aname;
            }
            catch (Exception)
            {

                return "Error 04; Client Info. Name No Data Found";
            }
  
        }

        public string addressInfo(string CustCode, string IPAdress, string PortNo, string userno)
        {
            // here get customer address
            var client1 = new TcpClient();
            try
            {
                SocketModule.sendtoserver(CustCode, IPAdress, int.Parse(PortNo), "028", userno, client1);
            }    
            catch( Exception e)
            {

                return "Error 01; Client Info. Name Connection Error";
            }


            string[] StartClientArray = SocketModule.StartClient("028", client1);
            if (StartClientArray == null)
            {
                return "Error 02; Client Info. Name Return failed ";
            }
            else if(StartClientArray[0] == "5")
            {
                return "Error 03; Client Info. Name Invaild Account Data";
            }
            string eaddress1, eaddress2, aaddress3, aaddress4;

            eaddress1 = StartClientArray[0].ToString().Trim();
            eaddress2 = StartClientArray[1].ToString().Trim();
            aaddress3 = StartClientArray[2].ToString().Trim();
            aaddress4 = StartClientArray[3].ToString().Trim();

            return eaddress1 + "|" + eaddress2 + "|" + aaddress3 + "|" + aaddress4;

        }

        // GET: Customers/GustomerInfo
        public void GetInfooo(string custCode,string ipaddress, string portno)
        {

            var CodeIsNume = int.TryParse(custCode, out int Code);
            int UserId = 1;
            if (UserId == 1)
            {
               var custinfo= GetInfoModule.GetCustInfo_MVC(custCode, ipaddress, portno, User.Identity.GetUserId());
                var custaddress = GetInfoModule.addressInfo(custCode, ipaddress, portno, User.Identity.GetUserId());
            }
            else
            {
                string ArName ;
                string address;
                string EnName;
                string connectionstring = WebConfigurationManager.AppSettings["DB2Con"];
                OleDbConnection conn = new OleDbConnection(connectionstring);
                DataSet myDataSet = new DataSet();
                conn.Open();
                OleDbCommand cmd = new OleDbCommand("select * from MIBDBF.NA01MST where BRC ="+custCode+" and CLNO ="+custCode.Substring(2), conn);
                cmd.CommandTimeout = 50;
                var reader = cmd.ExecuteReader();
                int ASXX = reader.RecordsAffected;
                OleDbDataAdapter DataAdapter = new OleDbDataAdapter(cmd);
                //                name.Text = rdr.Item("ENGNM1") ' English Full Name 50 Char
                //eaddress1.Text = rdr.Item("ENGAD1") ' English Full Address 50 Char
                //aname.Text = rdr.Item("ARBNM1") ' Arabic Full Name 50 Char
                //aaddress1.Text = rdr.Item("ARBAD1") ' Arabic Full Address 50 Char
                //tel1.Text = rdr.Item("CLMOB1") ' Mobile number 1 18 Char
                //tel2.Text = rdr.Item("CLMOB2") ' Mobile number 2 18 Char
                while (reader.Read())
                {
                    DataAdapter.Fill(myDataSet);
                }
                reader.Close();
                conn.Close();
            }



         
                
                ///////////////////////
                
        }
    }
}