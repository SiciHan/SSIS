﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using Team8ADProjectSSIS.DAO;
using Team8ADProjectSSIS.EmailModel;
using Team8ADProjectSSIS.Filters;
using Team8ADProjectSSIS.Hubs;
using Team8ADProjectSSIS.Models;
using Team8ADProjectSSIS.Report;

namespace Team8ADProjectSSIS.Controllers
{
    [AuthenticateFilter]
    [AuthorizeFilter]

    public class StoreClerkController : Controller
    {
        private readonly DisbursementDAO _disbursementDAO;
        private readonly RequisitionDAO _requisitionDAO;
        private readonly RequisitionItemDAO _requisitionItemDAO;
        private readonly StockRecordDAO _stockRecordDAO;
        private readonly DisbursementItemDAO _disbursementItemDAO;
        private readonly PurchaseOrderDAO _purchaseOrderDAO;
        private readonly PurchaseOrderDetailsDAO _purchaseOrderDetailsDAO;
        private readonly ItemDAO _itemDAO;
        private readonly CollectionPointDAO _collectionpointDAO;
        private readonly EmployeeDAO _employeeDAO;
        private readonly StatusDAO _statusDAO;
        private readonly NotificationChannelDAO _notificationChannelDAO;
        private readonly NotificationDAO _notificationDAO;
        private readonly SupplierItemDAO _supplierItemDAO;

        //private readonly NotificationChannelDAO _notificationChannelDAO;
        //private readonly EmployeeDAO _employeeDAO;
        public StoreClerkController()
        {
            this._disbursementDAO = new DisbursementDAO();
            this._requisitionDAO = new RequisitionDAO();
            this._requisitionItemDAO = new RequisitionItemDAO();
            this._stockRecordDAO = new StockRecordDAO();
            this._disbursementItemDAO = new DisbursementItemDAO();
            this._purchaseOrderDAO = new PurchaseOrderDAO();
            this._itemDAO = new ItemDAO();
            this._statusDAO = new StatusDAO();
            this._purchaseOrderDetailsDAO = new PurchaseOrderDetailsDAO();
            this._employeeDAO = new EmployeeDAO();
            this._collectionpointDAO = new CollectionPointDAO();
            this._notificationChannelDAO = new NotificationChannelDAO();
            this._notificationDAO = new NotificationDAO();
            this._supplierItemDAO = new SupplierItemDAO();
            //this._notificationChannelDAO = new NotificationChannelDAO();
            //this._employeeDAO = new EmployeeDAO();
        }



        // GET: StoreClerk

        //@Shutong
        public ActionResult Index()
        {
            int IdReceiver = 1;
            if (Session["IdEmployee"] != null)
            {
                IdReceiver = (int)Session["IdEmployee"];
            }
            ViewData["NCs"] = _notificationChannelDAO.FindAllNotificationsByIdReceiver(IdReceiver);

            return View();

        }

        // Willis
        public ActionResult FormRetrieve()
        {
            // Assume ClerkID
            int IdStoreClerk = (int)Session["IdEmployee"];

            // Get Department that seleceted same collection point as store clerk
            List<string> DClerk = _disbursementDAO.ReturnStoreClerkCP(IdStoreClerk);

            DateTime Today = DateTime.Now;
            DateTime LastThu = Today.AddDays(-1);
            while (LastThu.DayOfWeek != DayOfWeek.Thursday)
                LastThu = LastThu.AddDays(-1);

            // If Disbursement contain status "preparing" from last thurseday to today
            if (_disbursementDAO.CheckExistDisbursement(DClerk, Today, LastThu))
            {
                // Search Disbursement with status set as "preparing"
                List<Retrieval> RetrievalForm = _disbursementDAO.RetrievePreparingItem(DClerk, Today, LastThu);
                ViewData["RetrievalForm"] = RetrievalForm;
                ViewData["NoDisbursement"] = false;
                ViewData["NoNewRequisition"] = false;
                ViewBag.Today = Today.ToString("dd-MM-yyyy");
                ViewBag.LastThu = LastThu.ToString("dd-MM-yyyy");

            }
            // If Disbursement contain no status "preparing" from last thursday to today
            else
            {
                ViewData["NoDisbursement"] = true;
                ViewData["NoNewRequisition"] = false;
                ViewBag.Today = Today.ToString("dd-MM-yyyy");
                ViewBag.LastThu = LastThu.ToString("dd-MM-yyyy");
            }
            List<int> CPs = _collectionpointDAO.FindByClerkId(IdStoreClerk);
            ViewData["CPs"] = CPs;
            return View();
        }


