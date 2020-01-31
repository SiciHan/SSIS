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

        public HomeController()
        {
            _categoryDAO = new CategoryDAO();
            _employeeDAO = new EmployeeDAO();
            _roleDAO = new RoleDAO();
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
        public ActionResult LogIn(string UserName, string HashedPassword)
        {
            //Try to find the user from user name
            Employee user = _employeeDAO.FindEmployeeByUsername(UserName);
            //if the user exist
            if (user != null)
            {
                //get the hashed string of the input password
                SHA1CryptoServiceProvider sh = new SHA1CryptoServiceProvider();
                sh.ComputeHash(Encoding.ASCII.GetBytes(HashedPassword));
                byte[] re = sh.Hash;
                StringBuilder sb = new StringBuilder();
                foreach (byte b in re)
                {
                    sb.Append(b.ToString("x2"));//hexidecimal string of 2 chars
                }
                //compare the input password to actual password, if matched
                if (user.HashedPassward.Equals(sb.ToString()))
                {
                    //set the user session
                    Session["sessionId"] = Guid.NewGuid();//setting user session
                    Session["IdEmployee"] = user.IdEmployee;
                    Session["Role"] = user.IdRole;

                    //redirect to different pages based on the roles
                    switch (_roleDAO.FindRoleLabelById(user.IdRole))
                    {
                        case "Employee":
                            return RedirectToAction("Dashboard", "Employee");
                        case "Head":
                            return RedirectToAction("Dashboard", "DepartmentHead");
                        case "Representative":
                            return RedirectToAction("Dashboard", "DepartmentRepresentative");
                        case "StoreClerk":
                            return RedirectToAction("Dashboard", "StoreClerk");
                        case "StoreManager":
                            return RedirectToAction("Dashboard", "StoreManager");
                        case "StoreSupervisor":
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
    }
}