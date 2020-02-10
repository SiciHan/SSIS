using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Team8ADProjectSSIS.EmailModel;
using Team8ADProjectSSIS.DAO;
using Team8ADProjectSSIS.Models;
using Team8ADProjectSSIS.Report;
using Team8ADProjectSSIS.Filters;
using Microsoft.AspNet.SignalR;
using Team8ADProjectSSIS.Hubs;
using System.Data.SqlClient;
using Team8ADProjectSSIS.DataPoints;
using Newtonsoft.Json;

namespace Team8ADProjectSSIS.Controllers
{

    [AuthenticateFilter]
    [AuthorizeFilter]
      
    public class StoreSupervisorController : Controller
    {
        private readonly StockRecordDAO _stockRecordDAO;
        private readonly ItemDAO _itemDAO;
        private readonly PurchaseOrderDAO _purchaseOrderDAO;
        private readonly PurchaseOrderDetailsDAO _purchaseOrderDetailsDAO;
        private readonly NotificationChannelDAO _notificationChannelDAO;
        private readonly CategoryDAO _categoryDAO;
        private readonly DepartmentDAO _departmentDAO;
        public StoreSupervisorController()
        {
            this._stockRecordDAO = new StockRecordDAO();
            this._itemDAO = new ItemDAO();
            this._purchaseOrderDAO = new PurchaseOrderDAO();
            this._purchaseOrderDetailsDAO = new PurchaseOrderDetailsDAO();
            _notificationChannelDAO = new NotificationChannelDAO();
            _categoryDAO = new CategoryDAO();
            _departmentDAO = new DepartmentDAO();
        }
          
        public ActionResult Notification()
        {
            int IdReceiver = 1;
            if (Session["IdEmployee"] != null)
            {
                IdReceiver = (int)Session["IdEmployee"];
            }
            ViewData["NCs"] = _notificationChannelDAO.FindAllNotificationsByIdReceiver(IdReceiver);

            return View();
        }
          
        public ActionResult Voucher()
        {
            List<StockRecord> vouchers = _stockRecordDAO.FindVoucherForSupervisor();
            List<float> prices = new List<float>();
            foreach (StockRecord voucher in vouchers)
            {
                float price = _itemDAO.FindPriceById(voucher.IdItem);
                prices.Add(price);
            }
            ViewData["prices"] = prices;
            ViewData["vouchers"] = vouchers;
            return View();
        }
          
        public ActionResult VoucherHistory()
        {
            List<StockRecord> vouchers = _stockRecordDAO.FindJudgedVoucherForSupervisor();
            List<float> prices = new List<float>();
            List<string> status = new List<string>();
            foreach (StockRecord voucher in vouchers)
            {
                float price = _itemDAO.FindPriceById(voucher.IdItem);
                prices.Add(price);
                if (voucher.IdOperation == 7 || voucher.IdOperation == 9 || voucher.IdOperation == 12 || voucher.IdOperation == 14)
                {
                    status.Add("Approved");
                }
                else
                {
                    status.Add("Rejected");
                }
            }
            ViewData["prices"] = prices;
            ViewData["vouchers"] = vouchers;
            ViewData["status"] = status;
            return View();
        }
          
        public ActionResult PurchaseOrder()
        {
            List<PurchaseOrder> pendingPOs = _purchaseOrderDAO.FindPendingPO();
            ViewData["pengding"] = pendingPOs;
            return View();
        }
          
        public ActionResult POHistory()
        {
            List<PurchaseOrder> handledPOs = _purchaseOrderDAO.FindHandledPO();
            ViewData["handledPOs"] = handledPOs;
            return View();
        }
          
        public ActionResult PurchaseOrderDetail(int idPurchaseOrder)
        {
            List<PurchaseOrderDetail> PODetails = _purchaseOrderDetailsDAO.FindDetailPO(idPurchaseOrder);
            PurchaseOrder po = _purchaseOrderDAO.FindPOById(idPurchaseOrder);
            ViewData["POD"] = PODetails;
            ViewBag.po = po;
            return View();
        }
          
