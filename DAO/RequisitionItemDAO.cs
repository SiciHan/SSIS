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

        public List<Retrieval> RetrieveRequisitionItemQuantity(List<int> SelectedReqisition) 
        {
            List<Retrieval> RequestedItem = new List<Retrieval>();
            foreach (var sr in SelectedReqisition) 
            {
                /*var RequisitionItem = context.RequisitionItems
                                      .Where(x => x.IdRequisiton == sr).FirstOrDefault();
                RequestedItem.Add(RequisitionItem);*/
                var retrieval = context.Items
                        .Join(context.RequisitionItems,
                        items => items.IdItem, ri => ri.IdItem,
                        (items, ri) => new Retrieval
                        {
                            Description = items.Description,
                            IdItem = items.IdItem,
                            StockUnit = items.StockUnit,
                            IdReqItem = ri.IdReqItem,
                            Unit = ri.Unit,
                            IdRequisition = ri.IdRequisiton
                        }).Where(x => x.IdRequisition == sr)
                        .FirstOrDefault();

                RequestedItem.Add(retrieval);
            }
            List<Retrieval> ItemRetrieve = new List<Retrieval>();
            ItemRetrieve = RequestedItem.GroupBy(x => x.IdItem)
                            .Select(y => new Retrieval
                            {
                                Description = y.First().Description,
                                IdItem = y.First().IdItem,
                                StockUnit = y.First().StockUnit,
                                IdReqItem = y.Sum(z => z.IdReqItem),            // No physical usage after groupby
                                IdRequisition = y.Sum(z => z.IdRequisition),    // No physical usage after groupby
                                Unit = y.Sum(z => z.Unit)
                            }).ToList();

            return ItemRetrieve;

        }
    }
}