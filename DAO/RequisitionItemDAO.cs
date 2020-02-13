using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.DAO
{
    public class RequisitionItemDAO
    {
        private readonly SSISContext context;

        public RequisitionItemDAO()
        {
            this.context = new SSISContext();
        }
        //SH
        public List<RequisitionItem> FindRequisitionItem(int idRequisition)
        {
            return context.RequisitionItems.Include("Item").Where(r => r.IdRequisiton == idRequisition).ToList();
        }
        //SH --> 
        public List<RequisitionItem> GetItemListRequisition(int idRequisition)
        {
            RequisitionDAO _requisitionDAO = new RequisitionDAO();
            Requisition requisition = _requisitionDAO.FindRequisitionByRequisionId(idRequisition); // find requisitionItemList by reqId
            // compare idtem with Item to retrieve list of Requested Item
            return context.RequisitionItems.Where(r => r.IdRequisiton == requisition.IdRequisition).ToList(); // gives list of RequisitionItems
            //return context.Items.Where(i => i.IdItem == requisition.IdRequisition).ToList();
        }
        public List<Retrieval> RetrieveRequisitionItem(List<Retrieval> RetrievalItem)
        {
            var RetrievalForm = RetrievalItem.GroupBy(x => x.IdItem)
                            .Select(y => new Retrieval
                            {
                                Description = y.First().Description,
                                IdItem = y.First().IdItem,
                                StockUnit = y.First().StockUnit,
                                //CodeDepartment = y.First().CodeDepartment,      // Cannot be groupby No physical usage
                                //IdRequisition = y.Sum(z => z.IdRequisition),    // No physical usage after groupby
                                Unit = y.Sum(z => z.Unit)
                            }).ToList();

            return RetrievalForm;

        }
        //@SHutong
        internal void CreateRequisitionItem(int idEmployee, string itemName, int quantity)
        {
            RequisitionItem requisitionItem = new RequisitionItem();
            Requisition requisition = context.Requisitions.OfType<Requisition>().Where(x => x.IdEmployee == idEmployee && x.StatusCurrent.Label.Equals("Incomplete")).FirstOrDefault();
            Item item = context.Items.OfType<Item>().Where(x => x.Description.Equals(itemName)).FirstOrDefault();
            requisitionItem.Requisition = requisition;
            requisitionItem.Item = item;
            requisitionItem.Unit = quantity;
            context.RequisitionItems.Add(requisitionItem);
            context.SaveChanges();
        }
        //@SHutong
        internal void UpdateRequisitionItemUnit(int? selectedId, string itemName, int? quantity)
        {
            int id = selectedId.GetValueOrDefault(0);
            int qty = quantity.GetValueOrDefault(0);

            RequisitionItem requisitionItem = context.RequisitionItems.OfType<RequisitionItem>().Where(x => x.Item.Description.Equals(itemName) && x.IdRequisiton == id).FirstOrDefault();
            requisitionItem.Unit = qty;
            context.SaveChanges();

        }
        //@SHutong
        internal void CreateRequisitionItemByReqID(int? selectedId, string itemName, int? quantity)
        {
            RequisitionItem requisitionItem = new RequisitionItem();
            Item item = context.Items.OfType<Item>().Where(x => x.Description.Equals(itemName)).FirstOrDefault();
            requisitionItem.IdRequisiton = selectedId.GetValueOrDefault(0);
            requisitionItem.Item = item;
            requisitionItem.Unit = quantity.GetValueOrDefault(0);
            context.RequisitionItems.Add(requisitionItem);
            context.SaveChanges();
        }
        //@SHutong
        internal void DeleteRequisitionItem(int? selectedId, string itemName)
        {
            int id = selectedId.GetValueOrDefault(0);
            RequisitionItem requisitionItem = context.RequisitionItems.OfType<RequisitionItem>().Where(x => x.Item.Description.Equals(itemName) && x.IdRequisiton == id).FirstOrDefault();
            context.RequisitionItems.Remove(requisitionItem);
            context.SaveChanges();
        }
        //@SHutong
        internal void DeleteRequisitionItemByReqId(int? selectedId)
        {
            int id = selectedId.GetValueOrDefault(0);
            List<RequisitionItem> reqItemList = context.RequisitionItems.OfType<RequisitionItem>().Where(x => x.IdRequisiton == id).ToList();
            context.RequisitionItems.RemoveRange(reqItemList);
            context.SaveChanges();
        }
        //@SHutong
        internal List<RequisitionItem> RetrieveRequisitionItemByReqId(int ReqId)
        {
            List<RequisitionItem> reqItemList = context.RequisitionItems.OfType<RequisitionItem>().Where(x => x.IdRequisiton == ReqId).Include(x=>x.Item).ToList();
            return reqItemList;
        }
    }
}