        public ActionResult DashBoard(string category = "", string department = "")
        {
            List<DateTime> times = new List<DateTime>();
            List<int> amounts_groupbyCategory = new List<int>();
            List<int> amounts_groupbyDepartment = new List<int>();
            DateTime today = DateTime.Now;

            for (int i = 90; i >= 1; i -= 7)
            {
                DateTime temp = today.AddDays(-i);
                times.Add(temp);
            }
            using (SqlConnection conn = new SqlConnection(("Server=.; Database=SSIS; Integrated Security=true")))
            {
                conn.Open();
                foreach (DateTime time in times)
                {
                    DateTime time_nextweek = time.AddDays(8);
                    string sql_groupbyCategory;
                    string sql_groupbyDepartment;
                    if (category == "" || category == "Total")
                    {
                        sql_groupbyCategory = @"SELECT isNull(SUM(OrderUnit), 0) AS unit
                            FROM PurchaseOrderDetails pod
                            INNER JOIN PurchaseOrders po ON pod.IdPurchaseOrder = po.IdPurchaseOrder
                            WHERE po.DeliverDate BETWEEN '" + time + "' AND '" + time_nextweek + "'";
                    }
                    else
                    {
                        sql_groupbyCategory = @"SELECT isNull(SUM(OrderUnit), 0) AS unit
                            FROM PurchaseOrderDetails pod
                            INNER JOIN PurchaseOrders po ON pod.IdPurchaseOrder = po.IdPurchaseOrder
                            INNER JOIN Items i ON pod.IdItem = i.IdItem
                            INNER JOIN Categories c ON i.IdCategory = c.IdCategory
                            WHERE po.DeliverDate BETWEEN '" + time + "' AND '" + time_nextweek + "'" +
                            " AND c.Label = '" + category + "'";
                    }
                    if (department == "" || department == "Total")
                    {
                        sql_groupbyDepartment = @"SELECT isNull(SUM(Unit), 0) AS unit
                            FROM RequisitionItems ri INNER JOIN Requisitions r
                            ON ri.IdRequisiton = r.IdRequisition
                            INNER JOIN Employees emp
                            ON emp.IdEmployee = r.IdEmployee
                            WHERE r.RaiseDate BETWEEN '" + time + "' AND '" + time_nextweek + "'";
                    }
                    else
                    {
                        sql_groupbyDepartment = @"SELECT isNull(SUM(Unit), 0) AS unit
                            FROM RequisitionItems ri INNER JOIN Requisitions r
                            ON ri.IdRequisiton = r.IdRequisition
                            INNER JOIN Employees emp
                            ON emp.IdEmployee = r.IdEmployee
                            WHERE r.RaiseDate BETWEEN '" + time + "' AND '" + time_nextweek + "'" +
                            " AND emp.CodeDepartment = '" + department + "';";
                    }
                    SqlCommand cmd1 = new SqlCommand(sql_groupbyCategory, conn);
                    SqlCommand cmd2 = new SqlCommand(sql_groupbyDepartment, conn);
                    SqlDataReader reader1 = cmd1.ExecuteReader();

                    if (reader1.HasRows)
                    {
                        while (reader1.Read())
                        {
                            if (reader1["unit"] != null)
                            {
                                int t = (int)reader1["unit"];
                                amounts_groupbyCategory.Add(t);
                            }
                            else amounts_groupbyCategory.Add(0);
                        }
                    }
                    else
                    {
                        amounts_groupbyCategory.Add(0);
                    }
                    reader1.Close();
                    SqlDataReader reader2 = cmd2.ExecuteReader();
                    if (reader2.HasRows)
                    {
                        if (reader2.Read())
                        {
                            if (reader2["unit"] != null)
                            {
                                int t = (int)reader2["unit"];
                                amounts_groupbyDepartment.Add(t);
                            }
                            else amounts_groupbyDepartment.Add(0);
                        }

                    }
                    else
                    {
                        amounts_groupbyDepartment.Add(0);
                    }
                    reader2.Close();

                }
            }
            List<DataPoint> dataPoints1 = new List<DataPoint>();
            List<DataPoint> dataPoints2 = new List<DataPoint>();
            foreach (DateTime time in times)
            {
                DataPoint d = new DataPoint();
                string tt = time.ToString("dd-MM-yyyy");
                d.x = tt;
                dataPoints1.Add(d);
                dataPoints2.Add(d);
            }
            int j = 0;
            foreach (int amount in amounts_groupbyCategory)
            {
                dataPoints1[j].y = amount;
                j++;

            }
            int k = 0;
            foreach (int amount in amounts_groupbyDepartment)
            {
                dataPoints2[k].y = amount;
                k++;

            }
            ViewBag.categories = _categoryDAO.FindAllCategories();
            ViewBag.departments = _departmentDAO.FindAllDepartments();
            ViewBag.categorySelected = category == "" ? "Total" : category;
            ViewBag.departmentSelected = department == "" ? "Total" : department;
            JsonSerializerSettings _jsonSetting = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
            ViewBag.DataPoints1 = JsonConvert.SerializeObject(dataPoints1, _jsonSetting);
            ViewBag.DataPoints2 = JsonConvert.SerializeObject(dataPoints2, _jsonSetting);
            return View();
        }
          
