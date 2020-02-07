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
    public class AndroidController : Controller
    {
        private readonly EmployeeDAO _employeeDAO;
        public AndroidController()
        {
            _employeeDAO = new EmployeeDAO();
        }

        // GET: Android
        //here is for mobile parts

        public JsonResult MobileLogin(string username, string password)
        {
            //Try to find the user from user name
            Employee user = _employeeDAO.FindEmployeeByUsername(username);
            //if the user exist
            if (user != null)
            {
                //get the hashed string of the input password
                SHA1 sh = new SHA1CryptoServiceProvider();
                sh.ComputeHash(Encoding.ASCII.GetBytes(password));
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
                    return Json(new { role = user.Role.Label, status = "success", id = user.IdEmployee }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { status = "fail" }, JsonRequestBehavior.AllowGet);
        }
    
    }
}