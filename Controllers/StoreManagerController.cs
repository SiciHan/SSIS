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
using System.Diagnostics;
using Newtonsoft.Json;
using Team8ADProjectSSIS.DataPoints;

namespace Team8ADProjectSSIS.Controllers
{

    [AuthorizeFilter]
    [AuthenticateFilter]
    public class StoreManagerController : Controller
    {
        private readonly ItemDAO _itemDAO;
        private readonly SupplierItemDAO _supplieritemDAO;
        private readonly PurchaseOrderDAO _purchaseOrderDAO;
        private readonly PurchaseOrderDetailsDAO _purchaseOrderDetailsDAO;
        private readonly DisbursementDAO _disbursementDAO;
        private readonly DisbursementItemDAO _disbursementItemDAO;
        private readonly StockRecordDAO _stockRecordDAO;
        private readonly NotificationChannelDAO _notificationChannelDAO;
        private readonly RequisitionDAO _requisitionDAO;
        private readonly RequisitionItemDAO _requisitionItemDAO;
        private readonly CategoryDAO _categoryDAO;
        private readonly DepartmentDAO _departmentDAO;

        public StoreManagerController()
        {
            _itemDAO = new ItemDAO();
            _supplieritemDAO = new SupplierItemDAO();
            _purchaseOrderDAO = new PurchaseOrderDAO();
            _purchaseOrderDetailsDAO = new PurchaseOrderDetailsDAO();
            _disbursementDAO = new DisbursementDAO();
            _disbursementItemDAO = new DisbursementItemDAO();
            _stockRecordDAO = new StockRecordDAO();
            _notificationChannelDAO = new NotificationChannelDAO();
            _requisitionDAO = new RequisitionDAO();
            _requisitionItemDAO = new RequisitionItemDAO();
            _categoryDAO = new CategoryDAO();
            _departmentDAO = new DepartmentDAO();
        }

        // GET: StoreManager
        public ActionResult Home()
        {
            return View();
        }

        //@Shutong
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

