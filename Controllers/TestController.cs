using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Team8ADProjectSSIS.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Test1()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Test2(string remarks)
        {
            return View();
        }

    }
}