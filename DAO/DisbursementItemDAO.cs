using System;
using System.Collections.Generic;
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

        //James
        internal void GiveAndTake(IList<int> disbItemId, IList<int> transferQtyNum, IList<int> disbItemIdDeptFrom)
        {
            System.Diagnostics.Debug.WriteLine($"disbItemId Count: {disbItemId.Count}, transferQtyNum Count: {transferQtyNum.Count}, disbItemIdDeptFrom Count: {disbItemIdDeptFrom.Count}");
            DisbursementItem DItoReceive;
            DisbursementItem DItoGive;
            int DIid;
            int DIidDeptFrom;

            for (int i = 0; i < disbItemId.Count; i++)
            {
                System.Diagnostics.Debug.WriteLine("disbItemId: " + disbItemId[i]);
                if(disbItemIdDeptFrom[i] != 0)
                {
                    DIid = disbItemId[i];
                    DIidDeptFrom = disbItemIdDeptFrom[i];
                    // 1. get the DI objects for DItoReceive and DItoGive
                    DItoReceive = context.DisbursementItems.SingleOrDefault(di => di.IdDisbursementItem == DIid);
                    DItoGive = context.DisbursementItems.SingleOrDefault(di => di.IdDisbursementItem == DIidDeptFrom);

                    // 2. DItoGive.UnitIssued -= transferQtyNum[i]
                    DItoGive.UnitIssued -= transferQtyNum[i];

                    // 3. DItoReceive.UnitIssued += transferQtyNum[i]
                    DItoReceive.UnitIssued += transferQtyNum[i];

                    // wrap this in a transaction or db.SaveChanges()
                    context.SaveChanges();
                }
            }

        }
        
        //James
        internal List<DisbursementItem> FindCorrespondingDisbursementItems(ICollection<DisbursementItem> disbursementItems)
        {
            List<DisbursementItem> di = new List<DisbursementItem>();
            // for every disbItem, this will send along the list of other departments' items which are only "Prepared"
            // this would be for the dynamic dropdownlist used in selecting which dept to take items from
            foreach (DisbursementItem i in disbursementItems)
            {
                di.AddRange(context.DisbursementItems
                    .Where(x => x.Disbursement.IdStatus == 9 && x.IdDisbursement != i.IdDisbursement && x.IdItem == i.IdItem)
                    .ToList());
            }

            return di;
        }
    }
}