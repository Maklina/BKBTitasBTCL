using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BKBTitas.EntityModel;
using BKBTitas.Models.ViewModel;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using BKBTitas.Models.AppManager;
using PagedList;

namespace BKBTitas.Controllers
{
    public class AccountController : Controller
    {
        
        BKBTitasEntities db = new BKBTitasEntities();

        public string pageSized = ConfigurationManager.AppSettings["PageSize"];

        public ActionResult BranchList(int? page, string currentFilter, string searchString)
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
                var branchDet = from x in db.Branches
                              select new BranchViewModel
                              {
                                  branch_code = x.branch_code,
                                  branch_name = x.branch_name,
                                  region = x.region,
                                  BBCode = x.BBCode,
                                  RoutingNo = x.RoutingNo
                              };
                if (login.UserRoleId != 1)
                {
                    branchDet = branchDet.Where(o => o.branch_code.Equals(login.BranchCode));
                }
                if (searchString != null)
                {
                    branchDet = branchDet.Where(o => o.branch_code.Equals(searchString) || o.branch_name.Contains(searchString) || o.agentID.Equals(searchString));
                }
                List<BranchViewModel> userList = branchDet.OrderBy(x => x.branch_code).ToList();
                int pageSize = Convert.ToInt16(pageSized);
                int pageNumber = (page ?? 1);
                return View(userList.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

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
                var userDet = from x in db.LoginUsers
                              join br in db.Branches on x.branch_code equals br.branch_code
                              join rl in db.Roles on x.userRole equals rl.id
                              select new UserSignUpModel
                              {
                                  userID = x.userID,
                                  userName = x.userName,
                                  userRole = x.userRole,
                                  roleName = rl.role_name,
                                  branch_code = x.branch_code,
                                  branchName = br.branch_name,
                                  user_status = x.user_status.Equals("A") ? "Active" : "In-Active",
                                  UserMobile = x.UserMobile
                              };
                if(login.UserRoleId != 1)
                {
                    userDet = userDet.Where(o => o.branch_code.Equals(login.BranchCode));
                }
                if(searchString != null)
                {
                    userDet = userDet.Where(o => o.userID.Equals(searchString) || o.userName.Contains(searchString) || o.UserMobile.Contains(searchString));
                }
                List<UserSignUpModel> userList = userDet.OrderBy(x => x.userRole).ToList();
                int pageSize = Convert.ToInt16(pageSized);
                int pageNumber = (page ?? 1);
                return View(userList.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult Create()
        {
            var login = (BKBTitas.Models.ViewModel.LoginModels)Session["LoginCredentials"];
            int rolP = Convert.ToInt32(login.role_priority);
            UserSignUpModel usm = new UserSignUpModel();

            if (login != null)
            {
                var queryRole = from q in db.Roles where  q.role_priority > rolP
                            select new
                            {
                                value = q.id,
                                description = q.role_name
                            };
                usm.roleList = new SelectList(queryRole.ToList(), "value", "description");

                var query = from q in db.Branches
                            select new
                            {
                                value = q.branch_code,
                                description = q.branch_code + "--" + q.branch_name
                            };

                if (login.UserRoleId != 1)
                {
                    query = query.Where(o => o.value.Equals(login.BranchCode));
                }

                usm.BranchList = new SelectList(query.ToList(), "value", "description");

                return View(usm);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public ActionResult Create(UserSignUpModel USM)
        {
            var login = (BKBTitas.Models.ViewModel.LoginModels)Session["LoginCredentials"];
            int rolP = Convert.ToInt32(login.role_priority.ToString());
            LoginUser LU = new LoginUser();

            if (login != null)
            {
                try
                {
                    LU.userID = USM.userID;
                    LU.userName = USM.userName;
                    LU.userPass = PasswordManager.encryption(USM.userID+USM.userPass).ToString();
                    LU.branch_code = USM.branch_code;
                    LU.userRole = USM.userRole;
                    LU.user_status = USM.user_status;
                    LU.UserMobile = USM.UserMobile;
                    LU.first_login = "Y";
                    LU.makerID = login.UserId;
                    LU.makerTime = DateTime.Now;

                    db.LoginUsers.Add(LU);

                    db.SaveChanges();

                    TempData["retMsg"] = "User created successfully.";
                    return RedirectToAction("Index", "Account");
                }
                catch(Exception ex)
                {
                    TempData["retMsg"] = "Error Occured!";
                    return RedirectToAction("Index", "Account");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        
        public ActionResult Edit(string userID)
        {
            LoginModels logIn = new LoginModels();
            var login = (BKBTitas.Models.ViewModel.LoginModels)Session["LoginCredentials"];
            
            UserSignUpModel usm = new UserSignUpModel();
            if (login != null)
            {
                int rolP = Convert.ToInt32(login.role_priority);
                try
                {
                    var userDet = from x in db.LoginUsers where x.userID==userID
                                  select new UserSignUpModel
                                  {
                                      userID = x.userID,
                                      userName = x.userName,
                                      userRole = x.userRole,
                                      branch_code = x.branch_code,
                                      user_status = x.user_status
                                  };

                    usm = userDet.SingleOrDefault();

                    var queryRole = from q in db.Roles
                                    where q.role_priority > rolP
                                    select new
                                    {
                                        value = q.id,
                                        description = q.role_name
                                    };
                    usm.roleList = new SelectList(queryRole.ToList(), "value", "description");

                    var query = from q in db.Branches
                                select new
                                {
                                    value = q.branch_code,
                                    description = q.branch_code + "--" + q.branch_name
                                };

                    if (login.UserRoleId != 1)
                    {
                        query = query.Where(o => o.value.Equals(login.BranchCode));
                    }

                    usm.BranchList = new SelectList(query.ToList(), "value", "description");

                    return View(usm);
                }
                catch (Exception ex)
                {
                    TempData["retMsg"] = "Error! While loading data";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public ActionResult Edit(UserSignUpModel usm)
        {
            var login = (BKBTitas.Models.ViewModel.LoginModels)Session["LoginCredentials"];
            bool txnStatus = true;
            var txn = db.Database.BeginTransaction();

            if (login != null)
            {
                try
                {
                    LoginUser LU = db.LoginUsers.Find(usm.userID);

                    LU.userName = usm.userName;
                    LU.branch_code = usm.branch_code;
                    LU.userRole = usm.userRole;
                    LU.user_status = usm.user_status;
                    LU.UserMobile = usm.UserMobile;
                    LU.updatedBY = login.UserId;
                    LU.updateTime = DateTime.Now;
             
                    db.SaveChanges();
                    TempData["retMsg"] = usm.userID+ " Updated successfully!";
                    return RedirectToAction("Index", "Account");
                }
                catch (Exception ex)
                {
                    txnStatus = false;
                    TempData["retMsg"] = "Error occured!";
                    
                    return RedirectToAction("Index", "Account");
                }
                finally
                {
                    if (txnStatus == true)
                    {
                        txn.Commit();
                    }
                    else
                    {
                        txn.Rollback();
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult ChangePassword()
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
        public ActionResult ChangePassword(PasswordChangeModel pcm)
        {
            var _login = (BKBTitas.Models.ViewModel.LoginModels)Session["LoginCredentials"];
            string userID = _login.UserId.ToString();
            try
            {
                string password = PasswordManager.encryption(_login.UserId.Trim() + pcm.OldPassword).ToString();
                var isExist = db.LoginUsers.Where(x => x.userID.Trim().ToLower() == _login.UserId.Trim().ToLower() && x.userPass.Equals(password)).SingleOrDefault();
                if (isExist != null)
                {
                    LoginUser lu = db.LoginUsers.Find(userID);

                    lu.userPass = PasswordManager.encryption(lu.userID + pcm.NewPassword); 
                    lu.first_login = "N";
                    db.SaveChanges();
                    TempData["retMsg"] = "Password has been changed successfully.";
                    return View();
                }else
                {
                    TempData["retMsg"] = "Old password is incorrect.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["retMsg"] = "Unknown Error! Please try again.";
                return RedirectToAction("ChangePassword", "Account");
            }
        }

        public ActionResult ResetPassword()
        {
            var login = (BKBTitas.Models.ViewModel.LoginModels)Session["LoginCredentials"];
            if (login != null)
            {
                PasswordResetModel prm = new PasswordResetModel();
                var query = db.LoginUsers
                        .Select(x =>
                                new
                                {
                                    value = x.userID,
                                    description = x.userID+"-"+x.userName
                                });
                prm.userList = new SelectList(query.ToList(), "value", "description");
                return View(prm);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public ActionResult ResetPassword(PasswordResetModel prm)
        {
            var _login = (BKBTitas.Models.ViewModel.LoginModels)Session["LoginCredentials"];
            string userID = _login.UserId.ToString();
            try
            {
                if(prm.NewPassword != prm.ConfirmPassword)
                {
                    TempData["retMsg"] = "New Password and Confirm Password is not Same.";
                    return RedirectToAction("ResetPassword","Account");
                }
                //string password = PasswordManager.encryption(prm.userID.Trim() + prm.NewPassword).ToString();
                var isExist = db.LoginUsers.Where(x => x.userID.Trim().ToLower() == prm.userID.Trim().ToLower()).SingleOrDefault();
                if (isExist != null)
                {
                    LoginUser lu = db.LoginUsers.Find(prm.userID);

                    lu.userPass = PasswordManager.encryption(prm.userID + prm.NewPassword);
                    lu.first_login = "Y";
                    db.SaveChanges();
                    TempData["retMsg"] = "Password has been changed successfully.";
                    return RedirectToAction("ResetPassword", "Account");
                }
                else
                {
                    TempData["retMsg"] = "Old password is incorrect.";
                    return RedirectToAction("ResetPassword", "Account");
                }
            }
            catch (Exception ex)
            {
                TempData["retMsg"] = "Unknown Error! Please try again.";
                return RedirectToAction("ResetPassword", "Account");
            }
        }
        public ActionResult FirstLogin()
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
        public ActionResult FirstLogin(PasswordChangeModel pcm)
        {
            var login = (BKBTitas.Models.ViewModel.LoginModels)Session["LoginCredentials"];
            string userID = login.UserId.ToString();
            try
            {
                LoginUser lu = db.LoginUsers.Find(userID);

                lu.userPass = PasswordManager.encryption(lu.userID.Trim() + pcm.NewPassword).ToString(); 
                lu.first_login = "N";
                db.SaveChanges();

                return RedirectToAction("Index", "Login");
            }
            catch(Exception ex)
            {
                Console.Write("Error " + ex);
                return View("Index", "Login");
            }
        }
    }
}