using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BKBTitas.Models.ViewModel;
using BKBTitas.Models.AppManager;
using System.Configuration;
using log4net;
using System.Reflection;
using System.Net;
using System.IO;
using System.Xml;
using System.Text;
using System.Data;
using BKBTitas.SvcRef_BTCLBoss;

namespace BKBTitas.Controllers
{
    public class BTCLController : Controller
    {
        public string licensePath = ConfigurationManager.AppSettings["licensePath"];
        public string licensePass = ConfigurationManager.AppSettings["licensePass"];

        public string btclUserID = ConfigurationManager.AppSettings["btclUserID"];
        public string btclPassword = ConfigurationManager.AppSettings["btclPassword"];
        public string btclChannel = ConfigurationManager.AppSettings["btclChannel"];

        public string btclUserName = ConfigurationManager.AppSettings["btclUserName"];
        public string btclPass = ConfigurationManager.AppSettings["btclPass"];
        public string btclWsdl = ConfigurationManager.AppSettings["btclWsdl"];
        public string btclBank = ConfigurationManager.AppSettings["btclBank"];

        BTCLTokenManager tm = new BTCLTokenManager();
        BTCLAppManager btm = new BTCLAppManager();
        ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private object header;
        private object body;

        // GET: BTCL
        public ActionResult Index()
        {
            LoginModels logIn = new LoginModels();
            var login = (BKBTitas.Models.ViewModel.LoginModels)Session["LoginCredentials"];

            if (login != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        [HttpPost]
        public ActionResult Index(BTCLBillRequestModel billReq)
        {
            var login = (BKBTitas.Models.ViewModel.LoginModels)Session["LoginCredentials"];
            

            if (login != null)
            {
                if (btm.getBtclBranchCode(login.BranchCode) == "" || btm.getBtclBranchCode(login.BranchCode) == null)
                {
                    TempData["retMsg"] += "You Branch is not enlisted to collect BTCL Bill";

                    return RedirectToAction("Index", "BTCL");
                }
                try
                {
                    SvcRef_BTCLBoss.tBillDetailDtoList BillDetailDtoList = new SvcRef_BTCLBoss.tBillDetailDtoList();


                    string phoneNo, billNo, accountNo, resCode, resMessage, error;
                    string token = tm.GetToken(GetHeader(), GetTokenRequestBody(), out error);

                    logger.Debug(Serializer.GetXMLFromObject(GetTokenRequestBody()));

                    if (token != null)
                    {

                        BTCLBillRequestModel appResponse = new BTCLBillRequestModel();
                        appResponse = GetBillRaw(GetHeader(), GetBillRequestBody(billReq));
                        if(appResponse.Details != null)
                        {
                            appResponse.TxnNumber = btm.getBtclBankTransactionID();
                            //appResponse.TxnNumber = billReq.TxnNumber;
                            return View("SearchResponse", appResponse);
                        }
                        else
                        {
                            TempData["errMsg"] += "No record found";

                            return RedirectToAction("Index", "BTCL"); 
                        }
                      
                    }
                    else
                    {
                        TempData["errMsg"] = "Connection error between BKB and BTCL";
                        logger.Error("Connection error between BKB and BTCL");
                        return RedirectToAction("Index", "BTCL");
                    }
                    return View();
                }
                catch (WebException ex)
                {
                    string message = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                    TempData["errMsg"] = "Error" + message;
                    logger.Error("Error:" + message);
                    return RedirectToAction("Index", "BTCL");
                }
                catch (Exception ex)
                {
                    TempData["errMsg"] = "Connection error between BKB and BTCL";
                    logger.Error("Error:" + ex);
                    return RedirectToAction("Index", "BTCL");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }


        public ActionResult SearchResponse()
        {
            var login = (BKBTitas.Models.ViewModel.LoginModels)Session["LoginCredentials"];
            if (login != null)
            {
                BTCLBillDetailsModel Res = (BTCLBillDetailsModel)TempData["BillResponse"];
                return View(Res);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        public BTCLBillRequestModel GetBillRaw(SvcRef_BTCLBoss.tAuthHeader header, SvcRef_BTCLBoss.tBillRequest body)
        {
            BTCLBillRequestModel appResponse = new BTCLBillRequestModel();
            List<BTCLBillDetailsModel> detailDtoList = new List<BTCLBillDetailsModel>();
            string xmlString = "";
            xmlString = @"
                        <soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:xsd='http://com.ztesoft.zsmart/xsd'>
                        <soapenv:Header>
                        <xsd:AuthHeader>
                        <Username>" + header.Username + @"</Username>
                        <Password>" + header.Password + @"</Password>
                        <ChannelCode>" + header.ChannelCode + @"</ChannelCode>
                        </xsd:AuthHeader>
                        </soapenv:Header>
                        <soapenv:Body>
                        <xsd:BillRequest>
                        <TXNNumber>" + body.TXNNumber + @"</TXNNumber>
                        <UserID>" + body.UserID + @"</UserID>
                        <TokenNumber>" + body.TokenNumber + @"</TokenNumber>
                        <PhoneNumber>" + body.PhoneNumber + @"</PhoneNumber>
                        <ExchangeCode></ExchangeCode>
                        <AccountNo></AccountNo>
                        <BillNo>" + body.BillNo + @"</BillNo>
                        <PayStatus>" + body.PayStatus + @"</PayStatus>
                        </xsd:BillRequest>
                        </soapenv:Body>
                        </soapenv:Envelope>
                        ";
            var webRequest = (HttpWebRequest)WebRequest.Create(btclWsdl);
            webRequest.ContentType = "text/xml";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            using (var streamWriter = new StreamWriter(webRequest.GetRequestStream()))
            {
                streamWriter.Write(xmlString);
                streamWriter.Flush();
                streamWriter.Close();
            }
            string soapResult = "";
            var httpResponse = (HttpWebResponse)webRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                soapResult = streamReader.ReadToEnd();
                logger.Debug(soapResult);
            }
            DataSet ds = new DataSet();
            if (!string.IsNullOrEmpty(soapResult))
            {
                StringReader stringReader = new StringReader(soapResult);

                ds.ReadXml(stringReader);
            }
            DataTable dt = ds.Tables["BillResponse"];
            if (dt != null)
            {
                var responseCode = dt.Rows[0]["ResponseCode"].ToString();
                // appResponse.TxnNumber = T.ToString();
                appResponse.ResponseCode = responseCode;
                appResponse.ResponseMessage = dt.Rows[0]["Message"].ToString();
                if (responseCode == "0")
                {

                    appResponse.PhoneNumber = dt.Rows[0]["PhoneNumber"] == null ? "" :
                    dt.Rows[0]["PhoneNumber"].ToString();
                    appResponse.BillNumber = dt.Rows[0]["BillNo"] == null ? "" :
                    dt.Rows[0]["BillNo"].ToString();
                    appResponse.AccountNumber = dt.Rows[0]["AccountNo"] == null ? "" :
                    dt.Rows[0]["AccountNo"].ToString();
                    DataTable dtBillList = ds.Tables["BillDetailDto"];
                    if (dtBillList != null)
                    {
                        foreach (DataRow row in dtBillList.Rows)
                        {
                            BTCLBillDetailsModel bill = new BTCLBillDetailsModel();
                            //bill.RowNumber = Convert.ToInt32(row["RowNumber"]);
                            bill.RowNumber = row["RowNumber"].ToString();
                            bill.TxnNumber = appResponse.TxnNumber.ToString();
                            bill.BillNo = row["BillNo"].ToString();
                            bill.Name = row["Name"] == null ? "" :
                            row["Name"].ToString();
                            bill.BillMonth = row["BillMonth"].ToString();
                            bill.BillYear = row["BillYear"].ToString();
                            //bill.BillMonth = Convert.ToInt32(row["BillMonth"]);
                            //bill.BillYear = Convert.ToInt32(row["BillYear"]);
                            bill.BTCLAmount = Convert.ToDecimal(row["BTCLAmount"])/100;
                            bill.OverdueAmount = Convert.ToDecimal(row["OverdueAmount"])/100;
                            bill.VAT = Convert.ToDecimal(row["VAT"])/100;
                            bill.BillPayStatus = row["BillPayStatus"].ToString();
                            detailDtoList.Add(bill);
                        }
                    }
                    appResponse.Details = detailDtoList;
                }
               
            }
            return appResponse;

        }



        [HttpPost]
        public ActionResult BTCLBillPaymentRequest(BTCLBillRequestModel payReq)
        {
            LoginModels login = (BKBTitas.Models.ViewModel.LoginModels)Session["LoginCredentials"];
            try
            {
                SvcRef_BTCLBoss.tBillDetailDtoList BillDetailDtoList = new SvcRef_BTCLBoss.tBillDetailDtoList();

                string txnNumber, resMessage, error;
                string token = tm.GetToken(GetHeader(), GetTokenRequestBody(), out error);
                if (token != null)
                {
                    foreach (var txn in payReq.Details)
                    {
                       
                            if (txn.isChecked == true)
                            {
                                int logsuccess = btm.SaveBillDetailsLog(txn, login);
                                if (logsuccess != 1)
                                {
                                    SvcRef_BTCLBoss.CrmWebServicesPortTypeClient client = new SvcRef_BTCLBoss.CrmWebServicesPortTypeClient();

                                    payReq.TxnNumber = btm.getBtclBankTransactionID();


                                    string response = client.BillPayment(GetHeader(), GetBillPaymentRequest(txn), out txnNumber, out resMessage);

                                    logger.Debug(Serializer.GetXMLFromObject(GetBillPaymentRequest(txn)));

                                    if (response.Equals("0"))
                                    {
                                        payReq.ResponseMessage = resMessage;
                                        btm.SaveBillDetails(txn, login);
                                        //return View("BillPayResponse", payReq);
                                    }

                                    else
                                    {
                                        TempData["retMsg"] = "Error:" + response + "-" + resMessage;
                                        logger.Error("Error:" + response + "-" + resMessage);
                                        return RedirectToAction("Index", "BTCL");
                                    }
                                }
                                else
                                {
                                    TempData["retMsg"] = "Error:" + "DB can't save log details";
                                    logger.Error("Error:" + "DB can't save log details");
                                    return RedirectToAction("Index", "BTCL");
                                }

                            }
                          
                    }
                }
                else
                {
                    TempData["retMsg"] = "Connection error between BKB and BTCL";

                    logger.Error("Connection error between BKB and BTCL");

                    return RedirectToAction("Index", "BTCL");
                }
                return View("BillPayResponse", payReq);
            }
            catch (Exception ex)
            {
                TempData["retMsg"] = "Error" + ex.Message;
                logger.Error("Error" + ex.Message);
                return PartialView("Index", "BTCL");
            }
        }
        public ActionResult SuccessDetails()
        {
            var login = (BKBTitas.Models.ViewModel.LoginModels)Session["LoginCredentials"];
            if (login != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        public ActionResult BillCollection(string date_)
        {
            var login = (BKBTitas.Models.ViewModel.LoginModels)Session["LoginCredentials"];

            if (login != null)
            {
                try
                {
                    List<BTCLBillDetailsModel> detLst = btm.GetCollectionDetails();

                    if (date_ != null)
                    {
                        detLst = detLst.Where(o => Convert.ToDateTime(o.CollectionDate).ToString("yyyy-MM-dd").Equals(Convert.ToDateTime(date_).ToString("yyyy-MM-dd"))).ToList();
                    }
                    else
                    {
                        detLst = detLst.Where(o => Convert.ToDateTime(o.CollectionDate).ToString("yyyy-MM-dd").Equals(DateTime.Now.ToString("yyyy-MM-dd"))).ToList();
                    }

                    if (login.UserRoleId == 2)
                    {
                        detLst = detLst.Where(o => o.BranchCode.Equals(login.BranchCode)).ToList();
                    }

                    return View(detLst);
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        public ActionResult BillCancel(int id)
        {
             var login = (BKBTitas.Models.ViewModel.LoginModels)Session["LoginCredentials"];
            if (login != null)
            {
                BTCLBillDetailsModel item = btm.GetSIngleBillDetails(id);
                return View(item);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        [HttpPost]
        public ActionResult BillCancel(BTCLBillDetailsModel model)
        {
            var login = (BKBTitas.Models.ViewModel.LoginModels)Session["LoginCredentials"];

            try
            {

                string TXNNumber, message, resMessage = null, error; 

                SvcRef_BTCLBoss.CrmWebServicesPortTypeClient client = new CrmWebServicesPortTypeClient();
                

                string token = tm.GetToken(GetHeader(), GetTokenRequestBody(), out error);
                if (token != null)
                {
                    SvcRef_BTCLBoss.tPaymentCancelRequest a = BillCancelRequestBody(model,login);

                    logger.Debug(Serializer.GetXMLFromObject(BillCancelRequestBody(model, login)));

                    string response = client.PaymentCancel(GetHeader(), a, out TXNNumber, out message);

                    logger.Debug($"{a.BillNo} Cancel Response:{response} Txn Number:{TXNNumber} Message:{message}");

                    if (response.Equals("0"))
                    {

                        int pOut = btm.CancelBillDetails(model.id, model.CancelReason, login);
                        TempData["retMsg"] = pOut == 0 ? "Cancel Success." : "Cancel Failed.";
                        return RedirectToAction("Index", "BTCL");
                    }
                    else
                    {
                        TempData["retMsg"] = "Error:" + response + "-" + message;
                        logger.Error("Error:" + response + "-" + message);
                        return RedirectToAction("Index", "BTCL");
                    }
                }
                else
                {
                    TempData["retMsg"] = "Connection error between BKB and BTCL";

                    logger.Error("Connection error between BKB and BTCL");

                    return RedirectToAction("Index", "BTCL");
                }

            }
            
            catch (Exception Ex)
            {
                TempData["rteMsg"] = "Error:" + Ex.Message;
                logger.Debug("BTCL Bill Cancel Exception:" + Ex.Message);
                return RedirectToAction("BillCancel", "BTCL");
            }


        }


        public ActionResult CollectionHistory()
        {
            var login = (BKBTitas.Models.ViewModel.LoginModels)Session["LoginCredentials"];
            if (login != null)
            {
                //BTCLBillDetailsModel item = btm.GetSIngleBillDetails(id);
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public ActionResult CollectionHistory(string fromDate, string toDate)
        {
            string message; 
            var login = (BKBTitas.Models.ViewModel.LoginModels)Session["LoginCredentials"];
            if (login != null)
            {
                SvcRef_BTCLBoss.tAuthHeader header = new SvcRef_BTCLBoss.tAuthHeader();
                SvcRef_BTCLBoss.tPaymentHistoryRequest body = new tPaymentHistoryRequest();
                SvcRef_BTCLBoss.tBillDetailDtoList details = new tBillDetailDtoList();
                SvcRef_BTCLBoss.CrmWebServicesPortTypeClient client = new CrmWebServicesPortTypeClient();
                // List<BTCLBillDetailsModel> detLst = BTCLBillDetailsModel();
                BTCLBillRequestModel appResponse = new BTCLBillRequestModel();
               List<BTCLBillDetailsModel> detailDtoList = new List<BTCLBillDetailsModel>();
                string Out = client.PaymentHistory(GetHeader(), GetHistoryRequest(fromDate, toDate), out message, out details);
                if (Out.Equals("0"))
                {
                    //DataSet ds = new DataSet();
                    //DataTable dtBillList = ds.Tables["BillDetailDto"];
                    
                    if (details != null)
                    {
                       foreach(var x in details.BillDetailDto.RowNumber)
                        {
                            BTCLBillDetailsModel bill = new BTCLBillDetailsModel();
                      
                            bill.BillNo= details.BillDetailDto.BillNo;
                           //bill.BillMonthYear= details.BillDetailDto.BillMonthYear;
                            bill.BillMonth= details.BillDetailDto.BillMonth;
                            bill.BillYear = details.BillDetailDto.BillYear;
                            bill.BillPayStatus= details.BillDetailDto.BillPayStatus;
                            bill.BTCLAmount= details.BillDetailDto.BTCLAmount / 100;
                            bill.VAT= details.BillDetailDto.VAT / 100;
                           //bill.TotalAmount= details.BillDetailDto.BTCLAmount+;
                            bill.Name= details.BillDetailDto.Name;
                       
                            detailDtoList.Add(bill);

                        }
                    }
                    appResponse.Details = detailDtoList;
                    return View("CollectionHistoryRes", appResponse);

                }
                else
                {
                    TempData["retMsg"] = "Error:" + Out + "-" + message;
                    logger.Error("Error:" + Out + "-" + message);
                    return RedirectToAction("Index", "BTCL");
                }
              
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        private tPaymentHistoryRequest GetHistoryRequest(string fromDate, string toDate)
        {
            fromDate = fromDate == null || fromDate == "" ? DateTime.Now.ToString() : fromDate;
            toDate = toDate == null || toDate == "" ? DateTime.Now.ToString() : toDate;
            var login = (BKBTitas.Models.ViewModel.LoginModels)Session["LoginCredentials"];
            string error;
            SvcRef_BTCLBoss.tPaymentHistoryRequest request = new SvcRef_BTCLBoss.tPaymentHistoryRequest();
            request.UserID = btclUserName;
            request.TXNNumber = btm.getBtclBankTransactionID().ToString();
            request.TokenNumber = tm.GetToken(GetHeader(), GetTokenRequestBody(), out error);
            request.FromDate = Convert.ToDateTime(fromDate).ToString("yyyyMMddHHmmss");
            request.ToDate = Convert.ToDateTime(toDate).ToString("yyyyMMddHHmmss");
            //request.FromDate = Convert.ToDateTime(fromDate).ToString("YYYYMMDDHHMMSS");
            //request.ToDate = Convert.ToDateTime(toDate).ToString("YYYYMMDDHHMMSS");
            request.BranchCode = btm.getBtclBranchCode(login.BranchCode);
            return request;
        }

        private tBillPaymentRequest GetBillPaymentRequest(BTCLBillDetailsModel payReq)
        {
            var login = (BKBTitas.Models.ViewModel.LoginModels)Session["LoginCredentials"];
            string error;
            SvcRef_BTCLBoss.tBillPaymentRequest request = new SvcRef_BTCLBoss.tBillPaymentRequest();
            request.UserID = btclUserName;
            request.TXNNumber = btm.getBtclBankTransactionID().ToString();
            request.TokenNumber = tm.GetToken(GetHeader(), GetTokenRequestBody(), out error);
            request.BillNo = payReq.BillNo;
            request.PaidAmount = Convert.ToInt64(payReq.TotalAmount * 100);
            request.BankCode = btclBank;
            request.BillStatus = payReq.BillPayStatus;
            request.BranchCode = btm.getBtclBranchCode(login.BranchCode);
            return request;
        }

        public SvcRef_BTCLBoss.tAuthHeader GetHeader()
        {
            SvcRef_BTCLBoss.tAuthHeader header = new SvcRef_BTCLBoss.tAuthHeader();
            header.Username = btclUserID;
            header.Password = btclPassword;
            header.ChannelCode = btclChannel;
            return header;
        }

        public SvcRef_BTCLBoss.tTokenRequest GetTokenRequestBody()
        {
            SvcRef_BTCLBoss.tTokenRequest request = new SvcRef_BTCLBoss.tTokenRequest();
            request.UserID = btclUserName;
            request.Password = btclPass;
            return request;
        }

        public SvcRef_BTCLBoss.tBillRequest GetBillRequestBody(BTCLBillRequestModel billReq)
        {
            string error;
            SvcRef_BTCLBoss.tBillRequest request = new SvcRef_BTCLBoss.tBillRequest();
            request.UserID = btclUserName;
            request.TXNNumber = btm.getBtclBankTransactionID().ToString();
            request.TokenNumber = tm.GetToken(GetHeader(), GetTokenRequestBody(), out error);
            request.PhoneNumber = billReq.SearchType.Equals("PN") ? billReq.BillNumber : "";
            request.BillNo = billReq.SearchType.Equals("BN") ? billReq.BillNumber : "";
            request.ExchangeCode = billReq.ExchangeCode;
            request.AccountNo = billReq.SearchType.Equals("AN") ? billReq.BillNumber : "";
            request.PayStatus = billReq.Paystatus;

            return request;
        }

          public SvcRef_BTCLBoss.tPaymentCancelRequest BillCancelRequestBody(BTCLBillDetailsModel billReq, LoginModels login)
        {
            string error;
            SvcRef_BTCLBoss.tPaymentCancelRequest request = new SvcRef_BTCLBoss.tPaymentCancelRequest();       
            request.TXNNumber = btm.getBtclBankTransactionID().ToString();
            request.UserID = btclUserName;
            request.TokenNumber = tm.GetToken(GetHeader(), GetTokenRequestBody(), out error);
            request.BranchCode = btm.getBtclBranchCode(login.BranchCode);
            request.BankCode = btclBank;
            request.BillNo =billReq.BillNo;          
            request.BillStatus = "P";
            return request;
        }


    }
}