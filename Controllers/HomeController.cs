using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Team8ADProjectSSIS.DAO;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.Controllers
{
    public class HomeController : Controller
    {
        private readonly CategoryDAO _categoryDAO;

        public HomeController()
        {
            _categoryDAO = new CategoryDAO();
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