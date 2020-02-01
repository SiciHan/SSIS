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

        public List<int> CreateDisbursementItem(List<int> PKDisbursement, List<Retrieval> RetrievalItem)
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

            List<int> PKDisbursementItem = new List<int>();
            // Get Disbursement with IdDisbursement 
            foreach (int id in PKDisbursement)
            {
                Disbursement disbursement = context.Disbursements
                                            .Where(x => x.IdDisbursement == id)
                                            .FirstOrDefault();

                string DptCode = disbursement.CodeDepartment;
                Debug.WriteLine(DptCode);
                // Create DisbursementItem List using Retrieval with CodeDepartment equals DptCode
                var RequestedItemByDept = Retrieval.Where(ri => ri.CodeDepartment.Equals(DptCode))
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
                    PKDisbursementItem.Add(pk);
                }
            }

            return PKDisbursementItem;
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