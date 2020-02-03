using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.DAO
{
    public class DisbursementItemDAO
    {
        private readonly SSISContext context;

        public DisbursementItemDAO()
        {
            this.context = new SSISContext();
        }

        public List<int> CreateDisbursementItem(List<int> IdDisbursement, List<Retrieval> RetrievalItem)
        {
            // RetrievalItem Group by CodeDepartment, IdItem
            var Retrieval = RetrievalItem.GroupBy(x => new { x.IdItem, x.CodeDepartment })
                            .Select(y => new Retrieval
                            {
                                IdItem = y.First().IdItem,
                                StockUnit = y.First().StockUnit,
                                CodeDepartment = y.First().CodeDepartment,
                                Unit = y.Sum(z => z.Unit)
                            }).ToList();

            List<int> IdDisbursementItem = new List<int>();
            // Get Disbursement with IdDisbursement 
            foreach (int id in IdDisbursement)
            {
                Disbursement disbursement = context.Disbursements
                                            .Where(x => x.IdDisbursement == id)
                                            .FirstOrDefault();

                string DptCode = disbursement.CodeDepartment;
                // Create DisbursementItem List using Retrieval with CodeDepartment equals DptCode
                var RequestedItemByDept = Retrieval.Where(ri => ri.CodeDepartment.Equals(DptCode))
                                                    .OrderBy(ri => ri.ApprovedDate)
                                                    .ToList();

                // Status of "preparing"
                Status status = context.Status.Where(s => s.IdStatus == 8).FirstOrDefault();
                foreach (var ribd in RequestedItemByDept)
                {
                    Item items = context.Items.Where(i => i.IdItem == ribd.IdItem).FirstOrDefault();
                    Debug.WriteLine(ribd.IdItem);

                    DisbursementItem NewDisbursementItem = new DisbursementItem();
                    NewDisbursementItem.IdDisbursement = id;
                    NewDisbursementItem.Disbursement = disbursement;
                    NewDisbursementItem.IdItem = ribd.IdItem;
                    NewDisbursementItem.Item = items;
                    NewDisbursementItem.IdStatus = 8;
                    NewDisbursementItem.Status = status;
                    NewDisbursementItem.UnitRequested = ribd.Unit;
                    NewDisbursementItem.UnitIssued = ribd.Unit;

                    // If stockunit is less that request unit, it will update once become "prepared" 
                    /*if (ribd.StockUnit < ribd.Unit)
                    {
                        NewDisbursementItem.UnitRequested = ribd.StockUnit;
                        NewDisbursementItem.UnitIssued = ribd.StockUnit;
                    }
                    else
                    {
                        NewDisbursementItem.UnitRequested = ribd.Unit;
                        NewDisbursementItem.UnitIssued = ribd.Unit;
                    }*/

                    context.DisbursementItems.Add(NewDisbursementItem);
                    context.SaveChanges();

                    // Get Id of Created DisbursementItem
                    int pk = NewDisbursementItem.IdDisbursementItem;
                    IdDisbursementItem.Add(pk);
                }
            }

            return IdDisbursementItem;
        }

        public List<int> UpdateDisbursementItem(List<string> DClerk, int[] IdItemRetrieved)
        {
            // Join DisbursementItem and disbursement look for code department that is under user clerk
            // Search for DisbursementItem that have IdItem and status "preparing"
            List<int> IdDisbursement = new List<int>();
            List<int> IdDisbursementItemClerk = new List<int>();
            foreach (string CodeDpt in DClerk)
            {
                foreach (int id in IdItemRetrieved) 
                {
                    var IdDisbursementItem = context.Disbursements
                                                .Join(context.DisbursementItems,
                                                d => d.IdDisbursement, di => di.IdDisbursement,
                                                (d, di) => new { d, di })
                                                .Where(x => x.di.IdItem == id)
                                                .Where(x => x.d.CodeDepartment.Equals(CodeDpt))
                                                .Where(x => x.di.IdStatus == 8)
                                                .Select(x => x.di.IdDisbursementItem);

                    if (IdDisbursementItem != null)
                    {
                        foreach (var idDI in IdDisbursementItem)
                            IdDisbursementItemClerk.Add(idDI);
                    }
                    
                    foreach (int diid in IdDisbursementItemClerk)
                    {
                        DisbursementItem disbursementitem = context.DisbursementItems
                                                                .Where(x => x.IdDisbursementItem == diid)
                                                                .FirstOrDefault();


                        if (disbursementitem != null)
                        {
                            Status status = context.Status.Where(s => s.IdStatus == 9).FirstOrDefault();
                            disbursementitem.IdStatus = 9;
                            context.SaveChanges();

                            // As long as there is one item in disbursementitem that has been set to "prepared"
                            // It will retrieve it's IdDisbursement and set as "pepared"
                            if (!IdDisbursement.Contains(disbursementitem.IdDisbursement))
                                IdDisbursement.Add(disbursementitem.IdDisbursement);
                        }
                    }

                }
            }
            return IdDisbursement;
        }

        public List<JoinDandDI> FindDetailDisbursement(int IdDisbursement)
        {
            var DetailDisbursement = context.Disbursements
                                        .Join(context.DisbursementItems,
                                        d => d.IdDisbursement, di => di.IdDisbursement,
                                        (d, di) => new { d, di })
                                        .Join(context.Departments,
                                        ddi => ddi.d.CodeDepartment, dpt => dpt.CodeDepartment,
                                        (ddi, dpt) => new { ddi, dpt })
                                        .Join(context.Status,
                                        ddidpt => ddidpt.ddi.d.IdStatus, s => s.IdStatus,
                                        (ddidpt, s) => new JoinDandDI
                                        {
                                            disbursement = ddidpt.ddi.d,
                                            disbursementItem = ddidpt.ddi.di,
                                            department = ddidpt.dpt,
                                            status = s
                                        }).Where(x => x.disbursement.IdDisbursement == IdDisbursement)
                                        .ToList();
            return DetailDisbursement;
        }
    }
}