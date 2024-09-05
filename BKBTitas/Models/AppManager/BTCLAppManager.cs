using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using BKBTitas.EntityModel;
using System.Text;
using BKBTitas.SvcRef_BTCLBoss;
using BKBTitas.Models.ViewModel;
using System.Data.Entity.Validation;
using log4net;
using System.Reflection;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using ClosedXML.Excel;

namespace BKBTitas.Models.AppManager
{
    public class BTCLAppManager
    {
        BKBTitasEntities db = new BKBTitasEntities();
        ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        internal Int32 getBtclBankTransactionID()
        {
            ObjectParameter returnId = new ObjectParameter("value", typeof(int));
            var txnID = db.GetBtclBankTxnID(returnId).ToList();
            return Convert.ToInt32(System.DateTime.Now.ToString("MMdd") + returnId.Value.ToString().PadLeft(6, '0'));
        }

        public StringBuilder getBillXML(SvcRef_BTCLBoss.tAuthHeader header, SvcRef_BTCLBoss.tBillRequest body)
        {
            StringBuilder xml = new StringBuilder();
            xml.Append(@"<?xml version=""1.0"" encoding=""utf-8""?>");
            xml.Append(@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" ");
            xml.Append(@"xmlns:xsd=""http://com.ztesoft.zsmart/xsd""> ");
            xml.Append("<soapenv:Header>");
            xml.Append("<xsd:AuthHeader>");
            xml.Append("<Username>" + header.Username + "</Username>");
            xml.Append("<Password>" + header.Password + "</Password>");
            xml.Append("<ChannelCode>" + header.ChannelCode + "</ChannelCode>");
            xml.Append("</xsd:AuthHeader>");
            xml.Append("</soapenv:Header>");
            xml.Append("<soapenv:Body>");
            xml.Append("<xsd:BillRequest>");
            xml.Append("<TXNNumber>" + body.TXNNumber + "</TXNNumber>");
            xml.Append("<UserID>" + body.UserID + "</UserID>");
            xml.Append("<TokenNumber>" + body.TokenNumber + "</TokenNumber>");
            xml.Append("<PhoneNumber>" + body.PhoneNumber + "</PhoneNumber>");
            xml.Append("<ExchangeCode></ExchangeCode>");
            xml.Append("<AccountNo></AccountNo>");
            xml.Append("<BillNo>" + body.BillNo + "</BillNo>");
            xml.Append("<PayStatus>" + body.PayStatus + "</PayStatus>");
            xml.Append("</xsd:BillRequest>");
            xml.Append("</soapenv:Body>");
            xml.Append("</soapenv:Envelope>");
            return xml;
        }

        internal string getBtclBranchCode(string branchCode)
        {
            var branch = db.Branches.Where(o => o.branch_code.Equals(branchCode)).SingleOrDefault();
            return branch.BtclCode;
        }
    

