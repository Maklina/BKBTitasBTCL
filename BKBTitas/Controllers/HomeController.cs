using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Net.Http;

using BKBTitas.Models.ViewModel;
using BKBTitas.EntityModel;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using System.Configuration;
using System.IO;

namespace KGDCL.Controllers
{
    public class HomeController : Controller
    {
        public string baseURL = ConfigurationManager.AppSettings["baseURL"];
        public string userID = ConfigurationManager.AppSettings["userID"];
        public string password = ConfigurationManager.AppSettings["password"];
        BKBTitasEntities db = new BKBTitasEntities();

        // GET: Home
        public ActionResult Index()
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
        public ActionResult Welcome()
        {
            LoginModels logIn = new LoginModels();
            var login = (BKBTitas.Models.ViewModel.LoginModels)Session["LoginCredentials"];

            if (login != null)
            {
                logIn.UserId = login.UserId.ToString();
                return View(logIn);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
    }
}