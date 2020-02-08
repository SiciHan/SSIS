/*
 Author: Shutong
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Team8ADProjectSSIS.DAO;
using Team8ADProjectSSIS.Filters;
using Team8ADProjectSSIS.Models;
using Team8ADProjectSSIS.EmailModel;

namespace Team8ADProjectSSIS.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly CategoryDAO _categoryDAO;
        private readonly EmployeeDAO _employeeDAO;
        private readonly RoleDAO _roleDAO;
        private readonly NotificationChannelDAO _notificationChannelDAO;
        public HomeController()
        {
            _categoryDAO = new CategoryDAO();
            _employeeDAO = new EmployeeDAO();
            _roleDAO = new RoleDAO();
            _notificationChannelDAO = new NotificationChannelDAO();
        }
        public ActionResult Chat()
        {
            return View();
        }
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult LogIn()
        {
            Session.Clear();
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(string UserName, string HashedPassward)
        {
            //Try to find the user from user name
            Employee user = _employeeDAO.FindEmployeeByUsername(UserName);
            //if the user exist
            if (user != null)
            {
                //get the hashed string of the input password
                SHA1 sh = new SHA1CryptoServiceProvider();
                sh.ComputeHash(Encoding.ASCII.GetBytes(HashedPassward));
                byte[] re = sh.Hash;
                StringBuilder sb = new StringBuilder();
                foreach (byte b in re)
                {
                    sb.Append(b.ToString("x2"));//hexidecimal string of 2 chars
                }
                Console.WriteLine(sb.ToString());//5baa61e4c9b93f3f0682250b6cf8331b7ee68fd8
                //compare the input password to actual password, if matched
                if (user.HashedPassward.Equals(sb.ToString()))
                {
                    //set the user session
                    Session["sessionId"] = Guid.NewGuid();//setting user session
                    Session["IdEmployee"] = user.IdEmployee;
                    Session["Role"] = _roleDAO.FindRoleLabelById(user.IdRole);

                    //redirect to different pages based on the roles
                    switch (_roleDAO.FindRoleLabelById(user.IdRole))
                    {
                        case "Employee":
                            return RedirectToAction("Index", "Employee");
                        case "Head":
                            return RedirectToAction("Notification", "DepartmentHead");
                        case "Representative":
                            return RedirectToAction("Home", "DepartmentRepresentative");
                        case "StockClerk":
                            return RedirectToAction("Index", "StoreClerk");
                        case "StockManager":
                            return RedirectToAction("Home", "StoreManager");
                        case "StockSupervisor":
                            return RedirectToAction("Dashboard", "StoreSupervisor");
                        case "ActingHead":
                            return RedirectToAction("Notification", "DepartmentActingHead");
                        default:
                            return RedirectToAction("Index", "Home");
                    }
                }
            }
            //if the password does not match or user does not exist
            ModelState.AddModelError("", "User name or passward is invalid.");
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return View();
        }
        public ActionResult SessionExpired()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        [AuthenticateFilter]
        public JsonResult GetNotifications()
        {
            int IdReceiver = (int) Session["IdEmployee"];
            //int IdReceiver = 1;
            return Json(_notificationChannelDAO.FindAllNotificationsByIdReceiver(IdReceiver), JsonRequestBehavior.AllowGet);
        }

        [AuthenticateFilter]
        public JsonResult CreateNotificationsToGroup(string role,string message)
        {
            int IdSender= (int)Session["IdEmployee"];
            //int IdSender = 2;
            _notificationChannelDAO.CreateNotificationsToGroup(role,IdSender,message);
            string status = "OK";
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SendEmailToGroup(string role, string message)
        {
            List<string> emails=_employeeDAO.FindEmailsByRole(role);
            string status="Ok";
            try
            {
                foreach(string email in emails)
                {
                    EmailClass emailClass = new EmailClass();
                    emailClass.SendTo(email, "SSIS System Email", message);
                }
            }
            catch (Exception)
            {
                status = "Bad";
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        [AuthenticateFilter]
        public JsonResult CreateNotificationsToIndividual(int idReceiver, string message)
        {
            int IdSender = (int)Session["IdEmployee"];
            //int IdSender = 2;
            _notificationChannelDAO.CreateNotificationsToIndividual(idReceiver, IdSender, message);
            string status = "OK";
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        [AuthenticateFilter]
        public JsonResult MarkNotificationChannelAsRead(int IdNC)
        {
            string status = null;
            NotificationChannel nc= _notificationChannelDAO.MarkAsReadById(IdNC);
            if (nc == null)
            {
                status = "Bad";

            }
            else
            {
                status = "OK";
            }

            return Json(status, JsonRequestBehavior.AllowGet);
        }

        [AuthenticateFilter]
        public JsonResult MarkNotificationChannelAsUnread(int IdNC)
        {

            string status = null;
            NotificationChannel nc=_notificationChannelDAO.MarkAsUnreadById(IdNC);
            if (nc == null)
            {
                status = "Bad";

            }
            else
            {
                status = "OK";
            }
            
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        [AuthenticateFilter]
        public JsonResult GetUnreadNotificationCount(int IdReceiver)
        {
            
            int count = _notificationChannelDAO.GetUnreadNotificationCount(IdReceiver);
            return Json(count, JsonRequestBehavior.AllowGet);
        }


        

    }
}