        // Willis
        [HttpPost]
        public ActionResult FormRetrieve(int[] idCPs, string StartDate, string EndDate)
        {
            // Assume ClerkID
            int IdStoreClerk = (int)Session["IdEmployee"];
            //Huang Yuzhe : changing collection points function
            if (idCPs != null)
            {
                foreach (int id in idCPs)
                {
                    int tempClerkId = _employeeDAO.FindClerkIdByCPId(id);
                    if (tempClerkId == IdStoreClerk)
                    {
                        continue;
                    }
                    else
                    {
                        List<int> CPIdOfTempClerkId = _collectionpointDAO.FindByClerkId(tempClerkId);
                        List<int> CPIdOfClerkId = _collectionpointDAO.FindByClerkId(IdStoreClerk);
                        for (int i = 0; i < CPIdOfClerkId.Count; i++)
                        {
                            bool noNeedChange = false;
                            foreach (int id2 in idCPs)
                            {
                                if (CPIdOfClerkId[i] == id2)
                                {
                                    noNeedChange = true;
                                }
                            }
                            if (noNeedChange == false)
                            {
                                //swap the collection point between two clerks
                                int temp = CPIdOfClerkId[i];
                                CPIdOfClerkId[i] = id;
                                CPIdOfTempClerkId[CPIdOfTempClerkId.IndexOf(id)] = temp;
                                _collectionpointDAO.ChangeCPTo(IdStoreClerk, CPIdOfClerkId);
                                _collectionpointDAO.ChangeCPTo(tempClerkId, CPIdOfTempClerkId);
                                break;
                            }
                        }
                    }
                }
            }
            List<int> CPs = _collectionpointDAO.FindByClerkId(IdStoreClerk);
            ViewData["CPs"] = CPs;
            // Get Department that seleceted same collection point as store clerk
            List<string> DClerk = _disbursementDAO.ReturnStoreClerkCP(IdStoreClerk);

            if (StartDate.Equals("") || EndDate.Equals(""))
            {
                return RedirectToAction("FormRetrieve", "StoreClerk");
            }
            DateTime SDate = DateTime.ParseExact(StartDate, "dd-MM-yyyy",
                            System.Globalization.CultureInfo.InvariantCulture);
            DateTime EDate = DateTime.ParseExact(EndDate, "dd-MM-yyyy",
                            System.Globalization.CultureInfo.InvariantCulture).AddDays(1);

            // Search and retrieve Requisition & Requisition Item
            List<Retrieval> RetrievalItem = _requisitionDAO
                                            .RetrieveRequisition(DClerk, SDate, EDate);
            // Check if the the RetrievalForm has been created 
            // return retrieval item that have not created to disbursement and disbursementitem  
            List<Retrieval> NewRetrievalItem = _disbursementDAO.CheckRetrievalFormExist(RetrievalItem, SDate);

            // New Retrieval Item is null when all IdRequisition is disbursed
            if (NewRetrievalItem.Any())
            {
                // Create Disbursement and set status to "preparing"
                List<int> IdDisbursement = _disbursementDAO.CreateDisbursement(NewRetrievalItem);
                // Create DisbursementItem and set status to "preparing"
                List<int> IdDisbursementItem = _disbursementItemDAO
                                               .CreateDisbursementItem(IdDisbursement, NewRetrievalItem);

                // Distribute stock item based on approved date given stockunit less than sum of requested unit
                _disbursementItemDAO.DisbursementItemByPriority(NewRetrievalItem);

                // Search Disbursement with status set as "preparing"
                DateTime Today = DateTime.Now;
                List<Retrieval> RetrievalForm = _disbursementDAO.RetrievePreparingItem(DClerk, Today, SDate);
                ViewData["RetrievalForm"] = RetrievalForm;
                ViewData["NoDisbursement"] = false;
                ViewData["NoNewRequisition"] = false;
                ViewBag.Today = EndDate;
                ViewBag.LastThu = StartDate;
            }
            else
            {
                ViewData["NoDisbursement"] = true;
                ViewData["NoNewRequisition"] = true;
                ViewBag.Today = EndDate;
                ViewBag.LastThu = StartDate;
            }
            ViewBag.LastThu = StartDate;
            ViewBag.Today = EndDate;
            return View();
        }


        //Willis
        [HttpPost]
        public ActionResult SaveDisbursement(int[] IdItemRetrieved)
        {
            // Assume ClerkID
            int IdStoreClerk = (int)Session["IdEmployee"];

            // Get Department that seleceted same collection point as store clerk
            List<string> DClerk = _disbursementDAO.ReturnStoreClerkCP(IdStoreClerk);

            if (IdItemRetrieved.Any())
            {
                // Get IdDisbursementItem from Selecte Retrieved Item
                List<int> IdDisbursementItem = _disbursementItemDAO.GetIdByItemRetrieved(DClerk, IdItemRetrieved);
                // update disbursementitem and set status to "prepared"
                // return IdDisbursement with at lease one items have been set as "prepared"
                List<int> IdDisbursement = _disbursementItemDAO.UpdateDisbursementItem(IdDisbursementItem);
                // update disbursement and set status to "prepared"
                _disbursementDAO.UpdateDisbursement(IdDisbursement);
                // update item stock unit and available unit
                _itemDAO.UpdateItem(IdDisbursementItem);

                // update stockrecord
                _stockRecordDAO.UpdateStockRecord(IdStoreClerk, IdDisbursementItem);

                // check if stock unit is less reorder level
                bool IsLowerThanReorderLevel = _itemDAO.CheckIfLowerThanReorderLevel(IdItemRetrieved);

                if (IsLowerThanReorderLevel)
                {
                    // @Shutong: raise alert to all stockclerks, their ID are 1,2,3
                    Employee storeclerk1 = new Employee();
                    Employee storeclerk2 = new Employee();
                    Employee storeclerk3 = new Employee();
                    List<Employee> clerks = _employeeDAO.FindAllClerk();
                    if (clerks.Count >= 3)
                    {
                        storeclerk1 = clerks[0];
                        storeclerk2 = clerks[1];
                        storeclerk3 = clerks[2];
                    }

                    var hub = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                    hub.Clients.All.receiveNotification(1);
                    hub.Clients.All.receiveNotification(2);
                    hub.Clients.All.receiveNotification(3);
                    EmailClass emailClass = new EmailClass();
                    string message = "The stock levels of some items are running low!";
                    _notificationChannelDAO.CreateNotificationsToGroup("Clerk", IdStoreClerk, message);
                    emailClass.SendTo(storeclerk1.Email, "SSIS System Email", message);
                    emailClass.SendTo(storeclerk2.Email, "SSIS System Email", message);
                    emailClass.SendTo(storeclerk3.Email, "SSIS System Email", message);

                }
            }
            else
            {
                return RedirectToAction("FormRetrieve", "StoreClerk");
            }
            return RedirectToAction("FormRetrieve", "StoreClerk");

        }

