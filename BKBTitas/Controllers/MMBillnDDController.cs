using BKBTitas.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using System.Net.Http;
using BKBTitas.EntityModel;
using log4net;
using System.Reflection;
using BKBTitas.Models.AppManager;
using PagedList;

namespace BKBTitas.Controllers
{
    public class MMBillnDDController : Controller
    {
        // GET: MMBillnDD
        ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public string baseURL = ConfigurationManager.AppSettings["baseURL"];
        public string userID = ConfigurationManager.AppSettings["userID"];
        public string password = ConfigurationManager.AppSettings["password"];
        public string accessToken = ConfigurationManager.AppSettings["access_token"];
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
                string curDate = System.DateTime.Now.ToString("yyyyMMdd");
                var billDet = from x in db.MeteredBillDetails
                              where x.transactionDate == curDate
                              select new BillViewModel
                              {
                                  branchCode = x.branchCode,
                                  branchRoutingNo = x.branchRoutingNo,
                                  invoiceNo = x.invoiceNo,
                                  customerCode = x.customerCode,
                                  paymentId = x.paymentId,
                                  customerName = x.CustomerName,
                                  bankTransactionId = x.refNo,
                                  invoiceAmount=x.invoiceAmount,
                                  sourceTaxAmount=x.sourceTaxAmount,
                                  paidAmount = x.paidAmount,
                                  revenueStamp=x.revenueStamp,
                                  transactionStatus = x.transactionStatus
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
        public ActionResult MBill()
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

        public JsonResult getCustomerData(string customerCode, string invoiceNo)
        {
            TxnDataViewModel txnData = new TxnDataViewModel();
            ErrorViewModel errv = new ErrorViewModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                //HTTP GET
                var responseTask = client.GetAsync("metered/invoice/search?invoiceNo=" + invoiceNo + "&customerCode=" + customerCode);

                logger.Debug("Metered Bill Request:metered/invoice/search?invoiceNo=" + invoiceNo + "&customerCode=" + customerCode);

                try
                {
                    responseTask.Wait();
                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var billResponse = result.Content.ReadAsStringAsync().Result;
                        logger.Debug("Metered Bill Response:" + billResponse);

                        txnData = JsonConvert.DeserializeObject<TxnDataViewModel>(billResponse);
                        txnData.data.refNo = appMg.getBankTransactionID();
                        return Json(txnData);
                    }
                    else
                    {
                        var error = result.Content.ReadAsStringAsync().Result;
                        logger.Debug(error);

                        errv = JsonConvert.DeserializeObject<ErrorViewModel>(error);
                        return Json(errv);
                    }
                }
                catch (Exception ex)
                {
                    logger.Debug("Metered Bill Request:" + ex.Message);
                    return Json("status: unknown, message: " + ex.Message);
                }
                finally
                {
                    client.Dispose();
                }
            }
        }