        //Huang Yuzhe
        public ActionResult Dashboard(string category="", string department="")
        {
            List<DateTime> times = new List<DateTime>();
            List<int> amounts_groupbyCategory = new List<int>();
            List<int> amounts_groupbyDepartment = new List<int>();
            DateTime today = DateTime.Now;

            for(int i=90; i>=1; i-=7)
            {
                DateTime temp = today.AddDays(-i);
                times.Add(temp);
            }
            using (SqlConnection conn = new SqlConnection(("Server=.; Database=SSIS; Integrated Security=true")))
            {
                conn.Open();
                foreach(DateTime time in times)
                {
                    DateTime time_nextweek = time.AddDays(8);
                    string sql_groupbyCategory;
                    string sql_groupbyDepartment;
                    if (category == "" || category == "Total")
                    {
                        sql_groupbyCategory = @"SELECT isNull(SUM(Unit), 0) AS unit
                            FROM StockRecords sr
                            WHERE sr.Date BETWEEN '" + time + "' AND '" + time_nextweek + "'" + 
                            " AND sr.IdOperation = '2'";
                    }
                    else
                    {
                        sql_groupbyCategory = @"SELECT isNull(SUM(Unit), 0) AS unit
                            FROM StockRecords sr
                            INNER JOIN Items i ON sr.IdItem = i.IdItem
                            INNER JOIN Categories c ON i.IdCategory = c.IdCategory
                            WHERE sr.Date BETWEEN '" + time + "' AND '" + time_nextweek + "'" + 
                            " AND sr.IdOperation = '2'" + 
                            " AND c.Label = '" + category + "'";
                    }
                    if(department == "" || department == "Total")
                    {
                        sql_groupbyDepartment = @"SELECT isNull(SUM(Unit), 0) AS unit
                            FROM StockRecords sr
                            WHERE sr.Date BETWEEN '" + time + "' AND '" + time_nextweek + "'" + 
                            " AND sr.IdOperation = '1'";
                    }
                    else
                    {
                        sql_groupbyDepartment = @"SELECT isNull(SUM(Unit), 0) AS unit
                            FROM StockRecords sr
                            WHERE sr.Date BETWEEN '" + time + "' AND '" + time_nextweek + "'" +
                            " AND sr.IdOperation = '1'" + 
                            " AND sr.IdDepartment = '" + department + "'" ;
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
                        while (reader2.Read())
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
            foreach(DateTime time in times)
            {
                DataPoint d1 = new DataPoint();
                DataPoint d2 = new DataPoint();
                string tt = time.ToString("dd-MM-yyyy");
                d1.x = tt;
                d2.x = tt;
                dataPoints1.Add(d1);
                dataPoints2.Add(d2);
            }
            int j = 0;
            foreach(int amount in amounts_groupbyCategory)
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
            ViewBag.categorySelected = category == ""?"Total" : category;
            ViewBag.departmentSelected = department == ""?"Total" : department;
            JsonSerializerSettings _jsonSetting = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
            ViewBag.DataPoints2 = JsonConvert.SerializeObject(dataPoints2, _jsonSetting);
            ViewBag.DataPoints1 = JsonConvert.SerializeObject(dataPoints1, _jsonSetting);
            return View();
        }

        //Huang Yuzhe
        public ActionResult ItemsForSuppliers()
        {
            List<Item> items = _itemDAO.GetAllItems();
            ViewBag.items = items;
            return View();
        }

        //Huang Yuzhe
        public ActionResult Suppliers(int itemId)
        {
            List<SupplierItem> supplierItems = _supplieritemDAO.GetSuppliersById(itemId);
            List<Supplier> suppliers = new List<Supplier>();
            foreach(SupplierItem item in supplierItems)
            {
                Supplier temp = item.Supplier;
                suppliers.Add(temp);
            }
            ViewBag.suppliers = suppliers;
            return View();
        }

        // Willis
        public ActionResult POHistory()
        {
            List<PurchaseOrder> AllPO = _purchaseOrderDAO.FindAllPO();
            ViewData["AllPO"] = AllPO;
            return View();
        }

        // Willis
        public ActionResult PODetails(int IdPurchaseOrder)
        {
            List<PurchaseOrderDetail> DetailPO = _purchaseOrderDetailsDAO.FindDetailPO(IdPurchaseOrder);
            ViewData["IdPurchaseOrder"] = IdPurchaseOrder;
            ViewData["DetailPO"] = DetailPO;
            return View();
        }

        //Willis
        public ActionResult DisbursementHistory()
        {
            List<Disbursement> AllDisbursement = _disbursementDAO.GetAllDisbursements();
            ViewData["AllDisbursement"] = AllDisbursement;
            return View();
        }

        //Willis
        public ActionResult DisbursementDetails(int IdDisbursement)
        {
            List<DisbursementItem> DetailDisbursement = _disbursementItemDAO.FindDetailDisbursement(IdDisbursement);
            ViewData["DetailDisbursement"] = DetailDisbursement;
            ViewData["IdDisbursement"] = IdDisbursement;
            return View();
        }

        //Huang Yuzhe
        public ActionResult Voucher()
        {
            List<StockRecord> vouchers = _stockRecordDAO.FindVoucher();
            List<float> prices = new List<float>(); 
            foreach(StockRecord voucher in vouchers)
            {
                float price = _itemDAO.FindPriceById(voucher.IdItem);
                prices.Add(price);
            }
            ViewData["prices"] = prices;
            ViewData["vouchers"] = vouchers;
            return View();
        }

        //Huang Yuzhe
        public ActionResult VoucherHistory()
        {
            List<StockRecord> vouchers = _stockRecordDAO.FindJudgedVoucher();
            List<float> prices = new List<float>();
            List<string> status = new List<string>();
            foreach (StockRecord voucher in vouchers)
            {
                float price = _itemDAO.FindPriceById(voucher.IdItem);
                prices.Add(price);
                if(voucher.IdOperation == 7 || voucher.IdOperation == 9 || voucher.IdOperation == 12 || voucher.IdOperation == 14)
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

        //Huang Yuzhe
        [HttpPost]
        public ActionResult JudgeAdjustment(string judge, List<int> vouchersId)
        {
            List<StockRecord> vouchers = new List<StockRecord>();
            foreach(int id in vouchersId)
            {
                StockRecord voucher = _stockRecordDAO.FindById(id);
                vouchers.Add(voucher);
            }
            if(judge == "Approve")
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
            return RedirectToAction("Voucher", "StoreManager");
        }
        // Willis
        [HttpPost]
        public ActionResult ExportExcel()
        {

            List<Item> DownloadableData = _itemDAO.GetDownloadableData();
            ExcelReport excelReport = new ExcelReport();
            byte[] ExcelData = excelReport.GenerateExcelReport(DownloadableData);

            return File(ExcelData, "application/xlsx", "Ordered Data.xlsx");
        }
        // Willis
        public ActionResult RequisitionHistory()
        {
            List<Requisition> AllRequisition = _requisitionDAO.FindAllRequisition();
            ViewData["AllRequisition"] = AllRequisition;
            return View();
        }
        // Willis
        public ActionResult RequisitionDetails(int IdRequisition)
        {
            List<RequisitionItem> RequisitionDetails = _requisitionItemDAO.RetrieveRequisitionItemByReqId(IdRequisition);
            ViewData["RequisitionDetails"] = RequisitionDetails;
            ViewData["IdRequisition"] = IdRequisition;
            return View();
        }
    }
}