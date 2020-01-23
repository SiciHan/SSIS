using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Team8ADProjectSSIS.DAO;
using Team8ADProjectSSIS.Models;


namespace Team8ADProjectSSIS.Controllers
{
    public class EmployeeController : Controller
    {
        public static string connectionString = "Server=.;" +
              "Database=SSIS; Integrated Security=true;MultipleActiveResultSets=True ";
        // GET: Employee

        public ActionResult Index(string searchStr, string pid)
        {
            int cartCount = 0;
            int prodId;
            if (pid == null)
                prodId = -1;
            else
            {
                prodId = Int32.Parse(pid);
            //    AddToCart(prodId);
            }

          

                return View();
        }

        public List<Item> ListProducts(string searchStr)
        {
            List<Item> products = new List<Item>();

            using (var db = new SSISContext())
            {
                if (searchStr == null)
                {
                   
                    products = db.Items.ToList();
                }
                else
                    products = db.Items.Where(prod => prod.Name.Contains(searchStr) || prod.Description.Contains(searchStr)).ToList();
            }

            Session["searchStr"] = searchStr;
            ViewBag.products = products;
            return products;
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Update()
        {
            return View();
        }

        public ActionResult CheckStatus()
        {
            return View();
        }

        public ActionResult History()
        {
            return View();
        }

        public ActionResult CheckOut()
        {
            return View();
        }
    }
}