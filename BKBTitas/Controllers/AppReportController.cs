using BKBTitas.EntityModel;
using BKBTitas.Models.AppManager;
using BKBTitas.Models.ViewModel;
using log4net;
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace BKBTitas.Controllers
{
    public class AppReportController : Controller
    {
        ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public string baseURL = ConfigurationManager.AppSettings["baseURL"];
        public string baseURLNonM = ConfigurationManager.AppSettings["baseURLNonM"];
        public string userID = ConfigurationManager.AppSettings["userID"];
        public string password = ConfigurationManager.AppSettings["password"];
        public string accessToken = ConfigurationManager.AppSettings["access_token"];

        public string XAPIKEY = ConfigurationManager.AppSettings["XAPIKEY"];
        public string XAUTHSECRET = ConfigurationManager.AppSettings["XAUTHSECRET"];

        public string pageSized = ConfigurationManager.AppSettings["PageSize"];


        public string MailFrom = ConfigurationManager.AppSettings["MailFrom"];
        public string MailPass = ConfigurationManager.AppSettings["MailPass"];
        public string Host = ConfigurationManager.AppSettings["Host"];
        public string Port = ConfigurationManager.AppSettings["Port"];

        BKBTitasEntities db = new BKBTitasEntities();
        ApplicationManager appMg = new ApplicationManager();
        public ActionResult UserReport()
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

        public ActionResult AdminReport()
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

        public ActionResult MeteredPaymentHistory()
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
        [HttpPost]
        public ActionResult MeteredPaymentHistory(int? page, string dateFrom, string dateTo)
        {
            var login = (BKBTitas.Models.ViewModel.LoginModels)Session["LoginCredentials"];

            if (login != null)
            {
                dateFrom = dateFrom == null ? DateTime.Now.ToString("yyyyMMdd") : dateFrom;
                dateTo = dateTo == null ? DateTime.Now.ToString("yyyyMMdd") : dateTo;
                RptDataViewModel rptData = new RptDataViewModel();
                ErrorViewModel errv = new ErrorViewModel();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseURL);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    //HTTP GET
                    var responseTask = client.GetAsync("metered/payment/list/?transactionDateFrom=" + dateFrom.Replace("-","")+ "&transactionDateTo=" + dateTo.Replace("-", ""));

                    logger.Debug("Metered Bill Payment History on :" + dateFrom.Replace("-", "") + "&transactionDateTo=" + dateTo.Replace("-", ""));

                    try
                    {
                        responseTask.Wait();
                        var result = responseTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var billResponse = result.Content.ReadAsStringAsync().Result;
                            logger.Debug("Metered Bill Response:" + billResponse);

                            rptData = JsonConvert.DeserializeObject<RptDataViewModel>(billResponse);

                            List<BillViewModel> rptDetails = rptData.data.transactions;

                            if (rptData.data != null)
                            {
                                int pageSize = Convert.ToInt16(pageSized);
                                int pageNumber = (page ?? 1);
                                return View(rptDetails.ToPagedList(pageNumber, pageSize));
                            }
                            else
                            {
                                TempData["errMsg"] = "Code: " + rptData.status + "Message: " + rptData.message;
                                return View();
                            }
                        }
                        else
                        {
                            var error = result.Content.ReadAsStringAsync().Result;
                            logger.Debug(error);

                            errv = JsonConvert.DeserializeObject<ErrorViewModel>(error);
                            return Json(error);
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
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        

        public ActionResult NonMeteredPaymentHis()
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
        [HttpPost]
        public ActionResult NonMeteredPaymentHis(int? page, string inputDate)
        {
            var login = (BKBTitas.Models.ViewModel.LoginModels)Session["LoginCredentials"];

            if (login != null)
            {
                inputDate = inputDate == null ? DateTime.Now.ToString("dd/MM/yyyy") : Convert.ToDateTime(inputDate).ToString("dd/MM/yyyy");
                RptDataViewModelNonM rptData = new RptDataViewModelNonM();
                ErrorViewModel errv = new ErrorViewModel();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseURLNonM);

                    var request = new HttpRequestMessage();

                    inputDate = Convert.ToDateTime(inputDate).ToString("dd/MM/yyyy");

                    client.DefaultRequestHeaders.Add("X-API-KEY", XAPIKEY);
                    client.DefaultRequestHeaders.Add("X-AUTH-SECRET", XAUTHSECRET);
                    client.DefaultRequestHeaders.Add("X-ROUTE", appMg.getRoutingNo(Session["BranchCode"].ToString()));
                    //HTTP GET
                    var responseTask = client.GetAsync("bank/api/v1/payments/?voucherDate=" + inputDate);

                    logger.Debug("Non-Metered Bill Payment History on :bank/api/v1/payments/?voucherDate=" + inputDate);

                    try
                    {
                        responseTask.Wait();
                        var result = responseTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var billResponse = result.Content.ReadAsStringAsync().Result;
                            logger.Debug("Non-Metered Bill Response:" + billResponse);

                            rptData = JsonConvert.DeserializeObject<RptDataViewModelNonM>(billResponse);

                            List<BillViewModel> rptDetails = rptData.data;

                            if (rptData.data != null)
                            {
                                int pageSize = Convert.ToInt16(pageSized);
                                int pageNumber = (page ?? 1);
                                return View(rptDetails.ToPagedList(pageNumber, pageSize));
                            }
                            else
                            {
                                TempData["errMsg"] = "Code: " + rptData.status + " Message: " + rptData.message;
                                return View();
                            }
                        }
                        else
                        {
                            var error = result.Content.ReadAsStringAsync().Result;
                            logger.Debug(error);

                            errv = JsonConvert.DeserializeObject<ErrorViewModel>(error);
                            return Json(error);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Debug("Non-Metered Bill Request:" + ex.Message);
                        return Json("status: unknown, message: " + ex.Message);
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

        public ActionResult SendReport()
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

        [HttpPost]
        public ActionResult SendReport(string rptDate)
        {
            var login = (BKBTitas.Models.ViewModel.LoginModels)Session["LoginCredentials"];
            int success = 0, failed = 0;
            rptDate = rptDate == null ? DateTime.Now.ToString("yyyy-MM-dd") : Convert.ToDateTime(rptDate).ToString("yyyy-MM-dd");
            if (login != null)
            {
                try
                {
                    var mailTo = db.TitasZones.Select(i => new { i.zoneName, i.zoneEmail}).Distinct();
                    foreach (var item in mailTo)
                    {
                        try
                        {
                            int pOut = appMg.getData(item.zoneName, rptDate);
                            if (pOut > 0)
                            {
                                string AppLocation = "";
                                AppLocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
                                AppLocation = AppLocation.Replace("file:\\", "");
                                string file = AppLocation + "\\ExcelFiles\\" + item.zoneName + ".xlsx";

                                MailMessage mail = new MailMessage();
                                SmtpClient SmtpServer = new SmtpClient(Host);
                                mail.From = new MailAddress(MailFrom);

                                mail.To.Add(item.zoneEmail);                            // Sending MailTo


                                mail.Subject = "Latest non metered bill collection report on " + rptDate; // Mail Subject

                                mail.Body = "Dear Sir, Please find the bill collection report on "
                                            + rptDate +
                                            " attached herewith. For any kind of discripency please reply to this mail."
                                            + " Regards, BKB Titas Team.";

                                System.Net.Mail.Attachment attachment;
                                if (file != null)
                                {
                                    attachment = new System.Net.Mail.Attachment(file); //Attaching File to Mail
                                    mail.Attachments.Add(attachment);
                                }
                                SmtpServer.Port = Convert.ToInt32(Port); //PORT
                                
                                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                                SmtpServer.UseDefaultCredentials = false;
                                SmtpServer.Credentials = new NetworkCredential(MailFrom, MailPass);
                                SmtpServer.EnableSsl = true;
                                SmtpServer.Send(mail);
                                success = success + 1;
                            }else
                            {
                                failed = failed + 1;
                            }
                        }catch(Exception ex)
                        {
                            failed = failed + 1;
                        }
                    }


                    foreach (var item in mailTo)
                    {
                        try
                        {
                            int pOut = appMg.getDataMeterd(item.zoneName, rptDate);
                            if (pOut > 0)
                            {
                                string AppLocation = "";
                                AppLocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
                                AppLocation = AppLocation.Replace("file:\\", "");
                                string file = AppLocation + "\\ExcelFiles\\" + item.zoneName + "_metered.xlsx";

                                MailMessage mail = new MailMessage();
                                SmtpClient SmtpServer = new SmtpClient(Host);
                                mail.From = new MailAddress(MailFrom);

                                mail.To.Add(item.zoneEmail);                            // Sending MailTo


                                mail.Subject = "Latest meterd bill collection report on " + System.DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"); // Mail Subject

                                mail.Body = "Dear Sir, Please find the lates report on "
                                            + System.DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") +
                                            "attached herewith. For any kind of discripency please reply to this mail."
                                            + "Regards, BKB Titas Team.";

                                System.Net.Mail.Attachment attachment;
                                if (file != null)
                                {
                                    attachment = new System.Net.Mail.Attachment(file); //Attaching File to Mail
                                    mail.Attachments.Add(attachment);
                                }
                                SmtpServer.Port = Convert.ToInt32(Port); //PORT

                                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                                SmtpServer.UseDefaultCredentials = false;
                                SmtpServer.Credentials = new NetworkCredential(MailFrom, MailPass);
                                SmtpServer.EnableSsl = true;
                                SmtpServer.Send(mail);
                                success = success + 1;
                            }
                            else
                            {
                                failed = failed + 1;
                            }
                        }
                        catch (Exception ex)
                        {
                            failed = failed + 1;
                        }
                    }

                    //TempData["retMsg"] = "Total " + success + " sent successfully out of " + success + failed;
                    return RedirectToAction("SendReport", "AppReport");
                }
                catch(Exception ex)
                {
                    TempData["retMsg"] = "Error:" + ex.Message;
                    return RedirectToAction("SendReport", "AppReport");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }



    }
}