        [HttpPost]
        public ActionResult MBill(BillViewModel BVM)
        {
            var login = (BKBTitas.Models.ViewModel.LoginModels)Session["LoginCredentials"];

            if (BVM.paidAmount != BVM.totalAmount)
            {
                TempData["retMsg"] = "Total amount and bill amount is not same!";
                return View();
            }

            using (var client = new HttpClient())
            {
                TxnDataViewModel txnData = new TxnDataViewModel();
                ErrorViewModel errv = new ErrorViewModel();

                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var request = new HttpRequestMessage();

                BVM.chalanDate = BVM.chalanDate == null ? "" : Convert.ToDateTime(BVM.chalanDate).ToString("yyyyMMdd");

                string parameter= "metered/payment/add?invoiceNo=" + BVM.invoiceNo + "&customerCode=" + BVM.customerCode
                    + "&paidAmount=" + (BVM.paidAmount - 10) + "&sourceTaxAmount=" + BVM.sourceTaxAmount + "&revenueStamp=" + 10 + "&transactionDate=" + System.DateTime.Now.ToString("yyyyMMdd")
                    + "&branchRoutingNo=" + Session["RoutingNo"] + "&operator=" + login.UserId + "&refNo=" + BVM.refNo;

                if(BVM.sourceTaxAmount > 0)
                {
                    parameter = parameter +  "&chalanNo=" + BVM.chalanNo + "&chalanDate=" + BVM.chalanDate 
                             + "&chalanBranch=" + BVM.chalanBranch + "&chalanBank=" + BVM.chalanBank + "&branchCode=" +login.BranchCode;
                }

                logger.Debug("Metered Bill Post Request:" + parameter);

                request = new HttpRequestMessage(HttpMethod.Post, parameter);

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

                            logger.Debug("Metered Bill Post Response:" + billResponse);

                            txnData = JsonConvert.DeserializeObject<TxnDataViewModel>(billResponse);
                            if (txnData.data!=null)
                            {
                                txnData.data.issueDate = BVM.issueDate;
                                txnData.data.issueMonth = BVM.issueMonth;
                                txnData.data.customerName = BVM.customerName;
                                txnData.data.customerMobile = BVM.customerMobile;
                                txnData.data.bankTransactionId = BVM.refNo;

                                cout = appMg.saveTxnBillDetails(txnData.data, login);
                            }else
                            {
                                TempData["errMsg"] = "Code: " + txnData.status + "Message: " + txnData.message;
                                BVM.message = txnData.message;
                                return RedirectToAction("MBill", "MMBillnDD");
                            }
                        }
                        else
                        {
                            cout = 1;
                        }
                    }
                    if (cout == 0)
                    {
                        TempData["sucMsg"] = "Code: " + txnData.status + " Message: " + txnData.message +" Transaction ID:" + txnData.data.paymentId;
                        return RedirectToAction("MBill", "MMBillnDD");
                    }
                    else
                    {
                        TempData["errMsg"] = "Code: " + txnData.status + "Message: " + txnData.message;
                        return RedirectToAction("MBill", "MMBillnDD");
                    }
                }
                catch (Exception Ex)
                {
                    TempData["errMsg"] = "Connection Error/ Unauthorized Access.Error:" + Ex.Message;
                    logger.Debug("Metered Bill Post Exception:" + Ex.Message);
                    return RedirectToAction("MBill", "MMBillnDD");
                }
                finally
                {
                    client.Dispose();
                }
            }
        }

        public ActionResult MDemandD()
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

        public ActionResult CancelPayment(string invoiceNo, string customerCode)
        {
            var login = (BKBTitas.Models.ViewModel.LoginModels)Session["LoginCredentials"];

            if (login != null)
            {
                BillViewModel bvm = new BillViewModel();
                bvm.invoiceNo = invoiceNo;
                bvm.customerCode = customerCode;
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
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var request = new HttpRequestMessage();
                int cout = 0;
                try
                {
                    string parameter = "metered/payment/cancel?invoiceNo=" + BVM.invoiceNo + "&customerCode=" + BVM.customerCode
                        + "&operator=" + login.UserId + "&reason=" + BVM.reason;

                    request = new HttpRequestMessage(HttpMethod.Post, parameter);

                    logger.Debug("Metered Cancel Request:"+parameter);

                    var responseTask = client.SendAsync(request);
                    responseTask.Wait();
                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var billResponse = result.Content.ReadAsStringAsync().Result;

                        logger.Debug("Metered Cancel Response:" + billResponse);

                        txnData = JsonConvert.DeserializeObject<TxnDataViewModel>(billResponse);
                        if (txnData.status == "200")
                        {
                            BVM.message = txnData.message;
                            cout = appMg.updateMeteredTxnData(BVM, login, "1");
                            TempData["sucMsg"] = "Code: "+ txnData.status +"Message: "+txnData.message;
                            return RedirectToAction("CancelPayment", "MMBillnDD");
                        }
                        else
                        {
                            TempData["errMsg"] = "Code: " + txnData.status + "Message: " + txnData.message;
                            return RedirectToAction("CancelPayment", "MMBillnDD");
                        }
                    }
                    else
                    {
                        TempData["errMsg"] = "Unknown Error!";
                        return RedirectToAction("CancelPayment", "MMBillnDD");
                    }
                }
                catch (Exception Ex)
                {
                    TempData["rteMsg"] = "Error:"+Ex.Message;
                    logger.Debug("Metered Cancel Exception:" + Ex.Message);
                    return RedirectToAction("CancelPayment", "MMBillnDD");
                }
                finally
                {
                    client.Dispose();
                }
            }
        }

        public JsonResult getMeteredDemandData(string customerCode, string invoiceNo)
        {
            BillViewModel custDetails = new BillViewModel();
            using (var client = new HttpClient())
            {
                //int flatID = db.SecurityRealStateFlats.DefaultIfEmpty().Max(x => x == null ? 0 : x.id);
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                //HTTP GET
                var responseTask = client.GetAsync("metered/invoice/search?invoiceNo=" + invoiceNo + "&customerCode=" + customerCode);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var billResponse = result.Content.ReadAsStringAsync().Result;

                    try
                    {
                        return Json(billResponse);
                    }
                    catch (Exception ex)
                    {
                        return Json(billResponse);
                        //return RedirectToAction("Error", sucMessage);
                    }
                    finally
                    {
                        client.Dispose();
                    }
                }
                else
                {
                    return Json(result);
                }
            }
        }
    }
}