        internal int SaveBillDetails(BTCLBillDetailsModel tBillPaymentRequest, LoginModels login)
        {
            try
            {
                BtclBillDetail btclDetails = new BtclBillDetail();

                btclDetails.BranchCode = login.BranchCode;
                btclDetails.BtclBranchCode = getBtclBranchCode(login.BranchCode);
                btclDetails.BillNumber = tBillPaymentRequest.BillNo;
                btclDetails.ExchangeCode = tBillPaymentRequest.ExchangeCode;
                btclDetails.PhoneNumber = tBillPaymentRequest.PhoneNumber;
                btclDetails.AccountNumber = tBillPaymentRequest.AccountNumber;
                btclDetails.Name = tBillPaymentRequest.Name;
                //btclDetails.LastPayDate = tBillPaymentRequest.LastPayDate;
                btclDetails.TxnNumber = getBtclBankTransactionID().ToString();
                btclDetails.BillMonth = tBillPaymentRequest.BillMonth;
                btclDetails.BillYear = tBillPaymentRequest.BillYear;
                btclDetails.BTCLAmount = tBillPaymentRequest.BTCLAmount;
                btclDetails.OverdueAmount = tBillPaymentRequest.OverdueAmount;
                btclDetails.VAT = tBillPaymentRequest.VAT;
                btclDetails.TotalAmount = tBillPaymentRequest.TotalAmount;
                btclDetails.BillPayStatus = tBillPaymentRequest.BillPayStatus;
                btclDetails.ServiceNumber = tBillPaymentRequest.ServiceNumber;
                btclDetails.NewPayStatus = "P";
                btclDetails.Status = "Success";
                btclDetails.Createon = DateTime.Now;
                btclDetails.Createdby = login.UserId;
                //btclDetails.Updatedon = tBillPaymentRequest.Updatedon;
                //btclDetails.Ureatedby = tBillPaymentRequest.Ureatedby;


                db.BtclBillDetails.Add(btclDetails);
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
        internal int SaveBillDetailsLog(BTCLBillDetailsModel tBillPaymentRequest, LoginModels login)
        {
            try
            {
                BtclBillDetailLog btclDetails = new BtclBillDetailLog();
                btclDetails.BranchCode = login.BranchCode;
                btclDetails.BtclBranchCode = getBtclBranchCode(login.BranchCode);
                btclDetails.BillNumber = tBillPaymentRequest.BillNo;
                btclDetails.ExchangeCode = tBillPaymentRequest.ExchangeCode;
                btclDetails.PhoneNumber = tBillPaymentRequest.PhoneNumber;
                btclDetails.AccountNumber = tBillPaymentRequest.AccountNumber;
                btclDetails.Name = tBillPaymentRequest.Name;
                //btclDetails.LastPayDate = tBillPaymentRequest.LastPayDate;
                btclDetails.TxnNumber = getBtclBankTransactionID().ToString();
                btclDetails.BillMonth = tBillPaymentRequest.BillMonth;
                btclDetails.BillYear = tBillPaymentRequest.BillYear;
                btclDetails.BTCLAmount = tBillPaymentRequest.BTCLAmount;
                btclDetails.OverdueAmount = tBillPaymentRequest.OverdueAmount;
                btclDetails.VAT = tBillPaymentRequest.VAT;
                btclDetails.TotalAmount = tBillPaymentRequest.TotalAmount;
                btclDetails.BillPayStatus = tBillPaymentRequest.BillPayStatus;
                btclDetails.ServiceNumber = tBillPaymentRequest.ServiceNumber;
                //btclDetails.NewPayStatus = "U";
               // btclDetails.Status = "Success";
                btclDetails.Createon = DateTime.Now;
                btclDetails.Createdby = login.UserId;
                //btclDetails.Updatedon = tBillPaymentRequest.Updatedon;
                //btclDetails.Ureatedby = tBillPaymentRequest.Ureatedby;


                db.BtclBillDetailLogs.Add(btclDetails);
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

        internal int UpdateBillDetails(string txnNumber, LoginModels login)
        {
            try
            {
                var upId = db.BtclBillDetails.Where(o => o.TxnNumber.Equals(txnNumber)).Max(o => o.Id);

                BtclBillDetail btclDetails = db.BtclBillDetails.Find(upId);

                btclDetails.NewPayStatus = "P";
                btclDetails.Status = "Success";
                btclDetails.Updatedon = DateTime.Now;
                btclDetails.Ureatedby = login.UserId;
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

        internal int CancelBillDetails(int id, string reason, LoginModels login)
        {
            try
            {

                BtclBillDetail btclDetails = db.BtclBillDetails.Find(id);

                btclDetails.CancelReason = reason;
                btclDetails.Status = "Canceled";
                btclDetails.NewPayStatus = "U";
                btclDetails.Canceledon = DateTime.Now;
                btclDetails.CanceledBy = login.UserId;
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

        internal List<BTCLBillDetailsModel> GetCollectionDetails()
        {
            try
            {
                List<BTCLBillDetailsModel> billDet = (from q in db.BtclBillDetails
                                                      where q.Status == "Success" || q.Status == "Canceled"
                                                      select new BTCLBillDetailsModel
                                                      {
                                                          id = q.Id,
                                                          ServiceNumber = q.ServiceNumber,
                                                          BillNo = q.BillNumber,
                                                          BillMonth = q.BillMonth,
                                                          BillYear = q.BillYear,
                                                          BTCLAmount = q.BTCLAmount,
                                                          VAT = q.VAT,
                                                          OverdueAmount = q.OverdueAmount,
                                                          //TotalAmount = (q.BTCLAmount+ q.VAT+ q.OverdueAmount),
                                                          Name = q.Name,
                                                          CollectionDate = q.Createon,
                                                          BranchCode = q.BranchCode,
                                                          BillPayStatus = q.Status
                                                      }).ToList();
                return billDet;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }

        internal BTCLBillDetailsModel GetSIngleBillDetails(int id)
        {
            try
            {
                BTCLBillDetailsModel billDet = (from q in db.BtclBillDetails
                                                where q.Status == "Success" && q.Id == id
                                                select new BTCLBillDetailsModel
                                                {
                                                    id = q.Id,
                                                    ServiceNumber = q.ServiceNumber,
                                                    BillNo = q.BillNumber,
                                                    BillMonth = q.BillMonth,
                                                    BillYear = q.BillYear,
                                                    BTCLAmount = q.BTCLAmount,
                                                    VAT = q.VAT,
                                                    OverdueAmount = q.OverdueAmount,
                                                    //TotalAmount = (q.BTCLAmount+ q.VAT+ q.OverdueAmount),
                                                    Name = q.Name,
                                                    CollectionDate = q.Createon,
                                                    BranchCode = q.BranchCode,
                                                    BillPayStatus = q.Status
                                                }).SingleOrDefault();
                return billDet;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }

        public int getBTCLData( string rptdate)
        {
            DataSet ds = null;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["BKBTitasConnectionString"].ConnectionString))
            {
                try
                {
                    string query = @"select ROW_NUMBER() OVER(ORDER BY (SELECT 1)) AS  SL_No,[Bill No/ Invoice ID] [Bill No],[PhoneNumber] ,[BillMonth],[BillYear],[Bill Amount],[VAT],[TotalAmount],[TxnNumber] from BTCLDailyReport where convert(date, Createon,121) = @transactionDateTime ";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@transactionDateTime", rptdate));
                    //cmd.Parameters.Add(new SqlParameter("@zone", zone));
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = cmd;
                    ds = new DataSet();
                 
                    da.Fill(ds);
                    ExportBTCLDataSetToExcel(ds, rptdate);
                    return ds.Tables[0].Rows.Count;
                }
                catch (Exception ex)
                {
                    return -1;
                }
                finally
                {
                    ds.Dispose();
                }
            }
        }
        private void ExportBTCLDataSetToExcel(DataSet ds, string rptDate)
        {
            //zone = "Zone-4(Motijheel)";
            string AppLocation = "";
            AppLocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            AppLocation = AppLocation.Replace("file:\\", "");
            string file = AppLocation + "\\ExcelFiles\\" +"BTCL_DailyReport"+ ".xlsx";
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(ds.Tables[0]);
                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;
                wb.SaveAs(file);
            }
        }



        public int getBTCLMonthlyData(string rptdateFrom, string rptdateTo)
        {
            DataSet ds = null;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["BKBTitasConnectionString"].ConnectionString))
            {
                try
                {
                    //string From=
                    string query = @"select ROW_NUMBER() OVER(ORDER BY (SELECT 1)) AS  SL_No,[Bill No/ Invoice ID] [Bill No],[PhoneNumber] ,[BillMonth],[BillYear],[Bill Amount],[VAT],[TotalAmount],[TxnNumber] from BTCLDailyReport where convert(date, Createon,121)  between   @transactionDateTimeFrom and  @transactionDateTimeTo ";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.CommandType = CommandType.Text;
                    SqlParameter param = new SqlParameter();
                    cmd.Parameters.Add(new SqlParameter("@transactionDateTimeFrom", rptdateFrom));
                    cmd.Parameters.Add(new SqlParameter("@transactionDateTimeTo", rptdateTo));
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = cmd;
                    ds = new DataSet();

                    da.Fill(ds);
                    ExportBTCLMonthlyDataSetToExcel(ds, rptdateFrom,rptdateTo);
                    return ds.Tables[0].Rows.Count;
                }
                catch (Exception ex)
                {
                    return -1;
                }
                finally
                {
                    ds.Dispose();
                }
            }
        }
        private void ExportBTCLMonthlyDataSetToExcel(DataSet ds, string rptDateFrom, string rptDateTo)
        {
            //zone = "Zone-4(Motijheel)";
            string AppLocation = "";
            AppLocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            AppLocation = AppLocation.Replace("file:\\", "");
            string file = AppLocation + "\\ExcelFiles\\" + "BTCL_MonthlyReport" + ".xlsx";
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