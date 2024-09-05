using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BKBTitas.ReportFile.ASPXFile
{
    public partial class CancelnDDReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                rvCancelnDD.ProcessingMode = ProcessingMode.Local;

                string currentDate = DateTime.Today.ToString("yyyy-MM-dd");
                string branchCode = Request.QueryString["branchCode"].ToString() == "" ? "0000" : Request.QueryString["branchCode"].ToString();

                string dtFrom = Request.QueryString["dtFrom"].ToString() == "" ? currentDate : Request.QueryString["dtFrom"].ToString();

                string dtTo = Request.QueryString["dtTo"].ToString() == "" ? dtFrom : Request.QueryString["dtTo"].ToString();

                string type = Request.QueryString["type"].ToString();

                string userRole = Request.QueryString["userRole"].ToString();

                ReportParameter p1 = new ReportParameter("dtFrom", dtFrom);
                ReportParameter p2 = new ReportParameter("dtTo", dtTo);

                DataTable dtBrWiseNonMDetail;

                if (type == "CustDetBnDNNon")
                {
                    rvCancelnDD.LocalReport.ReportPath = Server.MapPath("~/ReportFile/RDLCFile/BrWiseNonMCustomerDetail.rdlc");
                    dtBrWiseNonMDetail = GetData("EXEC dbo.[BranchWiseNonMeterCustDetails] @dtFrom='" + dtFrom + "',@dtTo='" + dtTo + "',@branch='" + branchCode + "',@userRole='" + userRole + "'");

                    if (dtBrWiseNonMDetail.Rows.Count > 0)
                    {
                        this.rvCancelnDD.LocalReport.SetParameters(new ReportParameter[] { p1, p2 });
                        ReportDataSource dtReport = new ReportDataSource("CustDetBnDNNon", dtBrWiseNonMDetail);
                        rvCancelnDD.LocalReport.DataSources.Clear();
                        rvCancelnDD.LocalReport.DataSources.Add(dtReport);
                        rvCancelnDD.LocalReport.Refresh();
                    }
                }
                else if (type == "DNDetNon")
                {
                    rvCancelnDD.LocalReport.ReportPath = Server.MapPath("~/ReportFile/RDLCFile/BrWiseDNDetailNon.rdlc");
                    dtBrWiseNonMDetail = GetData("EXEC dbo.[BranchWiseNMDNoteDetails] @dtFrom='" + dtFrom + "',@dtTo='" + dtTo + "',@branch='" + branchCode + "',@userRole='" + userRole + "'");

                    if (dtBrWiseNonMDetail.Rows.Count > 0)
                    {
                        this.rvCancelnDD.LocalReport.SetParameters(new ReportParameter[] { p1, p2 });
                        ReportDataSource dtReport = new ReportDataSource("DNDetNon", dtBrWiseNonMDetail);
                        rvCancelnDD.LocalReport.DataSources.Clear();
                        rvCancelnDD.LocalReport.DataSources.Add(dtReport);
                        rvCancelnDD.LocalReport.Refresh();
                    }
                }
                else if (type == "DNSumZoneNon")
                {
                    rvCancelnDD.LocalReport.ReportPath = Server.MapPath("~/ReportFile/RDLCFile/ZoneWiseSummaryDNNon.rdlc");
                    dtBrWiseNonMDetail = GetData("EXEC dbo.[ZoneWiseSummaryDNNon] @dtFrom='" + dtFrom + "',@dtTo='" + dtTo + "',@branch='" + branchCode + "',@userRole='" + userRole + "'");

                    if (dtBrWiseNonMDetail.Rows.Count > 0)
                    {
                        this.rvCancelnDD.LocalReport.SetParameters(new ReportParameter[] { p1, p2 });
                        ReportDataSource dtReport = new ReportDataSource("DNSumNon", dtBrWiseNonMDetail);
                        rvCancelnDD.LocalReport.DataSources.Clear();
                        rvCancelnDD.LocalReport.DataSources.Add(dtReport);
                        rvCancelnDD.LocalReport.Refresh();
                    }
                }
                else if(type == "DNSumNon")
                {
                    rvCancelnDD.LocalReport.ReportPath = Server.MapPath("~/ReportFile/RDLCFile/BrWiseDNSumNon.rdlc");
                    dtBrWiseNonMDetail = GetData("EXEC dbo.[BranchWiseNonMeterDNoteSummary] @dtFrom='" + dtFrom + "',@dtTo='" + dtTo + "',@branch='" + branchCode + "',@userRole='" + userRole + "'");
                    if (dtBrWiseNonMDetail.Rows.Count > 0)
                    {
                        this.rvCancelnDD.LocalReport.SetParameters(new ReportParameter[] { p1, p2 });
                        ReportDataSource dtReport = new ReportDataSource("DNSumNon", dtBrWiseNonMDetail);
                        rvCancelnDD.LocalReport.DataSources.Clear();
                        rvCancelnDD.LocalReport.DataSources.Add(dtReport);
                        rvCancelnDD.LocalReport.Refresh();
                    }
                }
                if (type == "BillCancelNon")
                {
                    rvCancelnDD.LocalReport.ReportPath = Server.MapPath("~/ReportFile/RDLCFile/BrWiseBillCancelNon.rdlc");
                    dtBrWiseNonMDetail = GetData("EXEC dbo.[BranchWiseNonMeterCancelDetails] @dtFrom='" + dtFrom + "',@dtTo='" + dtTo + "',@branch='" + branchCode + "',@userRole='" + userRole + "'");
                    if (dtBrWiseNonMDetail.Rows.Count > 0)
                    {
                        this.rvCancelnDD.LocalReport.SetParameters(new ReportParameter[] { p1, p2 });
                        ReportDataSource dtReport = new ReportDataSource("BillCancelNon", dtBrWiseNonMDetail);
                        rvCancelnDD.LocalReport.DataSources.Clear();
                        rvCancelnDD.LocalReport.DataSources.Add(dtReport);
                        rvCancelnDD.LocalReport.Refresh();
                    }
                }
                else if(type == "DNCancelNon")
                {
                    rvCancelnDD.LocalReport.ReportPath = Server.MapPath("~/ReportFile/RDLCFile/BrWiseDNCancelNon.rdlc");
                    dtBrWiseNonMDetail = GetData("EXEC dbo.[BranchWiseNMDNoteCancelDetails] @dtFrom='" + dtFrom + "',@dtTo='" + dtTo + "',@branch='" + branchCode + "',@userRole='" + userRole + "'");
                    if (dtBrWiseNonMDetail.Rows.Count > 0)
                    {
                        this.rvCancelnDD.LocalReport.SetParameters(new ReportParameter[] { p1, p2 });
                        ReportDataSource dtReport = new ReportDataSource("CommonDataSet", dtBrWiseNonMDetail);
                        rvCancelnDD.LocalReport.DataSources.Clear();
                        rvCancelnDD.LocalReport.DataSources.Add(dtReport);
                        rvCancelnDD.LocalReport.Refresh();
                    }
                }
                else if(type == "BillCancelMet")
                {
                    rvCancelnDD.LocalReport.ReportPath = Server.MapPath("~/ReportFile/RDLCFile/BrWiseBillCancelMet.rdlc");
                    dtBrWiseNonMDetail = GetData("EXEC dbo.[BranchWiseMeterCancelDetails] @dtFrom='" + dtFrom + "',@dtTo='" + dtTo + "',@branch='" + branchCode + "',@userRole='" + userRole + "'");
                    if (dtBrWiseNonMDetail.Rows.Count > 0)
                    {
                        this.rvCancelnDD.LocalReport.SetParameters(new ReportParameter[] { p1, p2 });
                        ReportDataSource dtReport = new ReportDataSource("BillCancelMet", dtBrWiseNonMDetail);
                        rvCancelnDD.LocalReport.DataSources.Clear();
                        rvCancelnDD.LocalReport.DataSources.Add(dtReport);
                        rvCancelnDD.LocalReport.Refresh();
                    }
                }
                else if (type == "znWiseDetailsDNote")
                {
                    rvCancelnDD.LocalReport.ReportPath = Server.MapPath("~/ReportFile/RDLCFile/ZoneWiseNonDNoteDetail.rdlc");
                    dtBrWiseNonMDetail = GetData("EXEC dbo.[BranchWiseNMDNoteDetails] @dtFrom='" + dtFrom + "',@dtTo='" + dtTo + "',@branch='" + branchCode + "',@userRole='" + userRole + "'");
                    if (dtBrWiseNonMDetail.Rows.Count > 0)
                    {
                        this.rvCancelnDD.LocalReport.SetParameters(new ReportParameter[] { p1, p2 });
                        ReportDataSource dtReport = new ReportDataSource("ZoneWiseNMDNoteDetails", dtBrWiseNonMDetail);
                        rvCancelnDD.LocalReport.DataSources.Clear();
                        rvCancelnDD.LocalReport.DataSources.Add(dtReport);
                        rvCancelnDD.LocalReport.Refresh();
                    }
                }
            }
        }

        private DataTable GetData(string query)
        {
            string conString = ConfigurationManager.ConnectionStrings["BKBTitasConnectionString"].ConnectionString;
            //string conString = ReadConfig.GetAppSettingUsingConfigurationManager("ElectronicsDB");
            SqlCommand cmd = new SqlCommand(query);

            using (SqlConnection con = new SqlConnection(conString))
            {
                DataTable dt = new DataTable();

                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    cmd.Connection = con;

                    sda.SelectCommand = cmd;

                    sda.Fill(dt);
                    return dt;
                }
            }
        }
    }
}