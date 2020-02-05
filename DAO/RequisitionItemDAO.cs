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

        internal void UpdateRequisitionItemUnit(int? selectedId, string itemName, int? quantity)
        {
            RequisitionItem requisitionItem = context.RequisitionItems.OfType<RequisitionItem>().Where(x => x.Item.Description.Equals(itemName) && x.IdRequisiton == (selectedId.GetValueOrDefault())).FirstOrDefault();
            requisitionItem.Unit = quantity.GetValueOrDefault();
            context.SaveChanges();

        }

        internal void CreateRequisitionItemByReqID(int? selectedId, string itemName, int? quantity)
        {
            RequisitionItem requisitionItem = new RequisitionItem();
            Item item = context.Items.OfType<Item>().Where(x => x.Description.Equals(itemName)).FirstOrDefault();
            requisitionItem.IdRequisiton = selectedId.GetValueOrDefault();
            requisitionItem.Item = item;
            requisitionItem.Unit = quantity.GetValueOrDefault();
            context.RequisitionItems.Add(requisitionItem);
            context.SaveChanges();
        }

        internal void DeleteRequisitionItem(int? selectedId, string itemName)
        {
            RequisitionItem requisitionItem = context.RequisitionItems.OfType<RequisitionItem>().Where(x => x.Item.Description.Equals(itemName) && x.IdRequisiton == (selectedId.GetValueOrDefault())).FirstOrDefault();
            context.RequisitionItems.Remove(requisitionItem);
            context.SaveChanges();
        }

        internal void DeleteRequisitionItemByReqId(int? selectedId)
        {
            List<RequisitionItem> reqItemList = context.RequisitionItems.OfType<RequisitionItem>().Where(x => x.IdRequisiton == selectedId.GetValueOrDefault()).ToList();
            context.RequisitionItems.RemoveRange(reqItemList);
            context.SaveChanges();
        }

        internal List<RequisitionItem> RetrieveRequisitionItemByReqId(int ReqId)
        {
            List<RequisitionItem> reqItemList = context.RequisitionItems.OfType<RequisitionItem>().Where(x => x.IdRequisiton == ReqId).Include(x=>x.Item).ToList();
            return reqItemList;
        }
    }
}