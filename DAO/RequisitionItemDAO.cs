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