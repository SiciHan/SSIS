using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Team8ADProjectSSIS.DAO;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.Controllers
{
    public class StoreClerkController : Controller
    {
        private readonly PurchaseOrderDAO purchaseOrderDAO;

        // GET: StoreClerk
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PurchaseOrderList()
        {
            ViewData["Incomplete"]=purchaseOrderDAO.FindIncompletePO();
            ViewData["Pending"]=purchaseOrderDAO.FindPendingPO();
            ViewData["Rejected"]=purchaseOrderDAO.FindRejectedPO();
            ViewData["Approved"]=purchaseOrderDAO.FindApprovedPO();
            ViewData["Delivered"]=purchaseOrderDAO.FindDeliveredPO();
            ViewData["Cancelled"]=purchaseOrderDAO.FindCancelledPO();
            ViewData["IsStockLow"]=
            return View();
        }
    }
}