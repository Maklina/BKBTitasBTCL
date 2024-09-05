using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BKBTitas.Models.ViewModel;
using BKBTitas.EntityModel;
using System.Globalization;
using System.Net.Http;
using System.Configuration;
using System.Text;
using System.Net.Http.Headers;
using log4net;
using System.Reflection;
using BKBTitas.Models.AppManager;
using Newtonsoft.Json;
using PagedList;

namespace BKBTitas.Controllers
{
    public class NMBillnDDController : Controller
    {
        // GET: NMBillnDD
        ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public string baseURL = ConfigurationManager.AppSettings["baseURLNonM"];
        public string XAPIKEY = ConfigurationManager.AppSettings["XAPIKEY"];
        public string XAUTHSECRET = ConfigurationManager.AppSettings["XAUTHSECRET"];
        public string pageSized = ConfigurationManager.AppSettings["PageSize"];

        ApplicationManager appMg = new ApplicationManager();

        BKBTitasEntities db = new BKBTitasEntities();


        public ActionResult Index(int? page, string currentFilter, string searchString)
        {
            var login = (BKBTitas.Models.ViewModel.LoginModels)Session["LoginCredentials"];

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.currentFilter = searchString;

            if (login != null)
            {
                string curDate = System.DateTime.Now.ToString("dd/MM/yyyy");
                var billDet = from x in db.NonMeterBillDetails where x.voucherDate == curDate 
                                        && x.transactionStatus=="0"
                                        && x.invoiceNo== "000000"
                              select new BillViewModel
                                {
                                    branchCode = x.branchCode,
                                    branchRoutingNo = x.branchRoutingNo,
                                    customerCode = x.customerCode,
                                    paymentId = x.paymentId,
                                    customerName = x.CustomerName,
                                    customerAddress = x.custAddress,
                                    customerMobile = x.CustomerMobile,
                                    bankTransactionId = x.bankTransactionId,
                                    amount = x.invoiceAmount,
                                    surcharge = x.paidAmount-x.invoiceAmount,
                                    totalAmount = x.paidAmount
                                };

                if (login.UserRoleId != 1)
                {
                    billDet = billDet.Where(o => o.branchCode.Equals(login.BranchCode));
                }
                if (searchString != null)
                {
                    billDet = billDet.Where(o => o.branchCode.Equals(searchString) || o.customerCode.Contains(searchString) || o.paymentId.ToString().Contains(searchString) || o.bankTransactionId.Equals(searchString));
                }
                List<BillViewModel> billList = billDet.OrderBy(x => x.paymentId).ToList();
                int pageSize = Convert.ToInt16(pageSized);
                int pageNumber = (page ?? 1);
                return View(billList.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult NMBill()
        {
            var login = (BKBTitas.Models.ViewModel.LoginModels)Session["LoginCredentials"];

            if (login != null)
            {
                BillViewModel bvm = (BillViewModel)TempData["billInfo"];
                return View(bvm);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public JsonResult getCustomerData(string customerCode)
        {
            TxnDataViewModel txnData = new TxnDataViewModel();
            ErrorViewModel errv = new ErrorViewModel();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);

                var request = new HttpRequestMessage();


                client.DefaultRequestHeaders.Add("X-API-KEY", XAPIKEY);
                client.DefaultRequestHeaders.Add("X-AUTH-SECRET", XAUTHSECRET);
                client.DefaultRequestHeaders.Add("X-ROUTE", appMg.getRoutingNo(Session["BranchCode"].ToString()));

                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                //HTTP GET
                var responseTask = client.GetAsync("bank/api/v1/customers/" + customerCode);

                logger.Debug("Non-Metered Bill Request: bank/api/v1/customers/" + customerCode);

                try
                {
                    responseTask.Wait();
                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var billResponse = result.Content.ReadAsStringAsync().Result;
                        logger.Debug("Non-Metered Bill Response:" + billResponse);

                        txnData = JsonConvert.DeserializeObject<TxnDataViewModel>(billResponse);

                        return Json(txnData);
                    }
                    else
                    {
                        var error = result.Content.ReadAsStringAsync().Result;
                        logger.Error(error);

                        errv = JsonConvert.DeserializeObject<ErrorViewModel>(error);
                        return Json(errv);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("Metered Bill Request:" + ex.Message);
                    return Json("status: unknown, message: " + ex.Message);
                }
                finally
                {
                    client.Dispose();
                }
            }
        }

        [HttpPost]
        public ActionResult NMBill(BillViewModel BVM)
        {
            var login = (BKBTitas.Models.ViewModel.LoginModels)Session["LoginCredentials"];
            TxnDataViewModel txnData = new TxnDataViewModel();
            ErrorViewModel errv = new ErrorViewModel();
            
            if (login != null)
            {

                if (BVM.paidAmount != BVM.totalAmount)
                {
                    TempData["retMsg"] = "Total amount and bill amount is not same!";
                    return RedirectToAction("NMBill", "NMBillnDD");
                }


                string currDate = System.DateTime.Now.ToString("yyyy-MM-dd");
                var isPaid = db.NonMeterBillDetails.Where(o => o.branchCode == login.BranchCode && o.customerCode == BVM.customerCode &&
                           o.transactionDate == currDate).SingleOrDefault();

                if (isPaid != null)
                {
                    TempData["retMsg"] = "Customer has already paid Today. Same Branh is not authorized to pay more than once in a day for a customer.";
                    return RedirectToAction("NMBill", "NMBillnDD");
                }
                else
                {
                    //BVM.particulars = appMg.getPeriod(BVM.dtFrom, BVM.dtTo);

                    BVM.bankTransactionId = appMg.getBankTransactionID();

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(baseURL);

                        var request = new HttpRequestMessage();

                        string parameter = "bank/api/v1/payments/?customer=" + BVM.payId + "&amount=" + BVM.invoiceAmount
                            + "&surcharge=" + BVM.surchargeAmount + "&particulars=" + BVM.particulars + "&bankTransactionId=" + BVM.bankTransactionId + "&cellPhone=" + BVM.mobile;

                        request = new HttpRequestMessage(HttpMethod.Post, parameter);

                        client.DefaultRequestHeaders.Add("X-API-KEY", XAPIKEY);
                        client.DefaultRequestHeaders.Add("X-AUTH-SECRET", XAUTHSECRET);
                        client.DefaultRequestHeaders.Add("X-ROUTE", appMg.getRoutingNo(Session["BranchCode"].ToString()));

                        logger.Debug("Non-Metered Bill Post Request:"+parameter);
                        try
                        {
                            int cout = 0;
                            if (cout == 0)
                            {
                                var responseTask = client.SendAsync(request);
                                responseTask.Wait();
                                var result = responseTask.Result;
                                if (result.IsSuccessStatusCode)
                                {
                                    var billResponse = result.Content.ReadAsStringAsync().Result;

                                    logger.Debug("Non-Metered Bill Post Response:" + billResponse);

                                    txnData = JsonConvert.DeserializeObject<TxnDataViewModel>(billResponse);
                                    if (txnData.data != null)
                                    {
                                        txnData.data.customerCode = BVM.customerCode;
                                        txnData.data.customerName = BVM.customerName;
                                        txnData.data.mobile = BVM.mobile;
                                        txnData.data.connectionAddress = BVM.connectionAddress;
                                        txnData.data.applicationSummary = BVM.applicationSummary;

                                        cout = appMg.saveNonMeterBillDet(txnData.data, login);
                                    }
                                    else
                                    {
                                        TempData["errMsg"] = "Code: " + txnData.status + " Message: " + txnData.message;
                                        BVM.message = txnData.message;
                                        return RedirectToAction("NMBill", "NMBillnDD");
                                    }
                                }
                                else
                                {
                                    var error = result.Content.ReadAsStringAsync().Result;
                                    logger.Error(error);

                                    errv = JsonConvert.DeserializeObject<ErrorViewModel>(error);
                                    txnData.status = errv.status;
                                    txnData.message = errv.message;
                                    cout = 1;
                                }
                            }
                            if (cout == 0)
                            {
                                TempData["sucMsg"] = "Code: " + txnData.status + " Message: " + txnData.message + " Transaction ID:" + txnData.data.payId;
                                TempData["billInfo"] = txnData.data;
                                return RedirectToAction("NMBill", "NMBillnDD");
                            }
                            else
                            {
                                TempData["errMsg"] = "Code: " + txnData.status + " Message: " + txnData.message;
                                return RedirectToAction("NMBill", "NMBillnDD");
                            }
                        }
                        catch (Exception Ex)
                        {
                            TempData["errMsg"] = "Connection Error/ Unauthorized Access.Error:" + Ex.Message;
                            logger.Error("Metered Bill Post Exception:" + Ex.Message);
                            return RedirectToAction("NMBill", "NMBillnDD");
                        }
                        finally
                        {
                            client.Dispose();
                        }
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        public ActionResult CancelPayment(int? paymentId)
        {
            var login = (BKBTitas.Models.ViewModel.LoginModels)Session["LoginCredentials"];

            if (login != null)
            {
                BillViewModel bvm = new BillViewModel();
                if (TempData["deleteInfo"] != null)
                {
                    bvm = (BillViewModel)TempData["deleteInfo"];
                }
                else
                {
                    bvm.paymentId = Convert.ToInt32(paymentId);
                }
                return View(bvm);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public ActionResult CancelPayment(BillViewModel BVM)
        {
            var login = (BKBTitas.Models.ViewModel.LoginModels)Session["LoginCredentials"];

            TxnDataViewModel txnData = new TxnDataViewModel();
            ErrorViewModel errv = new ErrorViewModel();

            //SuccessResponse sucMsg = new SuccessResponse();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                var request = new HttpRequestMessage();
                client.DefaultRequestHeaders.Add("X-API-KEY", XAPIKEY);
                client.DefaultRequestHeaders.Add("X-AUTH-SECRET", XAUTHSECRET);
                client.DefaultRequestHeaders.Add("X-ROUTE", appMg.getRoutingNo(Session["BranchCode"].ToString()));
                int cout = 0;
                try
                {
                    request = new HttpRequestMessage(HttpMethod.Delete, "bank/api/v1/payments/" + BVM.paymentId);

                    logger.Debug("Non-Metered Cancel Request:bank/api/v1/payments/" + BVM.paymentId);

                    var responseTask = client.SendAsync(request);
                    responseTask.Wait();
                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var billResponse = result.Content.ReadAsStringAsync().Result;

                        logger.Debug("Non-Metered Cancel Response:" + billResponse);

                        txnData = JsonConvert.DeserializeObject<TxnDataViewModel>(billResponse);
                        if (txnData.status == "success")
                        {
                            txnData.data.reason = BVM.reason;
                            TempData["deleteInfo"] = txnData.data;
                            cout = appMg.updateNonMeteredTxnData(txnData.data, login, "1");
                            TempData["sucMsg"] = "Code: " + txnData.status + " Message: " + txnData.message;
                            return RedirectToAction("CancelPayment", "NMBillnDD");
                        }
                        else
                        {
                            TempData["errMsg"] = "Code: " + txnData.status + " Message: " + txnData.message;
                            return RedirectToAction("CancelPayment", "NMBillnDD");
                        }
                    }
                    else
                    {
                        TempData["errMsg"] = "Unknown Error!";
                        return RedirectToAction("CancelPayment", "NMBillnDD");
                    }
                }
                catch (Exception Ex)
                {
                    TempData["rteMsg"] = "Error:" + Ex.Message;
                    logger.Error("Non-Metered Cancel Exception:" + Ex.Message);
                    return RedirectToAction("CancelPayment", "NMBillnDD");
                }
                finally
                {
                    client.Dispose();
                }
            }
        }

        public ActionResult NonMtDNoteList(int? page, string currentFilter, string searchString)
        {
            var login = (BKBTitas.Models.ViewModel.LoginModels)Session["LoginCredentials"];

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.currentFilter = searchString;

            if (login != null)
            {
                string curDate = System.DateTime.Now.ToString("dd/MM/yyyy");
                var billDet = from x in db.NonMeterBillDetails
                              where x.voucherDate == curDate
                                   && x.transactionStatus == "0"
                                   && x.invoiceNo != "000000"
                              select new BillViewModel
                              {
                                  branchCode = x.branchCode,
                                  branchRoutingNo = x.branchRoutingNo,
                                  customerCode = x.customerCode,
                                  paymentId = x.paymentId,
                                  customerName = x.CustomerName,
                                  customerAddress = x.custAddress,
                                  customerMobile = x.CustomerMobile,
                                  bankTransactionId = x.bankTransactionId,
                                  invoiceNo=x.invoiceNo,
                                  amount = x.paidAmount,
                                  surcharge = x.paidAmount - x.invoiceAmount,
                                  totalAmount = x.paidAmount,
                                  revenueStamp = x.paidAmount > 400 ? 10 : 0
                              };

                if (login.UserRoleId != 1)
                {
                    billDet = billDet.Where(o => o.branchCode.Equals(login.BranchCode));
                }
                if (searchString != null)
                {
                    billDet = billDet.Where(o => o.branchCode.Equals(searchString) || o.customerCode.Contains(searchString) || o.paymentId.ToString().Contains(searchString) || o.bankTransactionId.Equals(searchString));
                }
                List<BillViewModel> billList = billDet.OrderBy(x => x.paymentId).ToList();
                int pageSize = Convert.ToInt16(pageSized);
                int pageNumber = (page ?? 1);
                return View(billList.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult NMDemandD()
        {
            var login = (BKBTitas.Models.ViewModel.LoginModels)Session["LoginCredentials"];

            if (login != null)
            {
                BillViewModel bvm = (BillViewModel)TempData["demandNoteInfo"];
                return View(bvm);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public JsonResult getNonMeterDNote(string invoiceNo)
        {
            TxnDataViewModel txnData = new TxnDataViewModel();
            ErrorViewModel errv = new ErrorViewModel();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                //HTTP GET

                HttpRequestMessage request = new HttpRequestMessage();

                client.DefaultRequestHeaders.Add("X-API-KEY", XAPIKEY);
                client.DefaultRequestHeaders.Add("X-AUTH-SECRET", XAUTHSECRET);
                client.DefaultRequestHeaders.Add("X-ROUTE", appMg.getRoutingNo(Session["BranchCode"].ToString()));
                //HTTP GET
                var responseTask = client.GetAsync("bank/api/v1/demand-note/invoice/" + invoiceNo);

                logger.Debug("Non-Metered Demand Draft Request:bank/api/v1/demand-note/invoice/" + invoiceNo);

                try
                {
                    responseTask.Wait();
                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var billResponse = result.Content.ReadAsStringAsync().Result;

                        logger.Debug(billResponse);

                        txnData = JsonConvert.DeserializeObject<TxnDataViewModel>(billResponse);

                        txnData.data.bankTransactionId = appMg.getBankTransactionID();
                        return Json(txnData);
                    }
                    else
                    {
                        var error = result.Content.ReadAsStringAsync().Result;
                        logger.Error(error);

                        errv = JsonConvert.DeserializeObject<ErrorViewModel>(error);
                        return Json(errv);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("Non Metered Demand Draft Request:" + ex.Message);
                    return Json("status: unknown, message: " + ex.Message);
                }
                finally
                {
                    client.Dispose();
                }
            }
        }

        [HttpPost]
        public ActionResult NMDemandD(BillViewModel BVM)
        {
            var login = (BKBTitas.Models.ViewModel.LoginModels)Session["LoginCredentials"];
            TxnDataViewModel txnData = new TxnDataViewModel();
            ErrorViewModel errv = new ErrorViewModel();

            //string billPeriod = appMg.getPeriod(BVM.dtFrom, BVM.dtTo);
            if (login != null)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseURL);
                    var request = new HttpRequestMessage();

                    string parameter = "bank/api/v1/demand-note/payments/?invoiceNo=" + BVM.invoiceNo + "&customerId=" + BVM.customerId + "&bankTransactionId=" + BVM.bankTransactionId;

                    request = new HttpRequestMessage(HttpMethod.Post, parameter);

                    logger.Debug(parameter);


                    client.DefaultRequestHeaders.Add("X-API-KEY", XAPIKEY);
                    client.DefaultRequestHeaders.Add("X-AUTH-SECRET", XAUTHSECRET);
                    client.DefaultRequestHeaders.Add("X-ROUTE", appMg.getRoutingNo(Session["BranchCode"].ToString()));

                    try
                    {
                        int cout = 0;
                        if (cout == 0)
                        {
                            var responseTask = client.SendAsync(request);
                            responseTask.Wait();
                            var result = responseTask.Result;
                            if (result.IsSuccessStatusCode)
                            {
                                var billResponse = result.Content.ReadAsStringAsync().Result;

                                logger.Debug("Non-Metered Demand Note Response:" + billResponse);

                                txnData = JsonConvert.DeserializeObject<TxnDataViewModel>(billResponse);
                                if (txnData.data != null)
                                {
                                    txnData.data.invoiceNo = BVM.invoiceNo;
                                    txnData.data.customerCode = BVM.customerCode;
                                    txnData.data.customerName = BVM.customerName;
                                    txnData.data.mobile = BVM.mobile;
                                    txnData.data.connectionAddress = BVM.connectionAddress;
                                    txnData.data.applicationSummary = BVM.applicationSummary;
                                    txnData.data.paidAmount = BVM.paymentAmount;

                                    cout = appMg.saveNonMeterBillDet(txnData.data, login);
                                }
                                else
                                {
                                    TempData["errMsg"] = "Code: " + txnData.status + " Message: " + txnData.message;
                                    BVM.message = txnData.message;
                                    return RedirectToAction("NMDemandD", "NMBillnDD");
                                }
                            }
                            else
                            {
                                var error = result.Content.ReadAsStringAsync().Result;
                                logger.Error(error);

                                errv = JsonConvert.DeserializeObject<ErrorViewModel>(error);
                                txnData.status = errv.status;
                                txnData.message = errv.message;
                                cout = 1;
                            }
                        }
                        if (cout == 0)
                        {
                            TempData["sucMsg"] = "Code: " + txnData.status + " Message: " + txnData.message + " Transaction ID:" + txnData.data.payId;
                            TempData["demandNoteInfo"] = txnData.data;
                            return RedirectToAction("NMDemandD", "NMBillnDD");
                        }
                        else
                        {
                            TempData["errMsg"] = "Code: " + txnData.status + " Message: " + txnData.message;
                            return RedirectToAction("NMDemandD", "NMBillnDD");
                        }
                    }
                    catch (Exception Ex)
                    {
                        TempData["errMsg"] = "Connection Error/ Unauthorized Access.Error:" + Ex.Message;
                        logger.Error("Metered Bill Post Exception:" + Ex.Message);
                        return RedirectToAction("NMDemandD", "NMBillnDD");
                    }
                    finally
                    {
                        client.Dispose();
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult CancelNonDNote(int? paymentId)
        {
            var login = (BKBTitas.Models.ViewModel.LoginModels)Session["LoginCredentials"];

            if (login != null)
            {
                BillViewModel bvm = new BillViewModel();
                if (TempData["deleteInfo"] != null)
                {
                    bvm = (BillViewModel)TempData["deleteInfo"];
                }
                else
                {
                    bvm.paymentId = Convert.ToInt32(paymentId);
                }
                return View(bvm);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public ActionResult CancelNonDNote(BillViewModel BVM)
        {
            var login = (BKBTitas.Models.ViewModel.LoginModels)Session["LoginCredentials"];

            TxnDataViewModel txnData = new TxnDataViewModel();
            ErrorViewModel errv = new ErrorViewModel();

            //SuccessResponse sucMsg = new SuccessResponse();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                var request = new HttpRequestMessage();
                client.DefaultRequestHeaders.Add("X-API-KEY", XAPIKEY);
                client.DefaultRequestHeaders.Add("X-AUTH-SECRET", XAUTHSECRET);
                client.DefaultRequestHeaders.Add("X-ROUTE", appMg.getRoutingNo(Session["BranchCode"].ToString()));
                int cout = 0;
                try
                {
                    request = new HttpRequestMessage(HttpMethod.Delete, "bank/api/v1/demand-note/payments/" + BVM.paymentId);

                    logger.Debug("Non-Metered D-Note Cancel Request:bank/api/v1/demand-note/payments/" + BVM.paymentId);

                    var responseTask = client.SendAsync(request);
                    responseTask.Wait();
                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var billResponse = result.Content.ReadAsStringAsync().Result;

                        logger.Debug("Non-Metered D-Note Cancel Response:" + billResponse);

                        txnData = JsonConvert.DeserializeObject<TxnDataViewModel>(billResponse);
                        if (txnData.status == "success")
                        {
                            txnData.data.reason = BVM.reason;
                            TempData["deleteInfo"] = txnData.data;
                            cout = appMg.updateNonMeteredTxnData(txnData.data, login, "1");
                            TempData["sucMsg"] = "Code: " + txnData.status + " Message: " + txnData.message;
                            return RedirectToAction("CancelNonDNote", "NMBillnDD");
                        }
                        else
                        {
                            TempData["errMsg"] = "Code: " + txnData.status + " Message: " + txnData.message;
                            return RedirectToAction("CancelNonDNote", "NMBillnDD");
                        }
                    }
                    else
                    {
                        TempData["errMsg"] = "Unknown Error!";
                        return RedirectToAction("CancelNonDNote", "NMBillnDD");
                    }
                }
                catch (Exception Ex)
                {
                    TempData["rteMsg"] = "Error:" + Ex.Message;
                    logger.Error("Non-Metered D-Note Cancel Exception:" + Ex.Message);
                    return RedirectToAction("CancelNonDNote", "NMBillnDD");
                }
                finally
                {
                    client.Dispose();
                }
            }
        }
    }
}