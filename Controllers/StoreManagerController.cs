using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Team8ADProjectSSIS.EmailModel;

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
    }
}