        //Willis
        [HttpPost]
        public ActionResult PrintPdf(string StartDate, string EndDate)
        {
            // Assume ClerkID
            int IdStoreClerk = (int)Session["IdEmployee"];

            List<string> DClerk = _disbursementDAO.ReturnStoreClerkCP(IdStoreClerk);
            DateTime SDate = DateTime.ParseExact(StartDate, "dd-MM-yyyy",
                            System.Globalization.CultureInfo.InvariantCulture);
            DateTime EDate = DateTime.ParseExact(EndDate, "dd-MM-yyyy",
                            System.Globalization.CultureInfo.InvariantCulture).AddDays(1);
            DateTime Today = DateTime.Now;
            List<Retrieval> RetrievalForm = _disbursementDAO.RetrievePreparingItem(DClerk, Today, SDate);

            RetrievalFormReport retrievalFromReport = new RetrievalFormReport();
            byte[] abytes = retrievalFromReport.PrepareReport(RetrievalForm);
            return File(abytes, "application/pdf", "Retrieve Form.pdf");

        }


        //@Shutong
        public ActionResult PurchaseOrderList()
        {
            ViewData["Incomplete"] = _purchaseOrderDAO.FindIncompletePO();
            ViewData["Pending"] = _purchaseOrderDAO.FindPendingPO();
            ViewData["Rejected"] = _purchaseOrderDAO.FindRejectedPO();
            ViewData["Approved"] = _purchaseOrderDAO.FindApprovedPO();
            ViewData["Delivered"] = _purchaseOrderDAO.FindDeliveredPO();
            ViewData["Cancelled"] = _purchaseOrderDAO.FindCancelledPO();
            ViewData["LowStock"] = _itemDAO.FindLowStockItems();
            return View();
        }


        //@Shutong
        public ActionResult MakePurchaseOrder(string searchStr)
        {
            if (!string.IsNullOrEmpty(searchStr))
            {
                ViewData["SearchResult"] = _itemDAO.FindItemsByKeyword(searchStr);
                ViewData["SearchStr"] = searchStr;
            }
            ViewData["LowStock"] = _itemDAO.FindLowStockItems();

            //ViewData["PODCart"] = _purchaseOrderDetailsDAO.FindAllIncompletePODetails(_purchaseOrderDAO.FindIncompletePO());
            return View();
        }

        //@Shutong
        [HttpPost]
        public ActionResult AddToCart(FormCollection form)
        {
            foreach (Item i in _itemDAO.FindLowStockItems())
            {
                var isChecked = form["checkbox-" + i.IdItem];
                string codeSupplier = form["orderFor-" + i.IdItem];//here got problem
                if (isChecked != null)
                {
                    PurchaseOrder po;
                    //check if there is any exisitng incomplete PO that is for this current supplier
                    if (!_purchaseOrderDAO.IsIncompletePOExist(codeSupplier))
                    {
                        po = _purchaseOrderDAO.Create(codeSupplier, (int)Session["IdEmployee"]);
                    }
                    else
                    {
                        po = _purchaseOrderDAO.FindIncompletePOWithSupplier(codeSupplier);
                    }
                    //if the purchaseorderdetail already exist, add the ammount to it.
                    //else create a new one
                    _purchaseOrderDetailsDAO.CreateOrAddAmount(i, po);
                }
            }
            return RedirectToAction("MakePurchaseOrder", "StoreClerk");
        }


        //@Shutong
        public ActionResult PurchaseOrderCart()
        {
            ViewData["SuppliersInPOCart"] = _purchaseOrderDAO.FindSuppliersFromIncompletePOCart();
            //ViewData["POCart"] = _purchaseOrderDAO.FindIncompletePO();
            return View();
        }

