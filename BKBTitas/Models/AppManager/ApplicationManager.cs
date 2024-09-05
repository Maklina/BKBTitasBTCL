using BKBTitas.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BKBTitas.Models.ViewModel;
using System.Net.Http;
using System.Data.Entity.Validation;
using log4net;
using System.Reflection;
using System.Data.Entity.Core.Objects;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using ClosedXML.Excel;

namespace BKBTitas.Models.AppManager
{
    public class ApplicationManager
    {
        public string AppLocation = ConfigurationManager.AppSettings["AppLocation"];
        BKBTitasEntities db = new BKBTitasEntities();
        ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public string getRoutingNo(string branchCode)
        {
            var row = db.Branches.Where(o => o.branch_code.Equals(branchCode)).SingleOrDefault();
            return row.RoutingNo;
        }
        public string getPeriod(DateTime startDate, DateTime endDate)
        {
            startDate = new DateTime(startDate.Year, startDate.Month, 1);
            endDate = new DateTime(endDate.Year, endDate.Month, 1);
            if (startDate == endDate)
            {
                return startDate.ToString("MMyy");
            }
            else
            {
                return startDate.ToString("MMyy") + "," + endDate.ToString("MMyy");
            }
        }
        internal string getBankTransactionID()
        {
            ObjectParameter returnId = new ObjectParameter("value", typeof(int)); 
            var txnID = db.GetBankTxnID(returnId).ToList(); 
            return  "BKB"+System.DateTime.Now.ToString("yyyyMMdd")+ returnId.Value.ToString().PadLeft(6,'0'); 
        }
        internal int saveTxnBillDetails(BillViewModel bVM, LoginModels login)
        {
            MeteredBillDetail mBillDet = new MeteredBillDetail();
            try
            {
                mBillDet.paymentId = bVM.paymentId;
                mBillDet.invoiceNo = bVM.invoiceNo;
                mBillDet.customerCode = bVM.customerCode;
                mBillDet.CustomerMobile = bVM.customerMobile;
                mBillDet.CustomerName = bVM.customerName;
                mBillDet.custAddress = bVM.customerAddress;
                mBillDet.issueDate = bVM.issueDate;
                mBillDet.issueMonth = bVM.issueMonth;
                mBillDet.invoiceAmount = bVM.invoiceAmount;
                mBillDet.paidAmount = bVM.paidAmount;     //hard code for revenue stamp
                mBillDet.settaleDate = bVM.settaleDate;
                mBillDet.revenueStamp = bVM.revenueStamp;       //hard code for revenue stamp
                mBillDet.sourceTaxAmount = bVM.sourceTaxAmount;
                mBillDet.transactionDateTime = DateTime.Now;
                mBillDet.zone = bVM.zone;
                mBillDet.bankCode = bVM.bankCode;
                mBillDet.transactionDate = bVM.transactionDate;
                mBillDet.creationTime = bVM.creationTime;
                mBillDet.refNo = bVM.bankTransactionId;
                
                mBillDet.creator = login.UserId;
                mBillDet.branchCode = login.BranchCode;
                mBillDet.branchRoutingNo = getRoutingNo(login.BranchCode);
                mBillDet.chalanBank = bVM.chalanBank;
                mBillDet.chalanBranch = bVM.chalanBranch;
                mBillDet.chalanNo = bVM.chalanNo;
                mBillDet.chalanDate = bVM.chalanDate;
                mBillDet.transactionStatus = bVM.transactionStatus; //Transaction inititated

                db.MeteredBillDetails.Add(mBillDet);
                db.SaveChanges();

                return 0;
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var entityValidationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityValidationErrors.ValidationErrors)
                    {
                        logger.Error("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                    }
                }
                return 1;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return 1;
            }
        }

        internal int updateMeteredTxnData(BillViewModel txnData, LoginModels login,string status)
        {
            try
            {
                txnData.invoiceNo = txnData.invoiceNo.Replace("-", "");
                txnData.customerCode = txnData.customerCode.Replace("-", "");

                var upId = db.MeteredBillDetails.Where(o => o.invoiceNo.Equals(txnData.invoiceNo)
                                                               && o.customerCode.Equals(txnData.customerCode)).Max(o=>o.id);

                MeteredBillDetail mBillDet = db.MeteredBillDetails.Find(upId);

                mBillDet.updateFlag = status;
                mBillDet.remarks = mBillDet.remarks + " " + txnData.message;
                mBillDet.transactionStatus = status;
                mBillDet.reason = txnData.reason;
                mBillDet.updateOperator = login.UserId;
                mBillDet.updateTime = System.DateTime.Now;
                db.SaveChanges();
                return 0;
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var entityValidationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityValidationErrors.ValidationErrors)
                    {
                        logger.Error("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                    }
                }
                return 1;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return 1;
            }
        }
        
