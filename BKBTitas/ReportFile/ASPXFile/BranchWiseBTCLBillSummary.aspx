<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BranchWiseBTCLBillSummary.aspx.cs" Inherits="BKBTitas.ReportFile.ASPXFile.BranchWiseBTCLBillSummary" %>

<%@ Register assembly="Microsoft.ReportViewer.WebForms" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
        </div>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
         <rsweb:ReportViewer ID="rvBranchWiseBTCLBillSummary" runat="server" Style="" Width="100%" Height="100%" ZoomMode="PageWidth"
            SizeToReportContent="True" Font-Names="Verdana" Font-Size="8pt" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" BackColor="" ClientIDMode="AutoID" HighlightBackgroundColor="" InternalBorderColor="204, 204, 204" InternalBorderStyle="Solid" InternalBorderWidth="1px" LinkActiveColor="" LinkActiveHoverColor="" LinkDisabledColor="" PrimaryButtonBackgroundColor="" PrimaryButtonForegroundColor="" PrimaryButtonHoverBackgroundColor="" PrimaryButtonHoverForegroundColor="" SecondaryButtonBackgroundColor="" SecondaryButtonForegroundColor="" SecondaryButtonHoverBackgroundColor="" SecondaryButtonHoverForegroundColor="" SplitterBackColor="" ToolbarDividerColor="" ToolbarForegroundColor="" ToolbarForegroundDisabledColor="" ToolbarHoverBackgroundColor="" ToolbarHoverForegroundColor="" ToolBarItemBorderColor="" ToolBarItemBorderStyle="Solid" ToolBarItemBorderWidth="1px" ToolBarItemHoverBackColor="" ToolBarItemPressedBorderColor="51, 102, 153" ToolBarItemPressedBorderStyle="Solid" ToolBarItemPressedBorderWidth="1px" ToolBarItemPressedHoverBackColor="153, 187, 226">
            <LocalReport ReportPath="ReportFile\RDLCFIle\BranchWiseBTCLBillSummary.rdlc">
               
            </LocalReport>
        </rsweb:ReportViewer>
      
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:BKBTitasConnectionString %>" SelectCommand="BranchWiseBTCLBillDetails" SelectCommandType="StoredProcedure">
            <SelectParameters>
                <asp:Parameter Name="dtFrom" Type="String" />
                <asp:Parameter Name="dtTo" Type="String" />
                <asp:Parameter Name="branch" Type="String" />
                <asp:Parameter Name="userRole" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
      
    </form>
</body>
</html>
