using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Team8ADProjectSSIS.EmailModel;
using Team8ADProjectSSIS.DAO;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.Controllers
{
    public class StoreManagerController : Controller
    {

        // GET: StoreManager
        public ActionResult Home()
        {
            
            return View();
        }

        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult ItemsForSuppliers()
        {
            List<Item> items = ItemDAO.GetAllItems();
            ViewBag.items = items;
            return View();
        }

        public ActionResult Suppliers(int itemId)
        {
            ViewBag.id = itemId;
            return View();
        }
    }
}