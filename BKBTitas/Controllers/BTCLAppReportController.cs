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
    public class BTCLAppReportController : Controller
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
        BTCLAppManager BTCLappMg = new BTCLAppManager();
        public ActionResult BTCLUserReport()
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

        public ActionResult BTCLAdminReport()
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




        public ActionResult SendBTCLReport()
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
        public ActionResult SendBTCLReport(string rptDate)
        {
            var login = (BKBTitas.Models.ViewModel.LoginModels)Session["LoginCredentials"];
            int success = 0, failed = 0;
            rptDate = rptDate == null ? DateTime.Now.ToString("yyyy-MM-dd") : Convert.ToDateTime(rptDate).ToString("yyyy-MM-dd");
            if (login != null)
            {
                try
                {
                    //    var mailTo = db.TitasZones.Select(i => new { i.zoneName, i.zoneEmail }).Distinct();
                    //    foreach (var item in mailTo)
                    //    {
                    try
                    {
                        int pOut = BTCLappMg.getBTCLData(rptDate);
                        if (pOut > 0)
                        {
                            string AppLocation = "";
                            AppLocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
                            AppLocation = AppLocation.Replace("file:\\", "");
                            string file = AppLocation + "\\ExcelFiles\\" + "BTCL_DailyReport" + ".xlsx";

                            MailMessage mail = new MailMessage();
                            SmtpClient SmtpServer = new SmtpClient(Host);
                            mail.From = new MailAddress(MailFrom);

                            // Sending MailTo

                            //mail.To.Add("dirrev1btcl@gmail.com");                            
                            //mail.To.Add("dirrev2btcl@gmail.com");
                            //mail.To.Add("adrevbtcl@gmail.com");
                            //mail.To.Add("shahid.sd3@gmail.com");
                            mail.To.Add("linacsejkkniu@gmail.com");


                            mail.Subject = "Latest BTCL bill collection report on " + rptDate; // Mail Subject

                            mail.Body = "Dear Sir, Please find the BTCL bill collection report on "
                                        + rptDate +
                                        " attached herewith. For any kind of discripency please reply to this mail."
                                        + " Regards, BKB BTCL Team.";

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
                            TempData["retMsg"] = "Mail " + success + " have been  sent successfully ";
                        }
                        else
                        {
                            failed = failed + 1;
                            TempData["retMsg"] = "Mail " + failed + "have been sent unsuccessfully";
                        }


                    }
                    catch (Exception ex)
                    {
                        failed = failed + 1;
                    }


                    
                    return RedirectToAction("SendBTCLReport", "BTCLAppReport");
                }
                catch (Exception ex)
                {
                    TempData["retMsg"] = "Error:" + ex.Message;
                    return RedirectToAction("SendBTCLReport", "BTCLAppReport");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

        }



        public ActionResult SendBTCLMonthlyReport()
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
        public ActionResult SendBTCLMonthlyReport(string rptDateFrom, string rptDateTo)
        {
            var login = (BKBTitas.Models.ViewModel.LoginModels)Session["LoginCredentials"];
            int success = 0, failed = 0;
            rptDateFrom = rptDateFrom == null ? DateTime.Now.ToString("yyyy-MM-dd") : Convert.ToDateTime(rptDateFrom).ToString("yyyy-MM-dd");
            rptDateTo = rptDateTo == null ? DateTime.Now.ToString("yyyy-MM-dd") : Convert.ToDateTime(rptDateTo).ToString("yyyy-MM-dd");
            if (login != null)
            {
                try
                {
                    //    var mailTo = db.TitasZones.Select(i => new { i.zoneName, i.zoneEmail }).Distinct();
                    //    foreach (var item in mailTo)
                    //    {
                    try
                    {
                        int pOut = BTCLappMg.getBTCLMonthlyData(rptDateFrom,rptDateTo);
                        if (pOut > 0)
                        {
                            string AppLocation = "";
                            AppLocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
                            AppLocation = AppLocation.Replace("file:\\", "");
                            string file = AppLocation + "\\ExcelFiles\\" + "BTCL_DailyReport" + ".xlsx";

                            MailMessage mail = new MailMessage();
                            SmtpClient SmtpServer = new SmtpClient(Host);
                            mail.From = new MailAddress(MailFrom);

                            // Sending MailTo

                            //mail.To.Add("dirrev1btcl@gmail.com");                            
                            //mail.To.Add("dirrev2btcl@gmail.com");
                            //mail.To.Add("adrevbtcl@gmail.com");
                            //mail.To.Add("shahid.sd3@gmail.com");
                            mail.To.Add("linacsejkkniu@gmail.com");


                            mail.Subject = "Latest BTCL bill collection report Between " + rptDateFrom+" To "+ rptDateTo; // Mail Subject

                            mail.Body = "Dear Sir, Please find the BTCL bill collection report Between " + rptDateFrom + " To " + rptDateTo+
                                        " attached herewith. For any kind of discripency please reply to this mail."
                                        + " Regards, BKB BTCL Team.";

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
                            TempData["retMsg"] = "Mail " + success + " have been  sent successfully ";
                        }
                        else
                        {
                            failed = failed + 1;
                            TempData["retMsg"] = "Mail " + failed + "have been sent unsuccessfully";
                        }


                    }
                    catch (Exception ex)
                    {
                        failed = failed + 1;
                    }


                    //TempData["retMsg"] = "Total " + success + " sent successfully out of " + success + failed;
                    return RedirectToAction("SendBTCLMonthlyReport", "BTCLAppReport");
                }
                catch (Exception ex)
                {
                    TempData["retMsg"] = "Error:" + ex.Message;
                    return RedirectToAction("SendBTCLMonthlyReport", "BTCLAppReport");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }



        }





    }
}