        //@Shutong
        public ActionResult DeletePODFromCart(int id)
        {
            _purchaseOrderDetailsDAO.DeletePOD(id);
            //ViewData["SuppliersInPOCart"] = _purchaseOrderDAO.FindSuppliersFromIncompletePOCart();
            //return Content("You have deleted your Purchase Order details"+id);
            return RedirectToAction("PurchaseOrderCart", "StoreClerk");
        }

        //@Shutong
        public ActionResult UpdateOrderUnit(int orderUnit, int idPOD)
        {
            _purchaseOrderDetailsDAO.UpdateOrderUnitById(orderUnit, idPOD);
            return RedirectToAction("PurchaseOrderCart", "StoreClerk");
        }

        //@Shutong
        [HttpPost]
        public ActionResult SubmitPurchaseOrder(FormCollection form)
        {

            foreach (int id in _purchaseOrderDAO.FindIdOfAllIncompletePO())
            {
                var purchaseOrderID = form["purchaseOrderId_" + id];
                if (purchaseOrderID != null)
                {
                    _purchaseOrderDAO.UpdateStatusToPending(id);
                }
            }
            return RedirectToAction("PurchaseOrderCart", "StoreClerk");
        }

        //@Shutong
        [HttpPost]
        public ActionResult CancelAllPurchaseOrder(FormCollection form)
        {

            foreach (int id in _purchaseOrderDAO.FindIdOfAllIncompletePO())
            {
                var purchaseOrderID = form["purchaseOrderId_" + id];
                if (purchaseOrderID != null)
                {
                    _purchaseOrderDAO.UpdateStatusToCancelled(id);
                }
            }

            return RedirectToAction("PurchaseOrderList", "StoreClerk");
        }

        //@Shutong
        public ActionResult CancelPO(int id)
        {

            _purchaseOrderDAO.UpdateStatusToCancelled(id);

            return RedirectToAction("PurchaseOrderList", "StoreClerk");
        }

        //@Shutong
        public ActionResult WithdrawPO(int id)
        {
            int idEmployee = (int)Session["IdEmployee"];
            _purchaseOrderDAO.UpdateStatusToIncomplete(id);
            //@Shutong: raise notification
            var hub = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            int idsupervisor = _employeeDAO.FindIdByRole("Supervisor")[0];
            hub.Clients.All.receiveNotification(idsupervisor);//5 is supervisor
            EmailClass emailClass = new EmailClass();
            string message = "Hi Supervisor, your store clerk " + _employeeDAO.FindEmployeeById(idEmployee).Name + " has withdrawed the PO " + id;
            _notificationChannelDAO.CreateNotificationsToGroup("Supervisor", idEmployee, message);
            emailClass.SendTo(_employeeDAO.FindEmailsByRole("Supervisor")[0], "SSIS System Email", message);
            return RedirectToAction("PurchaseOrderCart", "StoreClerk");
        }

        //@Shutong
        public ActionResult UpdatePO(int id)
        {
            //not only update the status 
            //but also need to merge with existing PO.... and delete remarks
            _purchaseOrderDAO.UpdateRejectedToIncomplete(id);
            return RedirectToAction("PurchaseOrderCart", "StoreClerk");
        }

        //@Shutong
        [HttpGet]
        public ActionResult CollectPO(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("PurchaseOrderList", "StoreClerk");
            }

            ViewData["PurchaseOrder"] = _purchaseOrderDAO.FindPOById(id.Value);
            ViewData["pod"] = _purchaseOrderDetailsDAO.FindPODetailsByPOId(id.Value);
            return View();
        }

        //@Shutong
        [HttpPost]
        public ActionResult CollectPO(FormCollection form)
        {
            var IdPO = form["IdPO"];
            if (IdPO == null || String.IsNullOrEmpty(IdPO) || IdPO.Contains("undefine"))
            {
                return RedirectToAction("PurchaseOrderList", "StoreClerk");
            }
            int id = Int32.Parse(IdPO);
            foreach (PurchaseOrderDetail pod in _purchaseOrderDetailsDAO.FindPODetailsByPOId(id))
            {
                var deliveredUnit = form["deliveredUnit_" + pod.IdPOD];
                var deliveryRemark = form["deliveryRemarks_" + pod.IdPOD];
                _purchaseOrderDetailsDAO.UpdateDeliveredUnitAndRemarksById(pod.IdPOD, Int32.Parse(deliveredUnit), deliveryRemark);
            }
            _purchaseOrderDAO.UpdateStatusToDelivered(id);
            return RedirectToAction("PurchaseOrderList", "StoreClerk");
        }

        //@Shutong
        public ActionResult Schedule(int IdPO, string deliverDate)
        {
            //2222 - 02 - 01T00: 12
            _purchaseOrderDAO.UpdateSchedule(IdPO, deliverDate);
            return RedirectToAction("PurchaseOrderList", "StoreClerk");

        }

        //@Shutong
        public ActionResult ViewSupplierItem(int Iditem, string codesupplier)
        {
            SupplierItem supplierItem = _supplierItemDAO.FindSupplierItemByIditemAndCodesupplier(Iditem, codesupplier);
            ViewData["supplierItem"] = supplierItem;
            return View();
        }


