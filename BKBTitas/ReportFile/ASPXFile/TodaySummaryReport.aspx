<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TodaySummaryReport.aspx.cs" Inherits="BKBTitas.ReportFile.ASPXFile.TodaySummaryReport" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms,Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845DCD8080CC91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
     <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <rsweb:ReportViewer ID="rvBrWiseMSummary" runat="server" Style="" Width="100%" Height="100%" ZoomMode="PageWidth" ShowPrintButton="true"
            SizeToReportContent="True" Font-Names="Verdana" Font-Size="8pt" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt">
            <LocalReport ReportPath="ReportFile\RDLCFile\BrWiseMSummary.rdlc">
            </LocalReport>
        </rsweb:ReportViewer>
    </div>
    </form>
</body>
</html>
