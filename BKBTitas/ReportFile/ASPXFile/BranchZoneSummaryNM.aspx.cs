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
    public partial class BranchZoneSummaryNM : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                rvBrZoneWiseMSummaryNon.ProcessingMode = ProcessingMode.Local;
                rvBrZoneWiseMSummaryNon.LocalReport.ReportPath = Server.MapPath("~/ReportFile/RDLCFile/BrZoneWiseSummaryNon.rdlc");
                string currentDate = DateTime.Today.ToString("yyyy-MM-dd");
                string branchCode = Request.QueryString["branchCode"].ToString() == "" ? "0000" : Request.QueryString["branchCode"].ToString();
                string dtFrom = currentDate;
                string dtTo = currentDate;

                ReportParameter p1 = new ReportParameter("dtFrom", dtFrom);
                ReportParameter p2 = new ReportParameter("dtTo", dtTo);

                DataTable dtBrWiseMSummary;
                dtBrWiseMSummary = GetData("EXEC dbo.BranchZoneNonMeterSummary @dtFrom='" + dtFrom + "',@dtTo='" + dtTo + "',@branch='" + 4001 + "'");

                if (dtBrWiseMSummary.Rows.Count > 0)
                {
                    this.rvBrZoneWiseMSummaryNon.LocalReport.SetParameters(new ReportParameter[] { p1, p2 });
                    ReportDataSource dtReport = new ReportDataSource("BranchZoneNonMeterSummary", dtBrWiseMSummary);
                    rvBrZoneWiseMSummaryNon.LocalReport.DataSources.Clear();
                    rvBrZoneWiseMSummaryNon.LocalReport.DataSources.Add(dtReport);
                    rvBrZoneWiseMSummaryNon.LocalReport.Refresh();
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