        // James: Disbursement overview
        public ActionResult Disbursement()
        {
            /*            if (Session["IdEmployee"] == null || (String)Session["Role"] != "StockClerk")
                            return RedirectToAction("Login", "Home");*/

            int IdStoreClerk = 1;
            if (Session["IdEmployee"] != null)
            {
                IdStoreClerk = (int)Session["IdEmployee"];
            }



            // retrieve 2 lists of Disbursement Lists which are "Prepared" and "Scheduled" under the same Coll Point
            // find by Status and Clerk's Collection Points
            List<Disbursement> prepList = _disbursementDAO.FindByStatus("Prepared", IdStoreClerk);
            List<Disbursement> scheList = _disbursementDAO.FindByStatus("Scheduled", IdStoreClerk);
            scheList.AddRange(_disbursementDAO.FindByStatus("Received", IdStoreClerk));
            scheList.AddRange(_disbursementDAO.FindByStatus("Disbursed", IdStoreClerk));

            ViewBag.prepList = prepList;
            ViewBag.scheList = scheList;

            // Finds Next Monday
            DateTime NextMon = DateTime.Now;
            while (NextMon.DayOfWeek != DayOfWeek.Monday)
                NextMon = NextMon.AddDays(1);

            ViewBag.NextMon = NextMon.ToString("yyyy-MM-dd");

            return View();
        }

        // James: Selecting multiple disbursements to schedule for collection
        [HttpPost]
        public ActionResult Schedule(IEnumerable<int> disbIdsToSchedule, String pickDate)
        {
            /*            if (Session["IdEmployee"] == null || (String)Session["Role"] != "StockClerk")
                            return RedirectToAction("Login", "Home");*/

            int IdStoreClerk = 1;
            if (Session["IdEmployee"] != null)
            {
                IdStoreClerk = (int)Session["IdEmployee"];
            }



            if (disbIdsToSchedule != null)
            {
                // schedule for selected date by setting the date from the form
                DateTime SDate = DateTime.ParseExact(pickDate, "yyyy-MM-dd",
                                System.Globalization.CultureInfo.InvariantCulture);

                _disbursementDAO.UpdateStatus(disbIdsToSchedule, 10, SDate, null);

                // add in notification here upon updating status
                foreach (var disbId in disbIdsToSchedule)
                {
                    Disbursement targetDisbursement = _disbursementDAO.FindById(disbId);
                    // Get Dep Rep
                    Employee depRep = targetDisbursement.Department.Employees
                        .Where(emp => emp.IdRole == 3)
                        .FirstOrDefault();

                    String message = (targetDisbursement.DisbursementItems.ToList().All(i => i.UnitIssued >= i.UnitRequested)) ?
                        $"Your department's request will be ready for collection on {SDate.ToString("dd/MM/yyyy")}." :
                        $"Your department's request will be ready for collection on {SDate.ToString("dd/MM/yyyy")}. " +
                            $"We are currently unable to prepare the full quantity of requested items from your department.";

                    int notifId = _notificationDAO.CreateNotification(message);
                    _notificationChannelDAO.SendNotification(IdStoreClerk, depRep.IdEmployee, notifId, DateTime.Now);
                    var hub = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                    hub.Clients.All.receiveNotification(depRep.IdEmployee);
                    EmailClass emailClass = new EmailClass();
                    emailClass.SendTo(depRep.Email, "SSIS System Email", message);


                }
            }

            return RedirectToAction("Disbursement");
        }