        internal int saveNonMeterBillDet(BillViewModel bVM, LoginModels login)
        {
            NonMeterBillDetail nBillDet = new NonMeterBillDetail();
            try
            {
                nBillDet.paymentId = bVM.payId;
                nBillDet.invoiceNo = bVM.invoiceNo==null?"000000":bVM.invoiceNo;
                nBillDet.customerCode = bVM.customerCode;
                nBillDet.CustomerMobile = bVM.mobile;
                nBillDet.CustomerName = bVM.customerName;
                nBillDet.transactionDate = System.DateTime.Now.ToString("yyyy-MM-dd");
                nBillDet.custAddress = bVM.connectionAddress;
                nBillDet.invoiceAmount = bVM.amount;  //return from 
                nBillDet.paidAmount = bVM.invoiceNo == null ? bVM.total: bVM.amount;     
                nBillDet.revenueStamp = bVM.amount>400?10:0;       //hard code for revenue stamp
                nBillDet.Surcharge =Convert.ToDecimal(bVM.surcharge);
                nBillDet.transactionDateTime = DateTime.Now;
                nBillDet.particulars = bVM.particulars;
                nBillDet.batchNo = bVM.batchNo;
                nBillDet.bankTransactionId = bVM.bankTransactionId;
                nBillDet.appliancesummary = bVM.applicationSummary;
                nBillDet.zone = getZoneName(bVM.customerCode.Substring(1, 3));

                nBillDet.voucherDate = bVM.voucherDate;
                nBillDet.creationTime = bVM.creationTime;

                nBillDet.creator = login.UserId;
                nBillDet.branchCode = login.BranchCode;
                nBillDet.branchRoutingNo = getRoutingNo(login.BranchCode);
                nBillDet.transactionStatus = "0"; //Transaction inititated
                nBillDet.bankCode = "Krishi";

                nBillDet.created_by = login.UserId;
                nBillDet.created_on = System.DateTime.Now;

                db.NonMeterBillDetails.Add(nBillDet);
                db.SaveChanges();

                return 0;
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var entityValidationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityValidationErrors.ValidationErrors)
                    {
                        logger.Error("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                    }
                }
                return 1;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return 1;
            }
        }

        private string getZoneName(string code)
        {
            var row = db.TitasZones.Where(o => o.zoneCode.Equals(code)).SingleOrDefault();
            return row.zoneName;
        }

        internal int updateNonMeteredTxnData(BillViewModel txnData, LoginModels login, string status)
        {
            try
            {
                var upId = db.NonMeterBillDetails.Where(o => o.paymentId.Equals(txnData.payId) && o.batchNo.Equals(txnData.batchNo)).Max(o => o.id);

                NonMeterBillDetail nBillDet = db.NonMeterBillDetails.Find(upId);

                nBillDet.updateFlag = status;
                nBillDet.remarks = nBillDet.remarks + " " + txnData.message;
                nBillDet.reason = txnData.reason;
                nBillDet.transactionStatus = status;
                nBillDet.updateOperator = login.UserId;
                nBillDet.updateTime = System.DateTime.Now;
                db.SaveChanges();
                return 0;
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var entityValidationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityValidationErrors.ValidationErrors)
                    {
                        logger.Error("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                    }
                }
                return 1;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return 1;
            }
        }

        public int getData(string zone, string rptdate)
        {
            DataSet ds = null;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["BKBTitasConnectionString"].ConnectionString))
            {
                try
                {
                    string query = "select * from Non_MeteredReport where convert(date, transactionDateTime,121) = @transactionDateTime and zone = @zone";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@transactionDateTime", rptdate));
                    cmd.Parameters.Add(new SqlParameter("@zone", zone));
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = cmd;
                    ds = new DataSet();
                    da.Fill(ds);
                    ExportDataSetToExcel(ds, zone);
                    return ds.Tables[0].Rows.Count;
                }
                catch (Exception)
                {
                    return -1;
                }
                finally
                {
                    ds.Dispose();
                }
            }
        }
        private void ExportDataSetToExcel(DataSet ds, string zone)
        {
            //zone = "Zone-4(Motijheel)";
            string AppLocation = "";
            AppLocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            AppLocation = AppLocation.Replace("file:\\", "");
            string file = AppLocation + "\\ExcelFiles\\"+ zone + ".xlsx";
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(ds.Tables[0]);
                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;
                wb.SaveAs(file);
            }
        }


        public int getDataMeterd(string zone, string rptdate)
        {
            DataSet ds = null;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["BKBTitasConnectionString"].ConnectionString))
            {
                try
                {
                    string query = "select * from MeteredReport where convert(date, transactionDateTime,121) = @transactionDateTime and zone = @zone";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@transactionDateTime", rptdate));
                    cmd.Parameters.Add(new SqlParameter("@zone", zone));
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = cmd;
                    ds = new DataSet();
                    da.Fill(ds);
                    ExportMeteredDataSetToExcel(ds, zone);
                    return ds.Tables[0].Rows.Count;
                }
                catch (Exception)
                {
                    return -1;
                }
                finally
                {
                    ds.Dispose();
                }
            }
        }
        private void ExportMeteredDataSetToExcel(DataSet ds, string zone)
        {
            //zone = "Zone-4(Motijheel)";
            string AppLocation = "";
            AppLocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            AppLocation = AppLocation.Replace("file:\\", "");
            string file = AppLocation + "\\ExcelFiles\\" + zone + "_metered.xlsx";
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(ds.Tables[0]);
                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;
                wb.SaveAs(file);
            }
        }
    }
}