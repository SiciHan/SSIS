using System;
using System.Collections.Generic;
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
    }
}