using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.DAO
{
    public class RequisitionDAO
    {
        private readonly SSISContext context;

        public RequisitionDAO()
        {
            this.context = new SSISContext();
        }

        public List<int> SearchRequisitionForRetrival(DateTime Yesterday, DateTime LastThu)
        {
            List<int> SelectedRequisition = new List<int>();
            var requisition = from r in context.Requisitions
                                    where r.IdStatusCurrent == 3
                                    where r.ApprovedDate <= Yesterday
                                    where r.ApprovedDate >= LastThu
                                    select r.IdRequisition;

            foreach (var id in requisition)
                SelectedRequisition.Add(id);
            return SelectedRequisition;             
        }

        public void RetrieveRequisition(int IdStoreClerk, DateTime StartDate, DateTime EndDate)
        {
            // Check IdStoreClerk selected collection point
            List<int> CPClerk = new List<int>();
            CPClerk = context.CPClerks
                      .Where(x => x.IdStoreClerk == IdStoreClerk)
                      .Select(x => x.IdCollectionPt).ToList();

            // Check Department that have selected the same collection point as the storeclerk
            List<String> DClerk = new List<String>();
            foreach (int CollectionPt in CPClerk)
            {
                var CodeDepartment = context.Departments
                                    .Where(x => x.IdCollectionPt == CollectionPt)
                                    .Select(x => x.CodeDepartment);
                foreach (var cd in CodeDepartment)
                {
                    DClerk.Add(cd);
                }
            }

            // Get Employee That is working in Department from DClerk
            List<int> IdEmployee = new List<int>();
            foreach (string CodeDpt in DClerk)
            {
                var IdEmp = context.Employees
                                    .Where(x => x.CodeDepartment.Equals(CodeDpt))
                                    .Select(x => x.IdEmployee);
                foreach (var ie in IdEmp)
                    IdEmployee.Add(ie);
            }

            // Get approved requisition between startdate and enddate
            // Ignore employee for the moment
            List<int> IdRequestedItem = new List<int>();
            foreach (int ie in IdEmployee) 
            {
                var IdReqItem = context.Requisitions
                                    .Where(x => x.IdStatusCurrent == 3)
                                    .Where(x => x.ApprovedDate <= StartDate)
                                    .Where(x => x.ApprovedDate >= EndDate)
                                    .Where(x => x.IdEmployee == ie)
                                    .Select(x => x.IdRequisition);
                foreach (var iri in IdReqItem)
                    IdRequestedItem.Add(iri);
            }
            Debug.WriteLine(IdRequestedItem);
                     
            // Get DepartmentCode from approved requistion Id and where id Employee equals to IdEmplloyee
            List<>

            List<Retrieval> RequestedItem = new List<Retrieval>();
            foreach (var sr in SelectedReqisition)
            {
                
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