        public ActionResult ExportExcel()
        {

            List<Item> DownloadableData = _itemDAO.GetDownloadableData();
            ExcelReport excelReport = new ExcelReport();
            byte[] ExcelData = excelReport.GenerateExcelReport(DownloadableData);

            return File(ExcelData, "application/xlsx", "Ordered Data.xlsx");
        }
          
        public ActionResult PrintPDF()
        {
            return View();
        }
          
        [HttpPost]
        public ActionResult HandlePO(string handle, List<int> purchase_ordersId, string remarks)
        {
            List<PurchaseOrder> purchaseOrders = new List<PurchaseOrder>();
            foreach (int id in purchase_ordersId)
            {
                PurchaseOrder po = _purchaseOrderDAO.FindPOById(id);
                purchaseOrders.Add(po);
            }
            if (handle == "Approve")
            {
                _purchaseOrderDAO.UpdatePOToApproved(purchaseOrders);

                foreach (PurchaseOrder po in purchaseOrders)
                {
                    var hub = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                    hub.Clients.All.receiveNotification(po.IdStoreClerk);
                    EmailClass emailClass = new EmailClass();
                    string message = "Hi," + po.StoreClerk.Name + " your purchase order made with " + po.Supplier.Name + " ordered on " + po.OrderDate + " has been approved";
                    _notificationChannelDAO.CreateNotificationsToIndividual(po.StoreClerk.IdEmployee, (int)Session["IdEmployee"], message);
                    emailClass.SendTo(po.StoreClerk.Email, "SSIS System Email", message);
                }   
            }
            else
            {
                _purchaseOrderDAO.UpdatePOToRejected(purchaseOrders, remarks);

                foreach (PurchaseOrder po in purchaseOrders)
                {
                    var hub = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                    hub.Clients.All.receiveNotification(po.IdStoreClerk);
                    EmailClass emailClass = new EmailClass();
                    string message = "Hi," + po.StoreClerk.Name + " your purchase order made with " + po.Supplier.Name + " ordered on " + po.OrderDate + " has been reject +/n" +
                        "Reasons: "+po.PurchaseRemarks;
                    _notificationChannelDAO.CreateNotificationsToIndividual(po.StoreClerk.IdEmployee, (int)Session["IdEmployee"], message);
                    emailClass.SendTo(po.StoreClerk.Email, "SSIS System Email", message);
                }
            }
            return RedirectToAction("PurchaseOrder", "StoreSupervisor");
        }
          
        [HttpPost]
        public ActionResult Handlejustment(string handle, List<int> vouchersId)
        {
            List<StockRecord> vouchers = new List<StockRecord>();
            foreach (int id in vouchersId)
            {
                StockRecord voucher = _stockRecordDAO.FindById(id);
                vouchers.Add(voucher);
            }
            if (handle == "Approve")
            {
                _stockRecordDAO.UpdateVoucherToApproved(vouchers);
                foreach (StockRecord sr in vouchers)
                {
                    var hub = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                    hub.Clients.All.receiveNotification(sr.IdStoreClerk);
                    EmailClass emailClass = new EmailClass();
                    string message = "Hi," + sr.StoreClerk.Name + " your stock adjustment voucher for (" + sr.Operation.Label.Split('-')[1] + ") " + sr.Unit + " " + sr.Item.unitOfMeasure + sr.Item.Description + " raised on " + sr.Date + " has been approved.";
  
                    _notificationChannelDAO.CreateNotificationsToIndividual(sr.StoreClerk.IdEmployee, (int)Session["IdEmployee"], message);
                    emailClass.SendTo(sr.StoreClerk.Email, "SSIS System Email", message);
                }
            }
            else
            {
                _stockRecordDAO.UpdateVoucherToRejected(vouchers);

                foreach (StockRecord sr in vouchers)
                {
                    var hub = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                    hub.Clients.All.receiveNotification(sr.IdStoreClerk);
                    EmailClass emailClass = new EmailClass();
                    string message = "Hi," + sr.StoreClerk.Name + " your stock adjustment voucher for (" + sr.Operation.Label.Split('-')[1] + ") " + sr.Unit + " " + sr.Item.unitOfMeasure + sr.Item.Description + " raised on " + sr.Date + " has been rejected.";
                    _notificationChannelDAO.CreateNotificationsToIndividual(sr.StoreClerk.IdEmployee, (int)Session["IdEmployee"], message);
                    emailClass.SendTo(sr.StoreClerk.Email, "SSIS System Email", message);
                }
            }
            return RedirectToAction("Voucher", "StoreSupervisor");
        }
    }
}