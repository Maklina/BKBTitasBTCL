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
    public partial class BranchWiseBTCLBillDetail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                rvBranchWiseBTCLBillDetail.ProcessingMode = ProcessingMode.Local;

                string currentDate = DateTime.Today.ToString("yyyy-MM-dd");
                string branchCode = Request.QueryString["branchCode"].ToString() == "" ? "0000" : Request.QueryString["branchCode"].ToString();

                string dtFrom = Request.QueryString["dtFrom"].ToString() == "" ? currentDate : Request.QueryString["dtFrom"].ToString();

                string dtTo = Request.QueryString["dtTo"].ToString() == "" ? dtFrom : Request.QueryString["dtTo"].ToString();

                string type = Request.QueryString["type"].ToString();

                string userRole = Request.QueryString["userRole"].ToString();

                if (type == "brWiseDetails")
                {
                    rvBranchWiseBTCLBillDetail.LocalReport.ReportPath = Server.MapPath("~/ReportFile/RDLCFile/BranchWiseBTCLBillDetail.rdlc");
                }
                else
                {
                    rvBranchWiseBTCLBillDetail.LocalReport.ReportPath = Server.MapPath("~/ReportFile/RDLCFile/ZoneWiseNonMDetail.rdlc");
                }

                ReportParameter p1 = new ReportParameter("dtFrom", dtFrom);
                ReportParameter p2 = new ReportParameter("dtTo", dtTo);


                DataTable dtBranchWiseBTCLBillDetails;
                dtBranchWiseBTCLBillDetails = GetData("EXEC dbo.[BranchWiseBTCLBillDetails] @dtFrom='" + dtFrom + "',@dtTo='" + dtTo + "',@branch='" + branchCode + "',@userRole='" + userRole + "'");

                if (dtBranchWiseBTCLBillDetails.Rows.Count > 0)
                {

                    this.rvBranchWiseBTCLBillDetail.LocalReport.SetParameters(new ReportParameter[] { p1, p2 });
                    ReportDataSource dtReport = new ReportDataSource("BranchWiseBTCLBillDetails", dtBranchWiseBTCLBillDetails);
                    rvBranchWiseBTCLBillDetail.LocalReport.DataSources.Clear();
                    rvBranchWiseBTCLBillDetail.LocalReport.DataSources.Add(dtReport);
                    rvBranchWiseBTCLBillDetail.LocalReport.Refresh();
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