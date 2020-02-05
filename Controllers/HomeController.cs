using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Team8ADProjectSSIS.DAO;
using Team8ADProjectSSIS.Models;

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
/*        public HomeController(CategoryDAO categoryDAO)
        {
            _categoryDAO = categoryDAO;
        }*/

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
                            return RedirectToAction("Dashboard", "DepartmentHead");
                        case "Representative":
                            return RedirectToAction("Home", "DepartmentRepresentative");
                        case "StockClerk":
                            return RedirectToAction("Index", "StoreClerk");
                        case "StockManager":
                            return RedirectToAction("Home", "StoreManager");
                        case "StockSupervisor":
                            return RedirectToAction("Dashboard", "StoreSupervisor");
                        case "ActingHead":
                            return RedirectToAction("Dashboard", "DepartmentActingHead");
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
            return Content("You are logged out. Please try to log in again.");
        }
        public ActionResult SessionExpired()
        {
            return Content("Your session is expired. Please try to log in again.");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            Category c = new Category();
            c.Label = "pen";
            _categoryDAO.Create(c);
           // DAO method
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            using (SSISContext context = new SSISContext())
            {
                // Context method
                Role c = new Role();
                c.Label = "Manager";
                context.Roles.Add(c);
                context.SaveChanges();
            }
            return View();
        }

        public JsonResult GetNotifications()
        {
            int IdReceiver = (int) Session["IdEmployee"];
            //int IdReceiver = 1;
            return Json(_notificationChannelDAO.FindAllNotificationsByIdReceiver(IdReceiver), JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateNotificationsToGroup(string role,string message)
        {
            int IdSender= (int)Session["IdEmployee"];
            //int IdSender = 2;
            _notificationChannelDAO.CreateNotificationsToGroup(role,IdSender,message);
            string status = "OK";
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateNotificationsToIndividual(int idReceiver, string message)
        {
            int IdSender = (int)Session["IdEmployee"];
            //int IdSender = 2;
            _notificationChannelDAO.CreateNotificationsToIndividual(idReceiver, IdSender, message);
            string status = "OK";
            return Json(status, JsonRequestBehavior.AllowGet);
        }

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

        public JsonResult GetUnreadNotificationCount(int IdReceiver)
        {
            
            int count = _notificationChannelDAO.GetUnreadNotificationCount(IdReceiver);
            return Json(count, JsonRequestBehavior.AllowGet);
        }
        
    }
}