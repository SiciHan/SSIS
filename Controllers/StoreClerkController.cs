﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Team8ADProjectSSIS.DAO;
using Team8ADProjectSSIS.Models;

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
        private readonly ItemDAO _itemDAO;
        private readonly CollectionPointDAO _collectionpointDAO;
        private readonly EmployeeDAO _employeeDAO;
        public StoreClerkController()
        {
            this._disbursementDAO = new DisbursementDAO();
            this._requisitionDAO = new RequisitionDAO();
            this._requisitionItemDAO = new RequisitionItemDAO();
            this._stockRecordDAO = new StockRecordDAO();
            this._disbursementItemDAO = new DisbursementItemDAO();
            this._purchaseOrderDAO = new PurchaseOrderDAO();
            this._itemDAO = new ItemDAO();
            this._collectionpointDAO = new CollectionPointDAO();
            this._employeeDAO = new EmployeeDAO();
        }
        

        // GET: StoreClerk
        public ActionResult Index()
        {
            return View();
        }
        // Get: FormRetrieve Method
        public ActionResult FormRetrieve()
        {
            // Assume ClerkID
            int IdStoreClerk = 3;

            // Get Department that seleceted same collection point as store clerk
            List<string> DClerk = _disbursementDAO.ReturnStoreClerkCP(IdStoreClerk);

            DateTime Today = DateTime.Now;
            DateTime LastThu = Today.AddDays(-3);
            while (LastThu.DayOfWeek != DayOfWeek.Thursday)
                LastThu = LastThu.AddDays(-1);

            // If Disbursement contain status "preparing" from last thurseday to today
            if (_disbursementDAO.CheckExistDisbursement(DClerk, Today, LastThu))
            {
                // Search Disbursement with status set as "preparing"
                List<Retrieval> RetrievalForm = _disbursementDAO.RetrievePreparingItem(DClerk);
                ViewData["RetrievalForm"] = RetrievalForm;
                ViewData["NoDisbursement"] = false;
                ViewData["NoNewRequisition"] = false;

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
                            System.Globalization.CultureInfo.InvariantCulture);
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
                List<Retrieval> RetrievalForm = _disbursementDAO.RetrievePreparingItem(DClerk);
                ViewData["RetrievalForm"] = RetrievalForm;
                ViewData["NoDisbursement"] = false;
                ViewData["NoNewRequisition"] = false;
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
            return View();
        }
    }
}