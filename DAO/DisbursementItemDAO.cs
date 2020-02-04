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

        public List<int> GetIdByItemRetrieved(List<string> DClerk, int[] IdItemRetrieved)
        {
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

                }
            }
            return IdDisbursementItemClerk;
        }

        public List<int> UpdateDisbursementItem(List<int> IdDisbursementItemClerk)
        {
            List<int> IdDisbursement = new List<int>();
            
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
            return IdDisbursement;
        }

        public void DisbursementItemByPriority(List<Retrieval> RetrievalItem)
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
            // Select IdItem where total requested unit is more than stock unit
            var IdItem = Retrieval.Where(ri => ri.Unit > ri.StockUnit)
                                  .Select(ri => ri.IdItem);

            // Select IdDisbursementItem order by ApprovedDate that stock unit is less than sum of the unit request
            List<int> IdDisbursementItem = new List<int>();
            foreach (var iditem in IdItem) 
            {
                var JoinTable = context.DisbursementItems
                                    .Join(context.Items, di => di.IdItem, i => i.IdItem,
                                    (di, i) => new { di, i })
                                    .Join(context.RequisitionItems, idi => idi.i.IdItem, ri => ri.IdItem,
                                    (idi, ri) => new { idi, ri })
                                    .Join(context.Requisitions, riidi => riidi.ri.IdRequisiton, r => r.IdRequisition,
                                    (riidi, r) => new { riidi, r })
                                    .Where(x => x.riidi.ri.IdItem == iditem)
                                    .OrderBy(x => x.r.ApprovedDate)
                                    .Select(x => x.riidi.idi.di.IdDisbursementItem);
                if (JoinTable != null)
                    foreach (var jt in JoinTable)
                    { 
                        if (!IdDisbursementItem.Any())
                            IdDisbursementItem.Add(jt);
                        else if (!IdDisbursementItem.Contains(jt))
                            IdDisbursementItem.Add(jt);
                    }
            }

            // Distribute stock item based on approved date given stockunit less than sum of requested unit
            foreach (var iditem in IdItem)
            {
                int stockunit = context.Items.Where(i => i.IdItem == iditem)
                                             .Select(i => i.StockUnit).FirstOrDefault();

                foreach (var id in IdDisbursementItem)
                {
                    DisbursementItem disbursementItem = context.DisbursementItems
                                                               .Where(di => di.IdDisbursementItem == id)
                                                               .Where(di => di.IdItem == iditem).FirstOrDefault();

                    if (disbursementItem != null)
                    {
                        if (disbursementItem.UnitIssued >= stockunit)
                        {
                            disbursementItem.UnitIssued = stockunit;
                            stockunit = 0;
                            context.SaveChanges();
                        }
                        else
                        {
                            stockunit = stockunit - disbursementItem.UnitIssued;
                        }
                    }

                }
            }
            
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

        //James
        internal void GiveAndTake(IList<int> disbItemId, IList<int> transferQtyNum, IList<int> disbItemIdDeptFrom, int IdStoreClerk)
        {
            //Debugging and PoC
            /*System.Diagnostics.Debug.WriteLine($"disbItemId Count: {disbItemId.Count}, transferQtyNum Count: {transferQtyNum.Count}");
            foreach (int i in disbItemId)
                System.Diagnostics.Debug.WriteLine("disbItemId: " + i);

            foreach (int i in transferQtyNum)
                System.Diagnostics.Debug.WriteLine("transferQtyNum: " + i);

            for (int i = 0; i < disbItemId.Count; i++)
            {
                System.Diagnostics.Debug.WriteLine("disbItemId: " + disbItemId[i]);
            }
            foreach (int i in disbItemIdDeptFrom)
                System.Diagnostics.Debug.WriteLine("disbItemIdDeptFrom: " + i);
            System.Diagnostics.Debug.WriteLine($"disbItemId Count: {disbItemId.Count}, transferQtyNum Count: {transferQtyNum.Count}, disbItemIdDeptFrom Count: {disbItemIdDeptFrom.Count}");*/
            DisbursementItem DItoReceive;
            DisbursementItem DItoGive;
            int DIid;
            int DIidDeptFrom;

            for (int i = 0; i < disbItemId.Count; i++)
            {
                //System.Diagnostics.Debug.WriteLine("disbItemId: " + disbItemId[i]);
                if(disbItemIdDeptFrom[i] != 0)
                {
                    DIid = disbItemId[i];
                    DIidDeptFrom = disbItemIdDeptFrom[i];
                    // 1. get the DI objects for DItoReceive and DItoGive
                    DItoReceive = context.DisbursementItems.SingleOrDefault(di => di.IdDisbursementItem == DIid);
                    DItoGive = context.DisbursementItems.SingleOrDefault(di => di.IdDisbursementItem == DIidDeptFrom);

                    // 2. DItoGive.UnitIssued -= transferQtyNum[i]
                    DItoGive.UnitIssued -= transferQtyNum[i];

                    // 2a. Create reversal entry for StockRecord for the ItemId
                    StockRecord reverseSr = new StockRecord
                    {
                        Date = DateTime.Now,
                        IdOperation = 1,
                        IdDepartment = DItoGive.Disbursement.CodeDepartment,
                        IdStoreClerk = IdStoreClerk,
                        IdItem = DItoGive.IdItem,
                        Unit = transferQtyNum[i] // positive for the reversal entry as we are taking back the units from that dept
                    };
                    context.StockRecords.Add(reverseSr);

                    // 3. DItoReceive.UnitIssued += transferQtyNum[i]
                    DItoReceive.UnitIssued += transferQtyNum[i];

                    // 3a. Create new entry for StockRecord for the ItemId
                    StockRecord newSr = new StockRecord
                    {
                        Date = DateTime.Now,
                        IdOperation = 1,
                        IdDepartment = DItoReceive.Disbursement.CodeDepartment,
                        IdStoreClerk = IdStoreClerk,
                        IdItem = DItoReceive.IdItem,
                        Unit = transferQtyNum[i] * -1 // negative for the new entry as we are disbursing more units to that dept
                    };
                    context.StockRecords.Add(newSr);

                    // wrap this in a transaction or db.SaveChanges()
                    context.SaveChanges();
                }
            }

        }
        
        //James
        internal List<DisbursementItem> FindCorrespondingDisbursementItems(ICollection<DisbursementItem> disbursementItems, int IdStoreClerk)
        {
            List<DisbursementItem> di = new List<DisbursementItem>();
            // Check IdStoreClerk selected collection point
            List<int> CPClerk = new List<int>();
            CPClerk = context.CPClerks
                      .Where(x => x.IdStoreClerk == IdStoreClerk)
                      .Select(x => x.IdCollectionPt).ToList();

            // for every disbItem, this will send along the list of other departments' items which are only "Prepared"
            // this would be for the dynamic dropdownlist used in selecting which dept to take items from
            foreach (DisbursementItem i in disbursementItems)
            {
                di.AddRange(context.DisbursementItems
                    .Where(x => x.Disbursement.IdStatus == 9 && 
                    x.IdDisbursement != i.IdDisbursement && 
                    x.IdItem == i.IdItem &&
                    CPClerk.Contains((int)x.Disbursement.IdCollectionPt))
                    .ToList());
            }

            return di;
        }

        //James
        internal void UpdateUnitIssued(IList<int> disbItemId, IList<int> qtyDisbursed)
        {
            DisbursementItem disbItem;
            int DIiD;
            for (int i = 0; i < disbItemId.Count; i++)
            {
                DIiD = disbItemId[i];
                disbItem = context.DisbursementItems.Single(di => di.IdDisbursementItem == DIiD); ;
                disbItem.UnitIssued = qtyDisbursed[i];
            }

            context.SaveChanges();
        }
    }
}