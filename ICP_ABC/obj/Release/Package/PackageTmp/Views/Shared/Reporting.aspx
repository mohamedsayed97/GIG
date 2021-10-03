<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>
<%@ Register assembly="Microsoft.ReportViewer.WebForms" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <meta name="viewport" content="width=device-width" />
    <title>ReportViwer in MVC4 Application</title>    
    <script runat="server">
        void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //List<ICP_ABC.Areas.NavRpt.Models.Report> fuhnds = null;
                using (ICP_ABC.Models.ApplicationDbContext db = new ICP_ABC.Models.ApplicationDbContext() )
                {
                    var funds = db.Funds.OrderBy(a => a.FundID).ToList();
                    ReportViewer3.LocalReport.ReportPath = Server.MapPath("~/Area/NavRpt/RPTDatasets/Nav1.rdlc");
                    ReportViewer3.LocalReport.DataSources.Clear();
                    ReportDataSource rdc = new ReportDataSource("MyDataset", funds);
                    ReportViewer3.LocalReport.DataSources.Add(rdc);
                    ReportViewer3.LocalReport.Refresh();
                }
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <rsweb:ReportViewer ID="ReportViewer3" runat="server"></rsweb:ReportViewer>
       
    </div>
    </form>
</body>
</html>