        // James: Scheduling a single disbursement with redistribution if necessary
        [HttpPost]
        public ActionResult ScheduleSingle(IEnumerable<int> disbId, IList<int> disbItemId, IList<int> transferQtyNum, IList<int> disbItemIdDeptFrom, String pickDate)
        {
            /*            if (Session["IdEmployee"] == null || (String)Session["Role"] != "StockClerk")
                            return RedirectToAction("Login", "Home");*/

            int IdStoreClerk = 1;
            if (Session["IdEmployee"] != null)
            {
                IdStoreClerk = (int)Session["IdEmployee"];
            }



            // give and take from disbursement Item and creates the reversal entry and new entry for stock records
            _disbursementItemDAO.GiveAndTake(disbItemId, transferQtyNum, disbItemIdDeptFrom, IdStoreClerk);

            // schedule for selected date by setting the date from the form
            DateTime SDate = DateTime.ParseExact(pickDate, "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture);

            // add in notification here upon updating status and notify on shortfall (if any)
            foreach (var di in disbId)
            {
                Disbursement targetDisbursement = _disbursementDAO.FindById(di);
                // Get Dep Rep
                Employee depRep = targetDisbursement.Department.Employees
                    .Where(emp => emp.IdRole == 3)
                    .FirstOrDefault();

                String message = (targetDisbursement.DisbursementItems.ToList().All(i => i.UnitIssued >= i.UnitRequested)) ?
                    $"Your department's request will be ready for collection on {SDate.ToString("dd/MM/yyyy")}." :
                    $"Your department's request will be ready for collection on {SDate.ToString("dd/MM/yyyy")}. " +
                        $"We are currently unable to prepare the full quantity of requested items from your department.";

                int notifId = _notificationDAO.CreateNotification(message);
                _notificationChannelDAO.SendNotification(IdStoreClerk, depRep.IdEmployee, notifId, DateTime.Now);
                var hub = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                hub.Clients.All.receiveNotification(depRep.IdEmployee);
                EmailClass emailClass = new EmailClass();
                emailClass.SendTo(depRep.Email, "SSIS System Email", message);
            }

            _disbursementDAO.UpdateStatus(disbId, 10, SDate, null);
            return RedirectToAction("Disbursement");
        }


        // James: Opens page to redistribute qty from other disbursements
        public ActionResult Redistribute(int disbId)
        {
            /*            if (Session["IdEmployee"] == null || (String)Session["Role"] != "StockClerk")
                            return RedirectToAction("Login", "Home");*/

            int IdStoreClerk = 1;
            if (Session["IdEmployee"] != null)
            {
                IdStoreClerk = (int)Session["IdEmployee"];
            }



            Disbursement targetDisbursement = _disbursementDAO.FindById(disbId);
            ViewBag.disb = targetDisbursement;

            ViewBag.dropdownDisbursementItems = _disbursementItemDAO.FindCorrespondingDisbursementItems(targetDisbursement.DisbursementItems, IdStoreClerk);

            // Finds Next Monday
            DateTime NextMon = DateTime.Now;
            while (NextMon.DayOfWeek != DayOfWeek.Monday)
                NextMon = NextMon.AddDays(1);

            ViewBag.NextMon = NextMon.ToString("yyyy-MM-dd");

            // Get Dep Rep
            Employee depRep = targetDisbursement.Department.Employees
                .ToList()
                .Where(emp => emp.IdRole == 3)
                .FirstOrDefault();

            ViewBag.depRep = depRep;

            return PartialView("Redistribute", targetDisbursement);
        }

        //James: Opens page to handle disbursement with Dep Rep
        public ActionResult DisbursementDetails(int disbId)
        {
            /*            if (Session["IdEmployee"] == null || (String)Session["Role"] != "StockClerk")
                 return RedirectToAction("Login", "Home");*/

            int IdStoreClerk = 1;
            if (Session["IdEmployee"] != null)
            {
                IdStoreClerk = (int)Session["IdEmployee"];
            }



            Disbursement targetDisbursement = _disbursementDAO.FindById(disbId);
            ViewBag.disb = targetDisbursement;

            // Get Dep Rep
            Employee depRep = targetDisbursement.Department.Employees
                .Where(emp => emp.IdRole == 3)
                .FirstOrDefault();

            ViewBag.depRep = depRep;

            return PartialView("DisbursementDetails");
        }

        //James: Refresh Disbursement page and updates the unitIssued to the QtyDisbursed
        [HttpPost]
        public ActionResult RefreshDisbursement(IEnumerable<int> disbId, IList<int> disbItemId, IList<int> qtyDisbursed)
        {
            /*            if (Session["IdEmployee"] == null || (String)Session["Role"] != "StockClerk")
                 return RedirectToAction("Login", "Home");*/

            int IdStoreClerk = 1;
            if (Session["IdEmployee"] != null)
            {
                IdStoreClerk = (int)Session["IdEmployee"];
            }





            Disbursement targetDisbursement = _disbursementDAO.FindById(disbId.First());
            ViewBag.disb = targetDisbursement;
            List<DisbursementItem> targetList = targetDisbursement.DisbursementItems.ToList();
            Employee sup = _employeeDAO.FindByRole(7).FirstOrDefault();
            Employee man = _employeeDAO.FindByRole(6).FirstOrDefault();

            // if qtyDisbursed < disbItem.UnitIssued then raise a SA-broken and a reversal entry to qtyDisbursed
            for (int i = 0; i < targetList.Count; i++)
            {
                if (qtyDisbursed[i] < targetList[i].UnitIssued)
                {
                    _stockRecordDAO.StockAdjustmentDuringDisbursement(qtyDisbursed[i], targetList[i], IdStoreClerk);
                    // reverses the amount disbursed to the department
                    // commented out as net = 0 diff
                    //_itemDAO.UpdateUnits(targetList[i].Item, -(targetList[i].UnitIssued - qtyDisbursed[i]));
                    //_itemDAO.UpdateUnits(targetList[i].Item, (targetList[i].UnitIssued - qtyDisbursed[i]));

                    // sends a notification to either supervisor or manager on stock record's value
                    #region Send notification
                    String message = $"Stock adjustment raised for Item ({targetList[i].Item.Description}). Please approve/reject.";
                    int notifId = _notificationDAO.CreateNotification(message);
                    var hub = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();

                    SupplierItem si = _supplierItemDAO.FindByItem(targetList[i].Item);
                    if (Math.Abs((targetList[i].UnitIssued - qtyDisbursed[i]) * si.Price) >= 250)
                    {
                        _notificationChannelDAO.SendNotification(IdStoreClerk, man.IdEmployee, notifId, DateTime.Now);
                        hub.Clients.All.receiveNotification(man.IdEmployee);
                        EmailClass emailClass = new EmailClass();
                        emailClass.SendTo(man.Email, "SSIS System Email", message);
                    }
                    else
                    {
                        _notificationChannelDAO.SendNotification(IdStoreClerk, sup.IdEmployee, notifId, DateTime.Now);
                        hub.Clients.All.receiveNotification(sup.IdEmployee);
                        EmailClass emailClass = new EmailClass();
                        emailClass.SendTo(sup.Email, "SSIS System Email", message);
                    }
                    #endregion

                }
            }

            // updates the disbitemId's unitissued to the qtyDisbursed
            _disbursementItemDAO.UpdateUnitIssued(disbItemId, qtyDisbursed);

            return RedirectToAction("Disbursement");
        }

        //James: For clerk to sign, raises SA in case of discrepancy and email out a copy of the disbursement details
        [HttpPost]
        public ActionResult ClerkSign(IEnumerable<int> disbId)
        {
            /*            if (Session["IdEmployee"] == null || (String)Session["Role"] != "StockClerk")
                            return RedirectToAction("Login", "Home");*/

            int IdStoreClerk = 1;
            if (Session["IdEmployee"] != null)
            {
                IdStoreClerk = (int)Session["IdEmployee"];
            }

            // updates the disb's status to "Disbursed" or 7
            _disbursementDAO.UpdateStatus(disbId, 7, DateTime.Now, IdStoreClerk);

            // emails a copy / sends notification to DR and Clerk
            foreach (var di in disbId)
            {
                Disbursement targetDisbursement = _disbursementDAO.FindById(di);
                // Get Dep Rep
                Employee depRep = targetDisbursement.Department.Employees
                    .Where(emp => emp.IdRole == 3)
                    .FirstOrDefault();

                String message = $"Attached a copy of the acknolwedged Disbursement for {targetDisbursement.CodeDepartment} on {targetDisbursement.Date.ToString("dd/MM/yyyy")}./n" +
                    $"Department Rep: " + depRep.Name + "/n" +
                    $"Store Clerk: " + _employeeDAO.FindEmployeeById(IdStoreClerk).Name +
                $"Both achknowledged. " +
                "Download report at https://localhost:44304/StoreClerk/PrintDisbursementPdf?disbId=" + targetDisbursement.IdDisbursement;

                int notifId = _notificationDAO.CreateNotification(message);
                _notificationChannelDAO.SendNotification(IdStoreClerk, depRep.IdEmployee, notifId, DateTime.Now);
                _notificationChannelDAO.SendNotification(IdStoreClerk, IdStoreClerk, notifId, DateTime.Now);
                var hub = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                hub.Clients.All.receiveNotification(depRep.IdEmployee);
                hub.Clients.All.receiveNotification(IdStoreClerk);
                EmailClass emailClass = new EmailClass();
                emailClass.SendTo(depRep.Email, "SSIS System Email", message);
                emailClass.SendTo(_employeeDAO.FindEmployeeById(IdStoreClerk).Email, "SSIS System Email", message);
            }

            return RedirectToAction("Disbursement");
        }

        //James: Print Disbursement Details' PDF
        [HttpGet]
        public ActionResult PrintDisbursementPdf(IEnumerable<int> disbId)
        {
            Disbursement targetDisbursement = _disbursementDAO.FindById(disbId.First());
            List<DisbursementItem> targetList = targetDisbursement.DisbursementItems.ToList();

            // Get Dep Rep
            Employee depRep = targetDisbursement.Department.Employees
                .Where(emp => emp.IdRole == 3)
                .FirstOrDefault();

            DisbursementReport disbursementReport = new DisbursementReport();
            byte[] abytes = disbursementReport.PrepareReport(targetDisbursement, targetList, depRep);

            String filename = $"{targetDisbursement.Date.ToString("yyyyMMdd")}_Disbursement_{targetDisbursement.IdDisbursement}.pdf";

            return File(abytes, "application/pdf", filename);
        }

        //James: Stocktake overview
        public ActionResult Stocktake()
        {
            /*            if (Session["IdEmployee"] == null || (String)Session["Role"] != "StockClerk")
                   return RedirectToAction("Login", "Home");*/

            int IdStoreClerk = 1;
            if (Session["IdEmployee"] != null)
            {
                IdStoreClerk = (int)Session["IdEmployee"];
            }

            ViewBag.allItems = _itemDAO.GetAllItems();

            ViewBag.mth1 = DateTime.Now.ToString("yyyy-MM");
            ViewBag.mth2 = DateTime.Now.AddMonths(-1).ToString("yyyy-MM");
            ViewBag.mth3 = DateTime.Now.AddMonths(-2).ToString("yyyy-MM");
            ViewBag.mth4 = DateTime.Now.AddMonths(-3).ToString("yyyy-MM");

            return View();
        }


        //James: Save the created stocktake into individual stockrecords
        [HttpPost]
        public ActionResult SaveStocktake(IList<int> itemId, IList<int> actualQty, IList<int> missingQty, IList<int> wrongQty, IList<int> brokenQty, IList<int> giftQty)
        {
            /*            if (Session["IdEmployee"] == null || (String)Session["Role"] != "StockClerk")
                    return RedirectToAction("Login", "Home");*/

            int IdStoreClerk = 1;
            if (Session["IdEmployee"] != null)
            {
                IdStoreClerk = (int)Session["IdEmployee"];
            }



            Debug.WriteLine($"actual: {actualQty.Count}, missing: {missingQty.Count}, wrong: {wrongQty.Count}, broken: {brokenQty.Count}, gift: {giftQty.Count}");
            Debug.WriteLine($"actual: {actualQty[0]}, missing: {missingQty[0]}, wrong: {wrongQty[0]}, broken: {brokenQty[0]}, gift: {giftQty[0]}");

            List<Item> allItems = _itemDAO.GetAllItems();

            Item item;
            int diff;
            DateTime now = DateTime.Now;
            Employee man = _employeeDAO.FindByRole(6).FirstOrDefault();
            Employee sup = _employeeDAO.FindByRole(7).FirstOrDefault();

            //update the Item's stock and available unit as well as create stock records
            for (int i = 0; i < actualQty.Count; i++)
            {
                // find Item
                item = allItems[i];

                // check if stockunit == actualQty
                if (item.StockUnit != actualQty[i])
                {
                    // get difference to be applied
                    diff = item.StockUnit - actualQty[i];

                    // apply difference to both Stock and Available units
                    _itemDAO.UpdateUnits(item, diff);

                    // Raise SA instead
                    //_stockRecordDAO.RaiseSA(now, 3, null, null, IdStoreClerk, item.IdItem, -diff);
                    RaiseSAandNotifyBoss(now, 3, null, null, IdStoreClerk, item, -diff, sup, man);

                }

                // Create stockRecord
                if (missingQty[i] > 0)
                {
                    //_stockRecordDAO.RaiseSA(now, 3, null, null, IdStoreClerk, item.IdItem, -missingQty[i]);
                    RaiseSAandNotifyBoss(now, 3, null, null, IdStoreClerk, item, -missingQty[i], sup, man);

                }
                if (wrongQty[i] > 0)
                {
                    //_stockRecordDAO.RaiseSA(now, 4, null, null, IdStoreClerk, item.IdItem, -wrongQty[i]);
                    RaiseSAandNotifyBoss(now, 4, null, null, IdStoreClerk, item, -wrongQty[i], sup, man);
                }
                if (brokenQty[i] > 0)
                {
                    //_stockRecordDAO.RaiseSA(now, 5, null, null, IdStoreClerk, item.IdItem, -brokenQty[i]);
                    RaiseSAandNotifyBoss(now, 5, null, null, IdStoreClerk, item, -brokenQty[i], sup, man);

                }
                if (giftQty[i] > 0)
                {
                    //_stockRecordDAO.RaiseSA(now, 6, null, null, IdStoreClerk, item.IdItem, giftQty[i]);
                    RaiseSAandNotifyBoss(now, 6, null, null, IdStoreClerk, item, giftQty[i], sup, man);
                }

            }

            return RedirectToAction("Stocktake");
        }

        public void RaiseSAandNotifyBoss(DateTime date, int IdOperation, String IdDepartment, String IdSupplier, int IdStoreClerk, Item item, int qty,
            Employee supervisor, Employee manager)
        {

            var hub = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            String message = $"Stock adjustment raised for Item ({item.Description}). Please approve/reject.";
            _stockRecordDAO.RaiseSA(date, IdOperation, IdDepartment, IdSupplier, IdStoreClerk, item.IdItem, qty);
            int notifId = _notificationDAO.CreateNotification(message);


            SupplierItem si = _supplierItemDAO.FindByItem(item);
            if (Math.Abs(qty * si.Price) > 250)
            {
                _notificationChannelDAO.SendNotification(IdStoreClerk, manager.IdEmployee, notifId, date);
                //var hub = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                hub.Clients.All.receiveNotification(manager.IdEmployee);
                EmailClass emailClass = new EmailClass();
                emailClass.SendTo(manager.Email, "SSIS System Email", message);
            }

            else
            {
                _notificationChannelDAO.SendNotification(IdStoreClerk, supervisor.IdEmployee, notifId, date);
                hub.Clients.All.receiveNotification(supervisor.IdEmployee);
                EmailClass emailClass = new EmailClass();
                emailClass.SendTo(supervisor.Email, "SSIS System Email", message);
            }

        }

        //James: View past stocktake based on time
        public ActionResult ViewStocktake(String targetMonth)
        {
            /*            if (Session["IdEmployee"] == null || (String)Session["Role"] != "StockClerk")
                return RedirectToAction("Login", "Home");*/

            int IdStoreClerk = 1;
            if (Session["IdEmployee"] != null)
            {
                IdStoreClerk = (int)Session["IdEmployee"];
            }



            DateTime month = DateTime.ParseExact(targetMonth, "yyyy-MM",
                System.Globalization.CultureInfo.InvariantCulture);
            List<StockRecord> SRbyMonth = _stockRecordDAO.FindByMonthAndYear(month);

            ViewBag.SRbyMonth = SRbyMonth;

            ViewBag.targetMonth = month;

            return PartialView("ViewStocktake");
        }

    }
}