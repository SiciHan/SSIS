using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Team8ADProjectSSIS.DAO;
using Team8ADProjectSSIS.Models;
using Team8ADProjectSSIS.Report;

namespace Team8ADProjectSSIS.Controllers
{
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
            //this._employeeDAO = new EmployeeDAO();
        }
        

        // GET: StoreClerk
        public ActionResult Index()
        {
            int IdReceiver = 1;
            if (Session["IdEmployee"] != null)
            {
                IdReceiver= (int)Session["IdEmployee"];
            }
            ViewData["NCs"]=_notificationChannelDAO.FindAllNotificationsByIdReceiver(IdReceiver);

            return View();
            
        }

        // Get: FormRetrieve Method
        public ActionResult FormRetrieve()
        {
            // Assume ClerkID
            int IdStoreClerk = 3;

            // Get Department that seleceted same collection point as store clerk
            List<string> DClerk = _disbursementDAO.ReturnStoreClerkCP(IdStoreClerk);

            bool NoDisbursement = false;
            DateTime Today = DateTime.Now;
            DateTime LastThu = Today.AddDays(-4);
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
        // Post Method
        [HttpPost]
        public ActionResult FormRetrieve(int[] idCPs, string StartDate, string EndDate)
        {
            // Assume ClerkID
            int IdStoreClerk = 3;
            if (idCPs != null)
            {
                foreach(int id in idCPs)
                {
                    int tempClerkId = _employeeDAO.FindClerkIdByCPId(id);
                    if(tempClerkId == IdStoreClerk)
                    {
                        continue;
                    }
                    else
                    {
                        List<int> CPIdOfTempClerkId = _collectionpointDAO.FindByClerkId(tempClerkId);
                        List<int> CPIdOfClerkId = _collectionpointDAO.FindByClerkId(IdStoreClerk);
                        for(int i=0; i<CPIdOfClerkId.Count; i++)
                        {
                            bool noNeedChange = false;
                            foreach(int id2 in idCPs)
                            {
                                if(CPIdOfClerkId[i] == id2)
                                {
                                    noNeedChange = true;
                                }
                            }
                            if(noNeedChange == false)
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
            List<Retrieval> NewRetrievalItem = _disbursementDAO.CheckRetrievalFormExist(RetrievalItem);

            // New Retrieval Item is null when all IdRequisition is disbursed
            if (NewRetrievalItem.Any())
            {
                // Create Disbursement and set status to "preparing"
                List<int> PKDisbursement = _disbursementDAO.CreateDisbursement(NewRetrievalItem);
                // Create DisbursementItem and set status to "preparing"
                List<int> PKDisbursementItem = _disbursementItemDAO
                                               .CreateDisbursementItem(PKDisbursement, NewRetrievalItem);
                // Search Disbursement with status set as "preparing"
                List<Retrieval> RetrievalForm = _disbursementDAO.RetrievePreparingItem(DClerk, EDate, SDate);
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

        
        [HttpPost]
        public ActionResult SaveDisbursement(int[] IdItemRetrieved) 
        {
            // Assume ClerkID
            int IdStoreClerk = 3;

            // Get Department that seleceted same collection point as store clerk
            List<string> DClerk = _disbursementDAO.ReturnStoreClerkCP(IdStoreClerk);

            if (IdItemRetrieved.Any())
            {
                // update disbursementitem and set status to "prepared"
                // return IdDisbursement with at lease one items have been set as "prepared"
                List<int> IdDisbursement = _disbursementItemDAO.UpdateDisbursementItem(DClerk, IdItemRetrieved);
                // update disbursement and set status to "prepared"
                _disbursementDAO.UpdateDisbursement(IdDisbursement);
                // update item stock unit

                // raise alert

            }
            else
            {
                return RedirectToAction("FormRetrieve", "StoreClerk");
            }
            return RedirectToAction("FormRetrieve", "StoreClerk");

        }
        [HttpPost]
        public ActionResult PrintPdf(string StartDate, string EndDate)
        {
            // Assume ClerkID
            int IdStoreClerk = 3;

            List<string> DClerk = _disbursementDAO.ReturnStoreClerkCP(IdStoreClerk);
            DateTime SDate = DateTime.ParseExact(StartDate, "dd-MM-yyyy",
                            System.Globalization.CultureInfo.InvariantCulture);
            DateTime EDate = DateTime.ParseExact(EndDate, "dd-MM-yyyy",
                            System.Globalization.CultureInfo.InvariantCulture).AddDays(1);

            List<Retrieval> RetrievalForm = _disbursementDAO.RetrievePreparingItem(DClerk, EDate, SDate);

            RetrievalFormReport retrievalFromReport = new RetrievalFormReport();
            byte[] abytes = retrievalFromReport.PrepareReport(RetrievalForm);
            return File(abytes,"application/pdf", "Retrieve Form.pdf");

            //return RedirectToAction("FormRetrieve", "StoreClerk");
        }
        
        //@Shutong
        public ActionResult PurchaseOrderList()
        {
            ViewData["Incomplete"]=_purchaseOrderDAO.FindIncompletePO();
            ViewData["Pending"]=_purchaseOrderDAO.FindPendingPO();
            ViewData["Rejected"]=_purchaseOrderDAO.FindRejectedPO();
            ViewData["Approved"]=_purchaseOrderDAO.FindApprovedPO();
            ViewData["Delivered"]=_purchaseOrderDAO.FindDeliveredPO();
            ViewData["Cancelled"]=_purchaseOrderDAO.FindCancelledPO();
            ViewData["LowStock"] = _itemDAO.FindLowStockItems();
            return View();
        }
        //@Shutong
        public ActionResult MakePurchaseOrder(string searchStr)
        {
            if (!string.IsNullOrEmpty(searchStr))
            {
                ViewData["SearchResult"]= _itemDAO.FindItemsByKeyword(searchStr);
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
            foreach(Item i in _itemDAO.FindLowStockItems())
            {
                var isChecked = form["checkbox-" + i.IdItem];
                string codeSupplier = form["orderFor-" + i.IdItem];//here got problem
                if (isChecked != null)
                {
                    PurchaseOrder po;
                    //check if there is any exisitng incomplete PO that is for this current supplier
                    if (!_purchaseOrderDAO.IsIncompletePOExist(codeSupplier))
                    {
                        po = _purchaseOrderDAO.Create(codeSupplier,1);
                    }
                    else
                    {
                        po= _purchaseOrderDAO.FindIncompletePOWithSupplier(codeSupplier);
                    }
                    //if the purchaseorderdetail already exist, add the ammount to it.
                    //else create a new one
                    _purchaseOrderDetailsDAO.CreateOrAddAmount(i,po);
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

            _purchaseOrderDAO.UpdateStatusToIncomplete(id);

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
            if (IdPO == null || String.IsNullOrEmpty(IdPO)||IdPO.Contains("undefine"))
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
        public ActionResult Schedule(int IdPO, string deliverDate)
        {
            //2222 - 02 - 01T00: 12
            _purchaseOrderDAO.UpdateSchedule(IdPO, deliverDate);
            return RedirectToAction("PurchaseOrderList", "StoreClerk");

        }
    }
}